using MySql.Data.MySqlClient;
using Rocket.Core.Logging;
using System;
using System.Text.RegularExpressions;

namespace UTools.Database
{
    public class DatabaseManagerMute
    {
        internal DatabaseManagerMute()
        {
            new I18N.West.CP1250();
            CheckSchema();
        }

        private MySqlConnection createConnection()
        {
            MySqlConnection connection = null;
            try
            {
                if (UTools.Instance.Configuration.Instance.DatabasePort == 0) UTools.Instance.Configuration.Instance.DatabasePort = 3306;
                connection = new MySqlConnection(string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", UTools.Instance.Configuration.Instance.DatabaseAddress, UTools.Instance.Configuration.Instance.DatabaseName, UTools.Instance.Configuration.Instance.DatabaseUsername, UTools.Instance.Configuration.Instance.DatabasePassword, UTools.Instance.Configuration.Instance.DatabasePort));
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
            return connection;
        }

        internal void CheckSchema()
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "show tables like '" + UTools.Instance.Configuration.Instance.DatabaseChatbanTableName + "'";
                connection.Open();
                object test = command.ExecuteScalar();

                if (test == null)
                {
                    command.CommandText = "CREATE TABLE `" + UTools.Instance.Configuration.Instance.DatabaseChatbanTableName + "` (`steamId` varchar(32) NOT NULL UNIQUE,`admin` varchar(32) NOT NULL,`banMessage` varchar(512) DEFAULT NULL,`charactername` varchar(255) DEFAULT NULL,`banDuration` int NULL,`banTime` timestamp NULL ON UPDATE CURRENT_TIMESTAMP,PRIMARY KEY (`steamId`));";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }

        public int convertTimeToSeconds(string time)
        {
            int ret = 0;
            int tm = 0;
            if (time == null)
                return 0;

            if (time.Contains("d"))
            {
                if (int.TryParse(time.Replace("d", ""), out tm))
                    ret = (tm * 86400);
            }
            else if (time.Contains("h"))
            {
                if (int.TryParse(time.Replace("h", ""), out tm))
                    ret = (tm * 3600);
            }
            else if (time.Contains("m"))
            {
                if (int.TryParse(time.Replace("m", ""), out tm))
                    ret = (tm * 60);
            }
            else if (time.Contains("s"))
            {
                if (int.TryParse(time.Replace("s", ""), out tm))
                    ret = tm;
            }
            else
            {
                if (int.TryParse(time, out tm))
                {
                    ret = tm;
                }
                else
                {
                    ret = 0;
                }
            }
            return ret;
        }

        public class UnbanResult
        {
            public ulong Id;
            public string Name;
        }

        public string IsChatBanned(string steamId)
        {
            string output = null;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `banMessage` from `" + UTools.Instance.Configuration.Instance.DatabaseChatbanTableName + "` where `steamId` = '" + steamId + "' and (banDuration is null or ((banDuration + UNIX_TIMESTAMP(banTime)) > UNIX_TIMESTAMP()));";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null) output = result.ToString();
                connection.Close();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
            return output;
        }

        public void ChatBanPlayer(string characterName, string steamid, string admin, string banMessage, int duration)
        {
            try
            {
                characterName = Regex.Replace(characterName, @"\p{Cs}", "");
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                if (banMessage == null) banMessage = "";
                command.Parameters.AddWithValue("@csteamid", steamid);
                command.Parameters.AddWithValue("@admin", admin);
                command.Parameters.AddWithValue("@charactername", characterName);
                command.Parameters.AddWithValue("@banMessage", banMessage);
                if (duration == 0)
                {
                    command.Parameters.AddWithValue("@banDuration", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@banDuration", duration);
                }
                command.CommandText = "insert into `" + UTools.Instance.Configuration.Instance.DatabaseChatbanTableName + "` (`steamId`,`admin`,`banMessage`,`charactername`,`banTime`,`banDuration`) values(@csteamid,@admin,@banMessage,@charactername,now(),@banDuration) on duplicate key update `banDuration` = @banDuration, `banTime` = now();";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }

        public UnbanResult UnChatbanPlayer(string player)
        {
            try
            {
                MySqlConnection connection = createConnection();

                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@player", "%" + player + "%");
                command.CommandText = "select steamId,charactername from `" + UTools.Instance.Configuration.Instance.DatabaseChatbanTableName + "` where `steamId` like @player or `charactername` like @player limit 1;";
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    ulong steamId = reader.GetUInt64(0);
                    string charactername = reader.GetString(1);
                    connection.Close();
                    command = connection.CreateCommand();
                    command.Parameters.AddWithValue("@steamId", steamId);
                    command.CommandText = "delete from `" + UTools.Instance.Configuration.Instance.DatabaseChatbanTableName + "` where `steamId` = @steamId;";
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    return new UnbanResult() { Id = steamId, Name = charactername };
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
            return null;
        }

    }
}
