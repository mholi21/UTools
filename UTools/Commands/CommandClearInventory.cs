using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace UTools.Commands
{
    class CommandClearInventory : IRocketCommand
    {
        public string Name
        {
            get { return "clearinventory"; }
        }

        public string Help
        {
            get { return "Clear your inventory!"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>() { "ci" }; }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Player; }
        }

        public List<string> Permissions
        {
            get { return new List<string>() { "ci" }; }
        }

        public string Syntax
        {
            get { return ""; }
        }

        public CommandClearInventory()
        {
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer unturnedPlayer = (UnturnedPlayer)caller;
            var playerInv = unturnedPlayer.Inventory;
            for (byte page = 0; page < SDG.Unturned.PlayerInventory.PAGES; page++)
            {
                if (page == SDG.Unturned.PlayerInventory.AREA)
                    continue;

                var count = playerInv.getItemCount(page);
                //page 7 (vault) - plugin, should not be cleared!
                if (page != 7)
                {
                    for (byte index = 0; index < count; index++)
                    {
                        playerInv.removeItem(page, 0);
                    }
                }  
            }
        }
    }
}
