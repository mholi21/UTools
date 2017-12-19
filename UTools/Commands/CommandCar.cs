using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace UTools.Commands
{
    public class CommandCar : IRocketCommand
    {
        public string Name
        {
            get { return "car"; }
        }

        public string Help
        {
            get { return "Spawn random car!"; }
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

        public CommandCar()
        {
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer unturnedPlayer = (UnturnedPlayer)caller;
            unturnedPlayer.GiveVehicle((ushort)randomID(168, new int[] { 0, 97, 98, 99, 100, 101, 102, 103, 104, 105, 108, 109, 120, 121, 124, 137, 140 }));
        }

        public static int randomID(int n, int[] x)
        {
            System.Random r = new System.Random();
            int result = r.Next(n - x.Length);

            for (int i = 0; i < x.Length; i++)
            {
                if (result < x[i])
                    return result;
                result++;
            }
            return result;
        }
    }
}