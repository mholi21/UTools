using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Linq;

namespace UTools.Misc
{
    internal class LineText
    {
        private string _text;

        public LineText(string configText, UnturnedPlayer player)
        {
            configText = configText.Replace("%servername%", Provider.serverName);
            configText = configText.Replace("%playername%", player.CharacterName);
            int num = Provider.clients.Count();
            configText = configText.Replace("%online%", string.Concat(num.ToString(), "/", Provider.maxPlayers.ToString()));
            configText = configText.Replace("%adminsonline%", Admins());
            configText = configText.Replace("%mode%", Provider.mode.ToString().ToLower());
            configText = configText.Replace("%pvp/pve%", PvPorPvE());
            configText = configText.Replace("%map%", Provider.map);
            _text = configText;
        }

        private string Admins()
        {
            string str = "";
            foreach (SteamPlayer player in Provider.clients)
            {
                if (player.isAdmin)
                {
                    if (str != "")
                    {
                        str = string.Concat(str, ", ");
                    }
                    str = string.Concat(str, player.player.name);
                }
            }
            return str;
        }

        public string getText()
        {
            return _text;
        }

        private string PvPorPvE()
        {
            return (!Provider.isPvP ? "PvE" : "PvP");
        }
    }
}
