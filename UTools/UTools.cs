using Rocket.Core.Plugins;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Rocket.Unturned;
using SDG.Unturned;
using UnityEngine;
using Steamworks;
using System;
using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Items;
using fr34kyn01535.Uconomy;
using Rocket.Core;
using System.Collections.Generic;
using Rocket.API.Collections;

using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

using UTools.Database;
using UTools.Commands;
using UTools.Misc;

namespace UTools
{
    public class UTools : RocketPlugin<UToolsConfiguration>
    {
        public static UTools Instance;
        public static DateTime Started;
        public DatabaseManagerAuction DatabaseAuction;
        public DatabaseManagerMute Database;
        public static Dictionary<CSteamID, string> Players = new Dictionary<CSteamID, string>();

        static UTools()
        {
            Started = DateTime.UtcNow;
        }

        public UTools()
        {
        }

        protected override void Load()
        {
            Instance = this;

            if (Configuration != null && Configuration.Instance.WebsiteCommands.Count != 0)
            {
                foreach (var command in Configuration.Instance.WebsiteCommands)
                {
                    CommandWeblink cmd = new CommandWeblink(command.CommandName, command.Desc, command.Url, command.Help);
                    R.Commands.Register(cmd);
                }
            }
            if (Instance.Configuration.Instance.AllowAuction) { DatabaseAuction = new DatabaseManagerAuction(); }

            UnturnedPlayerEvents.OnPlayerChatted += OnPlayerChatted;
            UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath;
            UnturnedPlayerEvents.OnPlayerInventoryAdded += IR_ItemAdded;
            Level.onLevelLoaded += OnLevelLoaded;
            U.Events.OnPlayerConnected += PlayerConnected;
            U.Events.OnPlayerDisconnected += PlayerDisconnected;

            Rocket.Core.Logging.Logger.LogWarning("--");
            int num = 0;
            foreach (UItem item in Configuration.Instance.Items)
            {
                num++;
            }
            Rocket.Core.Logging.Logger.LogWarning(string.Concat("Black listed items found: ", num));
            Rocket.Core.Logging.Logger.LogWarning("--");
        }

        protected override void Unload()
        {
            Instance = this;

            foreach (var command in Configuration.Instance.WebsiteCommands)
            {
                R.Commands.DeregisterFromAssembly(this.Assembly);
            }

            UnturnedPlayerEvents.OnPlayerChatted -= OnPlayerChatted;
            UnturnedPlayerEvents.OnPlayerDeath -= OnPlayerDeath;
            UnturnedPlayerEvents.OnPlayerInventoryAdded -= IR_ItemAdded;
            Level.onLevelLoaded -= OnLevelLoaded;
            U.Events.OnPlayerConnected -= PlayerConnected;
            U.Events.OnPlayerDisconnected -= PlayerDisconnected;
        }

        private void OnLevelLoaded(int level)
        {
            //HIDE SERVER CONFIG CHANGES
            SteamGameServer.SetKeyValue("Browser_Config_Count", null);
        }

        public void OnPlayerChatted(UnturnedPlayer player, ref Color color, string message, EChatMode chatMode, ref bool cancel)
        {
            if (Database.IsChatBanned(player.ToString()) != null)
            {
                cancel = true;
                UnturnedChat.Say(player, "You are muted!", Color.red);
            }
            if (Configuration.Instance.EnableDiscord)
            {
                DiscordHook(player.CharacterName, message, chatMode.ToString());
            }
        }

