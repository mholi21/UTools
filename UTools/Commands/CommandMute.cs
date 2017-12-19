using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Rocket.Unturned.Commands;
using Steamworks;
using System.Collections.Generic;
using Rocket.Core.Steam;
using UnityEngine;

namespace UTools.Commands
{
    public class CommandMute : IRocketCommand
    {
        public string Help
        {
            get { return "Mute a player"; }
        }

        public string Name
        {
            get { return "mute"; }
        }

        public string Syntax
        {
            get { return "<player> [reason] [duration]"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Both; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "mute" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            try
            {
                if (command.Length == 0 || command.Length > 3)
                {
                    UnturnedChat.Say(caller, "Invalid input. /mute <name> Reason <time>", UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DutyMsgColor, Color.red));
                    return;
                }

                bool isOnline = false;
                CSteamID steamid;
                string charactername = null;
                string adminId = "Console";
                string adminName = "Console";
                string reason = "";
                int duration = 0;

                UnturnedPlayer otherPlayer = UnturnedPlayer.FromName(command[0]);
                ulong? otherPlayerID = command.GetCSteamIDParameter(0);

                if (otherPlayer == null || otherPlayer.CSteamID.ToString() == "0" || caller != null && otherPlayer.CSteamID.ToString() == caller.Id)
                {
                    KeyValuePair<CSteamID, string> player = UTools.GetPlayer(command[0]);
                    if (player.Key.ToString() != "0")
                    {
                        steamid = player.Key;
                        charactername = player.Value;
                    }
                    else
                    {
                        if (otherPlayerID != null)
                        {
                            steamid = new CSteamID(otherPlayerID.Value);
                            Profile playerProfile = new Profile(otherPlayerID.Value);
                            charactername = playerProfile.SteamID;
                        }
                        else
                        {
                            UnturnedChat.Say(caller, UTools.Instance.Translate("tpa_player_not_found"), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DutyMsgColor, Color.red));
                            return;
                        }
                    }
                }
                else
                {
                    isOnline = true;
                    steamid = otherPlayer.CSteamID;
                    charactername = otherPlayer.CharacterName;
                    if (otherPlayer.HasPermission("mute.immune"))
                    {
                        UnturnedChat.Say(caller, UTools.Instance.Translate("mute_immune", otherPlayer.CharacterName));
                        return;
                    }
                }

                if (caller != null)
                {
                    if (caller.ToString() != "Rocket.API.ConsolePlayer")
                    {
                        adminId = caller.ToString();
                    }
                    adminName = caller.DisplayName;
                }

                if (adminId.ToString() == steamid.ToString())
                {
                    UnturnedChat.Say(caller, "You cannot mute yourself!", UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DutyMsgColor, Color.red));
                    return;
                }
                if (UTools.Instance.Database.convertTimeToSeconds(UTools.Instance.Configuration.Instance.DefaultMuteDuration) != 0)
                    int.TryParse(UTools.Instance.Database.convertTimeToSeconds(UTools.Instance.Configuration.Instance.DefaultMuteDuration).ToString(), out duration);

                if (command.Length == 3)
                {
                    if (int.TryParse(UTools.Instance.Database.convertTimeToSeconds(command[2]).ToString(), out duration))
                    {
                        reason = command[1];
                    }
                    else
                    {
                        UnturnedChat.Say(caller, UTools.Instance.Translate("Invalid use! /mute <name> Reason <time>"), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DutyMsgColor, Color.red));
                        return;
                    }
                }
                else if (command.Length == 2)
                {

                    if (UTools.Instance.Database.convertTimeToSeconds(command[1]) > 0)
                    {
                        int.TryParse(UTools.Instance.Database.convertTimeToSeconds(command[1]).ToString(), out duration);
                    }
                    else
                    {
                        reason = command[1];
                    }
                }
                if (UTools.Instance.Database.IsChatBanned(steamid.ToString()) != null)
                {
                    UnturnedChat.Say(caller, "Player already muted", UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DutyMsgColor, Color.red));
                    return;
                }

                UTools.Instance.Database.ChatBanPlayer(charactername, steamid.ToString(), adminId, reason, duration);
                UnturnedChat.Say(UTools.Instance.Translate("mute_public", charactername), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DutyMsgColor, Color.red));
                UnturnedChat.Say(steamid, UTools.Instance.Translate("mute_private", (duration / 60)), UnturnedChat.GetColorFromName(UTools.Instance.Configuration.Instance.DutyMsgColor, Color.red));
            }
            catch (System.Exception e)
            {
                Rocket.Core.Logging.Logger.LogException(e);
            }
        }
    }
}
