using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System.Collections.Generic;

namespace UTools.Commands
{
    class CommandRemoveGroups : IRocketCommand
    {
        public List<string> Aliases
        {
            get
            {
                return new List<string>()
                {
                    "r"
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
                return "Remove all groups with priority less than 80";
            }
        }

        public string Name
        {
            get
            {
                return "remove";
            }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>();
            }
        }

        public string Syntax
        {
            get
            {
                return "<player name or ID>";
            }
        }

        public CommandRemoveGroups()
        {
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, "Invalid use!");
                return;
            }

            RocketPermissions rocketPerms = new RocketPermissions();
            bool var = true;

            if (command.Length == 1)
            {
                foreach (RocketPermissionsGroup group in rocketPerms.Groups)
                {
                    if (group.Priority < 80)
                    {
                        CommandWindow.onCommandWindowInputted.Invoke("p remove " + command[0] + " " + group.DisplayName, ref var);
                    }
                }
                return;
            }
            else
            {
                UnturnedChat.Say(caller, "Invalid use!");
                return;
            }
        }
    }
}
