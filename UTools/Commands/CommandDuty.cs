using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace UTools.Commands
{
    public class CommandDuty : IRocketCommand
    {
        public string Name
        {
            get { return "duty"; }
        }

        public string Help
        {
            get { return "Activate or deactivate admin status!"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>() { "d" }; }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Player; }
        }

        public List<string> Permissions
        {
            get { return new List<string>(); }
        }

        public string Syntax
        {
            get { return ""; }
        }

        public CommandDuty()
        {
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (caller != null)
            {
                UnturnedPlayer unturnedPlayer = (UnturnedPlayer)caller;
                Duty(unturnedPlayer);
            }
        }

        public void Duty(UnturnedPlayer caller)
        {
            if (!caller.IsAdmin)
            {
                caller.Admin(true);
                if (UTools.Instance.Configuration.Instance.DutyAnnounce)
                {
                    UnturnedChat.Say(UTools.Instance.Translate("on_duty_message", new object[] { caller.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DutyMsgColor, Color.red));
                }
            }
            else if (caller.IsAdmin == true)
            {
                caller.Admin(false);
                caller.GodMode = false;
                caller.VanishMode = false;
                if (UTools.Instance.Configuration.Instance.DutyAnnounce)
                {
                    UnturnedChat.Say(UTools.Instance.Translate("off_duty_message", new object[] { caller.DisplayName }), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DutyMsgColor, Color.red));
                }
            }
            else { return; }
        }
    }
}