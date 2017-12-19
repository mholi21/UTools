using Rocket.API;
using Rocket.API.Collections;
using Rocket.Unturned.Chat;
using System.Collections.Generic;
using PlayerInfoLibrary;
using Rocket.Unturned.Player;
using Rocket.Core.Plugins;
using UnityEngine;

namespace UTools.Commands
{
    public class CommandPlayed : IRocketCommand
    {
        public List<string> Aliases
        {
            get
            {
                return new List<string>()
                {
                    "played"
                };
            }
        }

        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Both;
            }
        }

        public string Help
        {
            get
            {
                return "How much time player spent playing on server.";
            }
        }

        public string Name
        {
            get
            {
                return "played";
            }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>()
                {
                    "played"
                };
            }
        }

        public string Syntax
        {
            get
            {
                return "";
            }
        }

        public CommandPlayed()
        {
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            //Requires PlayerInfoLib to work, if not loaded time won't be tracked
            if (RocketPlugin.IsDependencyLoaded("PlayerInfoLib"))
            {
                int totalSeconds = PlayerInfoLib.Database.QueryById(UnturnedPlayer.FromName(caller.DisplayName).CSteamID, true).TotalPlayime;
                string str = "";
                if (totalSeconds >= 86400)
                {
                    int num = totalSeconds / 86400;
                    str = string.Concat(num.ToString(), "d ");
                }
                if (totalSeconds >= 3600)
                {
                    int num1 = totalSeconds / 3600 % 24;
                    str = string.Concat(str, num1.ToString(), "h ");
                }
                if (totalSeconds >= 60)
                {
                    int num2 = totalSeconds / 60 % 60;
                    str = string.Concat(str, num2.ToString(), "m ");
                }
                int num3 = totalSeconds % 60;
                str = string.Concat(str, num3.ToString(), "s");
                TranslationList translationLists = UTools.Instance.Translations.Instance;
                object[] objArray = new object[] { str };
                UnturnedChat.Say(caller, translationLists.Translate("total_playtime", objArray), Color.yellow);
            }
            else
            {
                UnturnedChat.Say(caller, "Playtime is not being tracked!", Color.yellow);
            }
        }
    }
}