        public void DiscordHook(string sender, string message, string chatMode)
        {
            int color = 15844367;
            switch (chatMode)
            {
                case "GLOBAL":
                    color = 4444444;
                    break;
                case "LOCAL":
                    color = 222;
                    break;
                case "GROUP":
                    color = 15844367;
                    break;
                default:
                    color = 15158332;
                    break;
            }
            if (message.StartsWith("/"))
            {
                color = 15158332;
            }
            //CRAZY TEMP HACK REPLACE " WITH \\\" for JSON escape of special character
            string newMessage = message.Replace("\"", "\\\\\\\"");
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Instance.Configuration.Instance.DiscordWebhook);
                //SSL CERTIFICATION CHECK
                ServicePointManager.ServerCertificateValidationCallback =
                delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    //DATA EXAMPLE! {"embed":{ "description":"Long message example with unturned limit of 80 chars? or what is it? 120?","color":15844367,"author": { "name":"PlayerName [Group] [11/12/2017 5:10:41 PM]"} } }
                    string json = "{\"embeds\":[{\"description\":\"" + newMessage + "\",\"color\":" + color + ",\"author\":{\"name\":\"" + sender + " [" + chatMode + "] [" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]\"}}]}";
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }
                httpWebRequest.GetResponse().Close();
            }
            catch (WebException ex)
            {
                Rocket.Core.Logging.Logger.LogWarning(ex.Message);
            }
        }

        private void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            UToolsOnPlayerDeathEvent.PrintMessage(player, cause, limb, murderer);
        }

        private void PlayerConnected(UnturnedPlayer player)
        {
            string str = PlayerPermission(player);
            if (str != "none")
            {
                foreach (Group gropu in Configuration.Instance.MOTDGroups)
                {
                    if (str == string.Concat("m.", gropu.Name.ToLower()))
                    {
                        foreach (Message message in gropu.Messages)
                        {
                            try
                            {
                                string text = (new LineText(message.text, player)).getText();
                                UnturnedChat.Say(player, text, Color.yellow);
                            }
                            catch (Exception exception)
                            {
                                Rocket.Core.Logging.Logger.LogError(string.Concat("[MOTD msg] ", exception.Message));
                            }
                        }
                        return;
                    }
                }
            }

            if (player.IsAdmin)
            {
                player.GodMode = true;
                if (Configuration.Instance.DutyAnnounceLog)
                {
                    UnturnedChat.Say(Instance.Translate("admin_login_message", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(Instance.Configuration.Instance.DutyMsgColor, Color.red));
                }
            }
        }

        public string PlayerPermission(UnturnedPlayer player)
        {
            string str;
            int num = 0;
            string str1 = "invalid";
            foreach (Group gropu in Configuration.Instance.MOTDGroups)
            {
                if (IRocketPlayerExtension.HasPermission(player, string.Concat("m.", gropu.Name.ToLower())))
                {
                    num++;
                    str1 = string.Concat("m.", gropu.Name.ToLower());
                }
            }
            if (num == 1)
            {
                str = str1;
            }
            else if (num != 0)
            {
                if (Configuration.Instance.MOTDShowConsoleWarning)
                {
                    Rocket.Core.Logging.Logger.LogWarning(string.Concat("[MOTD msg] Player ", player.DisplayName, " has more than one permission. We will show messages only from latest group to him"));
                }
                str = str1;
            }
            else
            {
                if (Configuration.Instance.MOTDShowConsoleWarning)
                {
                    Rocket.Core.Logging.Logger.LogWarning(string.Concat("[MOTD msg] Cant find permission for player ", player.DisplayName, ". Nothing will be shown to  him"));
                }
                str = "none";
            }
            return str;
        }

        private void PlayerDisconnected(UnturnedPlayer player)
        {
            if (player.IsAdmin)
            {
                if (Configuration.Instance.DutyRemoveAdminOnLogout)
                {
                    player.Admin(false);
                    player.Features.GodMode.Equals(false);
                    player.Features.VanishMode.Equals(false);
                    if (Configuration.Instance.DutyAnnounceLog)
                    {
                        UnturnedChat.Say(Instance.Translate("admin_logoff_message", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(Instance.Configuration.Instance.DutyMsgColor, Color.red));
                    }
                }
            }
        }

        public static KeyValuePair<CSteamID, string> GetPlayer(string search)
        {
            foreach (KeyValuePair<CSteamID, string> pair in Players)
            {
                if (pair.Key.ToString().ToLower().Contains(search.ToLower()) || pair.Value.ToLower().Contains(search.ToLower()))
                {
                    return pair;
                }
            }
            return new KeyValuePair<CSteamID, string>(new CSteamID(0), null);
        }

        private void IR_ItemAdded(UnturnedPlayer player, InventoryGroup inventoryGroup, byte inventoryIndex, ItemJar P)
        {
            if ((!player.IsAdmin ? true : !Configuration.Instance.IRIgnoreAdmin))
            {
                foreach (UItem item in Configuration.Instance.Items)
                {
                    if (item.ID == P.item.id)
                    {
                        ExecuteDependencyCode("Uconomy", (IRocketPlugin plugin) =>
                        {
                            Uconomy Uconomy = (Uconomy)plugin;
                            Uconomy.Database.IncreaseBalance(player.CSteamID.ToString(), item.Money.Value);
                        });

                        while (true)
                        {
                            if (!RemoveItem(player, P.item.id))
                                break;
                        }
                        UnturnedChat.Say(player, Translate("item_notPermitted", UnturnedItems.GetItemAssetById(item.ID).itemName, item.Money.Value), UnturnedChat.GetColorFromName(Configuration.Instance.ItemRemoveColor, UnityEngine.Color.red));

                    }
                }
            }
        }

        private bool RemoveItem(UnturnedPlayer player, ushort ItemID)
        {
            for (byte page = 0; page < PlayerInventory.PAGES; page++)
            {
                byte itemCount = 0;
                bool hasPage = false;
                try
                {
                    itemCount = player.Inventory.getItemCount(page);
                    hasPage = true;
                }
                catch
                {
                }

                if (hasPage)
                {

                    for (byte index = 0; index < itemCount; index++)
                    {
                        if (player.Player.inventory.getItem(page, index).item.id == ItemID)
                        {
                            player.Inventory.removeItem(page, index);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList()
                {
                    {"invalid_arg", "Invalid Arguments."},
                    {"running_since", "Running since: {0} UTC, running for: {1}"},
                    {"total_playtime", "You played for: {0}"},
                    {"admin_login_message", "{0} is now on duty"},
                    {"admin_logout_message", "{0} is now off duty"},
                    {"on_duty_message", "{0} is now on duty"},
                    {"off_duty_message", "{0} is now off duty"},
                    {"item_notPermitted", "Removed {0} and got {1} money!"},
                    {"killed_by_zombie", "{0} has been killed by zombie in {1}!"},
                    {"killed_by_gun", "{0} shot {1} in {4} with {2} from {3} m!"},
                    {"killed_by_melee", "{0} has melee'd {1} in {2} to death!"},
                    {"killed_by_punch", "{0} has punched {1} in {2} to death!"},
                    {"killed_by_roadkill", "{0} ran over {1} with {2} going {3} kph!"},
                    {"killed_by_vehicle", "{0} died from explosion of vehicle!"},
                    {"killed_by_starvation", "{0} starved to death!"},
                    {"killed_by_dehydration", "{0} dehydrated to death!"},
                    {"killed_by_infection", "{0} died from an infection!"},
                    {"killed_by_bleeding", "{0} has bleed to death!"},
                    {"killed_by_landmine", "{0} died from explosion of landmine!"},
                    {"killed_by_suicide", "{0} died by his own hand!"},
                    {"killed_by_breath", "{0} died due to lack of oxygen!"},
                    {"killed_by_bones", "{0} died by braking his bones!"},
                    {"killed_by_freezing", "{0} froze to death!"},
                    {"killed_by_burning", "{0} burned to death!"},
                    {"killed_by_animal", "{0} got killed by animal!"},
                    {"killed_by_kill", "{0} got killed mysteriously!"},
                    {"killed_by_grenade", "{1} got killed by grenade from {0}!"},
                    {"killed_by_shred", "{0} got shredded to bits!"},
                    {"killed_by_arena", "{0} got finished off by arena!"},
                    {"killed_by_missile", "{1} got killed by missile from {0}!"},
                    {"killed_by_charge", "{1} got killed by charges {0}!"},
                    {"killed_by_splash", "{0} got killed by explosive bullet!"},
                    {"killed_by_sentry", "{0} got killed by sentry in {1}!"},
                    {"killed_by_acid", "{0} was blown up by zombie!"},
                    {"killed_by_boulder", "{0} was killed by boulder!"},
                    {"killed_by_burner", "{0} was blown up by burner zombie!"},
                    {"killed_by_spit", "{0} died from zombie spit!"},
                    {"mute_immune", "Player is immune to mute!"},
                    {"mute_public", "Player {0} was muted!"},
                    {"mute_private", "You are muted for {0}!"},
                    {"unmute_public", "{0} has been unmuted!"},
                    {"auction_command_usage","/auction add, /auction list, /auction find, /auction cancel"},
                    {"auction_addcommand_usage","/auction add <Item Name or ID> <Price>"},
                    {"auction_addcommand_usage2","Missing <Price> Parameter"},
                    {"auction_disabled","Auction is disabled."},
                    {"not_have_item_auction", "You do not have any {0} for auctioning."},
                    {"auction_item_notinshop","The item you are auctioning is not available from the shop."},
                    {"auction_item_mag_ammo","Unable to auction magazines or ammo!"},
                    {"auction_item_succes","You have placed {0} on auction for {1} {2}"},
                    {"auction_item_failed","Failed to place item on auction"},
                    {"auction_unequip_item","Please de-equip {0} first before auctioning"},
                    {"auction_buycommand_usage","To buy /auction buy <ID of auction>"},
                    {"auction_addcommand_idnotexist","Auction ID does not exist!"},
                    {"auction_buy_msg","You got item {0} for {1} {2}.  You now have {3} {4}."},
                    {"auction_notexist","Auction ID does not exist"},
                    {"auction_notown","You do not own that auction!"},
                    {"auction_cancelled","You have removed Auction {0}"},
                    {"auction_cancelcommand_usage","To cancle /auction cancel [Auction ID]"},
                    {"auction_findcommand_usage","To search /auction find [Item Name or ID]"},
                    {"auction_find_invalid","Invalid Item ID or Item Name"},
                    {"auction_find_failed","No item found with that ID/Name"},
                    {"auction_add_no_perm","You don't have permisson to add auctions."},
                    {"could_not_find", "Could not find {0} on auction."},
                    {"auction_price_low_high", "Incorrect price, min 30% [{0}] or max 150% [{1}] of shop price!"},
                };
            }
        }
    }
}
