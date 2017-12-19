using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;
using UTools.Misc;

namespace UTools
{
    public class UToolsConfiguration : IRocketPluginConfiguration, IDefaultable
    {
        [XmlArrayItem(ElementName = "WebsiteCommand")]
        public List<WebsiteCommand> WebsiteCommands;

        public class WebsiteCommand
        {
            public string CommandName;
            public string Desc;
            public string Help;
            public string Url;
        }

        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public int DatabasePort;
        public string DatabaseChatbanTableName;
        public string DefaultMuteDuration;
        public string DatabaseAuction;
        public bool AllowAuction;
        public bool EnableShopPriceCheck;
        public bool EnableDiscord;

        public bool DeathMessageSuicideShow;
        public bool DutyAnnounce;
        public bool DutyAnnounceLog;
        public bool DutyRemoveAdminOnLogout;
        public bool IRIgnoreAdmin;
        public bool MOTDShowConsoleWarning;

        public string DeathMsgColor;
        public string MOTDMsgColor;
        public string DutyMsgColor;
        public string ItemRemoveColor;
        public string JoinMsgColor;
        public string DiscordWebhook;

        public List<Group> MOTDGroups;

        [XmlArrayItem(ElementName = "Item")]
        public List<UItem> Items;

        public UToolsConfiguration()
        {

        }

        public void LoadDefaults()
        {
            DatabaseAddress = "localhost";
            DatabaseUsername = "unturned";
            DatabasePassword = "password";
            DatabaseName = "unturned";
            DatabasePort = 3306;
            DatabaseChatbanTableName = "chatbans";
            DefaultMuteDuration = "5m";
            DatabaseAuction = "AuctionShop";
            AllowAuction = true;
            EnableShopPriceCheck = true;

            EnableDiscord = true;
            DiscordWebhook = "https://discordapp.com/api/webhooks/";

            DutyAnnounce = true;
            DutyAnnounceLog = false;
            DutyRemoveAdminOnLogout = false;
            DeathMessageSuicideShow = false;
            IRIgnoreAdmin = true;
            MOTDShowConsoleWarning = true;

            DutyMsgColor = "yellow";
            MOTDMsgColor = "yellow";
            DeathMsgColor = "red";
            ItemRemoveColor = "yellow";
            JoinMsgColor = "yellow";

            WebsiteCommands = new List<WebsiteCommand>()
            {
                new WebsiteCommand { CommandName = "website", Desc = "Open up our website homepage.", Help = "Open Homepage", Url = "https://example.com" },
                new WebsiteCommand { CommandName = "steam", Desc = "Opens up steam group for our servers", Help = "Opens steam group", Url = "http://steamcommunity.com/groups/" } };

            List<Group> MOTDgroups = new List<Group>();
            List<Message> MOTDmessages = new List<Message>()
            {
                new Message("Welcome %playername%, read /rules and check help /p", "yellow"),
                new Message("Join one of jobs and start %pvp/pve%", "yellow"),
                new Message("Check our forum form more info and to vote!", "yellow")
            };
            MOTDgroups.Add(new Group("default", MOTDmessages));            
            MOTDGroups = MOTDgroups;

            List<UItem> items = new List<UItem>()
            {
                new UItem(10, 10),
                new UItem(20, 0)
            };
            Items = items;
        }
    }
}
