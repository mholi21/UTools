using Rocket.API;
using Rocket.API.Collections;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;

namespace UTools.Commands
{
    class CommandUptime
    {
        public List<string> Aliases
        {
            get
            {
                return new List<string>()
                {
                    "up"
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
                return "Show server uptime!";
            }
        }

        public string Name
        {
            get
            {
                return "uptime";
            }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>()
                {
                    "uptime"
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

        public CommandUptime()
        {
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            int totalSeconds = (int)(DateTime.UtcNow - UTools.Started).TotalSeconds;
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
            object[] objArray = new object[] { UTools.Started.ToString(), str };
            UnturnedChat.Say(caller, translationLists.Translate("running_since", objArray));
        }
    }
}

