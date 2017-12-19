using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;


namespace UTools.Commands
{
    public class CommandLocation : IRocketCommand
    {
        public List<string> Aliases
        {
            get
            {
                return new List<string>()
                {
                    "loc",
                    "l"
                };
            }
        }

        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Player;
            }
        }

        public string Help
        {
            get
            {
                return "Get exact coordinates for your location";
            }
        }

        public string Name
        {
            get
            {
                return "location";
            }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>()
                {
                    "location"
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

        public CommandLocation()
        {
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (caller.IsAdmin || caller.HasPermission("location") )
            {
                UnturnedPlayer unturnedPlayer = UnturnedPlayer.FromName(caller.DisplayName);
                UnturnedChat.Say(caller, string.Concat(new object[] { "Position x:", unturnedPlayer.Position.x, " y:", unturnedPlayer.Position.y, " z:", unturnedPlayer.Position.z }), Color.yellow);
            }
        }
    }
}