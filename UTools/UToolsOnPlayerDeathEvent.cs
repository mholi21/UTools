using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Steamworks;
using System;

namespace UTools
{
    public class UToolsOnPlayerDeathEvent
    {
        public static void PrintMessage(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            if (cause.ToString() == "ZOMBIE")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_zombie", new object[] { player.DisplayName, CheckLimb(limb) }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "GUN")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_gun", new object[] { UnturnedPlayer.FromCSteamID(murderer).CharacterName, player.DisplayName, UnturnedPlayer.FromCSteamID(murderer).Player.equipment.asset.itemName.ToString(), Math.Round(Vector3.Distance(player.Position, UnturnedPlayer.FromCSteamID(murderer).Position), 2), CheckLimb(limb) }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
                CheckAdminKill(player, murderer);
            }
            else if (cause.ToString() == "MELEE")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_melee", new object[] { UnturnedPlayer.FromCSteamID(murderer).CharacterName, player.DisplayName, CheckLimb(limb) }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
                CheckAdminKill(player, murderer);
            }
            else if (cause.ToString() == "PUNCH")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_punch", new object[] { UnturnedPlayer.FromCSteamID(murderer).CharacterName, player.DisplayName, CheckLimb(limb) }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
                CheckAdminKill(player, murderer);
            }
            else if (cause.ToString() == "ROADKILL")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_roadkill", new object[] { UnturnedPlayer.FromCSteamID(murderer).CharacterName, player.DisplayName, UnturnedPlayer.FromCSteamID(murderer).CurrentVehicle.asset.vehicleName.ToString(), UnturnedPlayer.FromCSteamID(murderer).CurrentVehicle.speed }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
                CheckAdminKill(player, murderer);
            }
            else if (cause.ToString() == "VEHICLE")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_vehicle", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "FOOD")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_starvation", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "WATER")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_dehydration", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "INFECTION")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_infection", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "BLEEDING")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_bleeding", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "LANDMINE")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_landmine", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "BREATH")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_breath", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "BONES")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_bones", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "FREEZING")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_freezing", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "BURNING")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_burning", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "KILL")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_kill", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "ANIMAL")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_animal", new object[] { player.DisplayName, CheckLimb(limb) }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "GRENADE")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_grenade", new object[] { UnturnedPlayer.FromCSteamID(murderer).CharacterName, player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
                CheckAdminKill(player, murderer);
            }
            else if (cause.ToString() == "SHRED")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_shred", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "ARENA")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_arena", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "MISSILE")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_missile", new object[] { UnturnedPlayer.FromCSteamID(murderer).CharacterName, player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
                CheckAdminKill(player, murderer);
            }
            else if (cause.ToString() == "CHARGE")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_charge", new object[] { UnturnedPlayer.FromCSteamID(murderer).CharacterName, player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
                CheckAdminKill(player, murderer);
            }
            else if (cause.ToString() == "SPLASH")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_splash", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "SENTRY")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_sentry", new object[] { player.DisplayName, CheckLimb(limb) }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "ACID")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_acid", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "BOULDER")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_boulder", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "BURNER")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_burner", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "SPIT")
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_spit", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else if (cause.ToString() == "SUICIDE" && UTools.Instance.Configuration.Instance.DeathMessageSuicideShow)
            {
                UnturnedChat.Say(UTools.Instance.Translate("killed_by_suicide", new object[] { player.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DeathMsgColor, Color.red));
            }
            else
            {
                return;
            }
        }

        private static void CheckAdminKill(UnturnedPlayer player, CSteamID murderer)
        {
            if (UnturnedPlayer.FromCSteamID(murderer).IsAdmin)
            {
                UnturnedPlayer admin = UnturnedPlayer.FromCSteamID(murderer);
                if (admin.GodMode == true || admin.VanishMode == true)
                {
                    UnturnedChat.Say(player, "You got killed by admin (" + admin.CharacterName + ") with powers on.", Color.yellow);
                    UnturnedChat.Say(admin, "You killed with powers on!", Color.yellow);
                    Rocket.Core.Logging.Logger.Log(admin.CharacterName + " killed with powers on god: " + admin.GodMode.ToString() + " , vanish: " + admin.VanishMode.ToString(), ConsoleColor.Green);
                }
            }
            else
            {
                return;
            }
        }

        public static string CheckLimb(ELimb limb)
        {
            string limbd;
            if (limb.ToString() == "LEFT_BACK" || limb.ToString() == "LEFT_FRONT" || limb.ToString() == "RIGHT_BACK" || limb.ToString() == "RIGHT_FRONT" || limb.ToString() == "SPINE")
            {
                limbd = "body";
                return limbd;
            }
            else if (limb.ToString() == "SKULL")
            {
                limbd = "head";
                return limbd;
            }
            else if (limb.ToString() == "LEFT_FOOT" || limb.ToString() == "LEFT_LEG" || limb.ToString() == "RIGHT_FOOT" || limb.ToString() == "RIGHT_LEG")
            {
                limbd = "leg";
                return limbd;
            }
            else if (limb.ToString() == "LEFT_ARM" || limb.ToString() == "LEFT_HAND" || limb.ToString() == "RIGHT_ARM" || limb.ToString() == "RIGHT_HAND")
            {
                limbd = "arm";
                return limbd;
            }
            else
            {
                return limbd = "unknown";
            }
        }
    }
}
