using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace UTools.Commands
{
    class CommandWeblink : IRocketCommand
    {
        private string name;
        private string help;
        private string url;
        private string desc;

        public string Name
        {
            get { return name; }
        }

        public string Help
        {
            get { return help; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
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

        public CommandWeblink(string cmdName, string cmdDesc, string cmdURL, string cmdHelp)
        {
            this.name = cmdName;
            this.help = cmdHelp;
            this.url = cmdURL;
            this.desc = cmdDesc;
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer uCaller = (UnturnedPlayer)caller;

            uCaller.Player.channel.send("askBrowserRequest", uCaller.CSteamID, ESteamPacket.UPDATE_RELIABLE_BUFFER, desc, url);
        }
    }
}
