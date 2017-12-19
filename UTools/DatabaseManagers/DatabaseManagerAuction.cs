using MySql.Data.MySqlClient;
using Rocket.Core.Logging;
using System;
using System.Data;

namespace UTools.Database
{
    public class DatabaseManagerAuction
    {
        internal DatabaseManagerAuction()
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
                connection = new MySqlConnection(string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4}; Convert Zero Datetime=True;", UTools.Instance.Configuration.Instance.DatabaseAddress, UTools.Instance.Configuration.Instance.DatabaseName, UTools.Instance.Configuration.Instance.DatabaseUsername, UTools.Instance.Configuration.Instance.DatabasePassword, UTools.Instance.Configuration.Instance.DatabasePort));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return connection;
        }

        internal void CheckSchema()
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                if (UTools.Instance.Configuration.Instance.AllowAuction)
                {
                    command.CommandText = "show tables like '" + UTools.Instance.Configuration.Instance.DatabaseAuction + "'";
                    connection.Open();
                    object test = command.ExecuteScalar();
                    if (test == null)
                    {
                        command.CommandText = "CREATE TABLE `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "` (`id` int(6) NOT NULL,`itemid` int(7) NOT NULL,`ItemName` varchar(56) NOT NULL,`Price` decimal(15,2) NOT NULL DEFAULT '20.00',`ShopPrice` decimal(15,2) NOT NULL DEFAULT '0.00', `Quality` int(3) NOT NULL, `SellerID` varchar(20) NOT NULL , `SellerName` varchar(20) NOT NULL, `metadata` varchar(255) NOT NULL, PRIMARY KEY (`id`))";
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }
        }

        public void DeleteAuction(string auctionID)
        {
            try
            {
                MySqlConnection Connection = createConnection();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "delete from `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "` where `id` = '" + auctionID + "'";
                Connection.Open();
                object obj = Command.ExecuteNonQuery();
                Connection.Close();
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }
        }

        public string GetOwner(int auctionID)
        {
            string ID = "";
            try
            {
                MySqlConnection Connection = createConnection();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "select `SellerID` from `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "` where `id` = '" + auctionID + "'";
                Connection.Open();
                object obj = Command.ExecuteScalar();
                if (obj != null)
                    ID = obj.ToString().Trim();
                Connection.Close();
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }
            return ID;
        }

        public bool AddAuctionItem(int id, string itemid, string itemname, decimal price, decimal shopprice, int quality, string metadata, string sellerID, string sellerName)
        {
            bool added = false;
            try
            {
                MySqlConnection Connection = createConnection();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "Insert into `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "` (`id`,`itemid`,`ItemName`,`Price`,`ShopPrice`,`Quality`,`SellerID`,`SellerName`,`metadata`) Values('" + id.ToString() + "', '" + itemid + "', '" + itemname + "', '" + price + "', '" + shopprice + "', '" + quality + "', '" + sellerID + "', '" + sellerName + "', '" + metadata + "')";
                Connection.Open();
                Command.ExecuteNonQuery();
                Connection.Close();
                added = true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return added;
        }

        public bool checkAuctionExist(int id)
        {
            bool exist = false;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `id` from `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "` where `id` = " + id + "";
                connection.Open();
                object obj = command.ExecuteScalar();
                connection.Close();
                if (obj != null)
                    exist = true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return exist;
        }

        public int GetLastAuctionNo()
        {
            DataTable dt = new DataTable();
            int AuctionNo = 0;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `id` from `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "`";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    if (dt.Rows[i].ItemArray[0].ToString() != i.ToString())
                    {
                        AuctionNo = i;
                        return AuctionNo;
                    }
                }
                AuctionNo = dt.Rows.Count;

            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return AuctionNo;
        }

        public string[] GetAllItemNameWithQuality()
        {
            DataTable dt = new DataTable();
            DataTable quality = new DataTable();
            string[] ItemName = { };
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `ItemName` from `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "`";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                command.CommandText = "select `Quality` from `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "`";
                adapter = new MySqlDataAdapter(command);
                adapter.Fill(quality);
                connection.Close();
                ItemName = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    ItemName[i] = dt.Rows[i].ItemArray[0].ToString() + "(" + quality.Rows[i].ItemArray[0].ToString() + ")";
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemName;
        }

        public string[] FindAllItemNameWithQualityByID(string ItemID)
        {
            DataTable dt = new DataTable();
            DataTable quality = new DataTable();
            string[] ItemName = { };
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `ItemName` from `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "` where `itemid` = " + ItemID + "";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                command.CommandText = "select `Quality` from `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "` where `itemid` = " + ItemID + "";
                adapter = new MySqlDataAdapter(command);
                adapter.Fill(quality);
                connection.Close();
                ItemName = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    ItemName[i] = dt.Rows[i].ItemArray[0].ToString() + "(" + quality.Rows[i].ItemArray[0].ToString() + ")";
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemName;
        }

        public string[] FindAllItemNameWithQualityByItemName(string Itemname)
        {
            DataTable dt = new DataTable();
            DataTable quality = new DataTable();
            string[] ItemName = { };
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `ItemName` from `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "` where `ItemName` = " + Itemname + "";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                command.CommandText = "select `Quality` from `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "` where `ItemName` = " + Itemname + "";
                adapter = new MySqlDataAdapter(command);
                adapter.Fill(quality);
                connection.Close();
                ItemName = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    ItemName[i] = dt.Rows[i].ItemArray[0].ToString() + "(" + quality.Rows[i].ItemArray[0].ToString() + ")";
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemName;
        }

        public string[] GetAllAuctionID()
        {
            DataTable dt = new DataTable();
            string[] AuctionID = { };
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `id` from `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "`";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                AuctionID = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    AuctionID[i] = dt.Rows[i].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return AuctionID;
        }

        public string[] FindAllItemPriceByID(string ItemID)
        {
            DataTable dt = new DataTable();
            string[] ItemPrice = { };
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `Price` from `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "` where `itemid` = " + ItemID + "";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                ItemPrice = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    ItemPrice[i] = dt.Rows[i].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemPrice;
        }

        public string[] FindAllItemPriceByItemName(string ItemName)
        {
            DataTable dt = new DataTable();
            string[] ItemPrice = { };
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `Price` from `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "` where `ItemName` = " + ItemName + "";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                ItemPrice = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    ItemPrice[i] = dt.Rows[i].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemPrice;
        }

        public string[] GetAllItemPrice()
        {
            DataTable dt = new DataTable();
            string[] ItemPrice = { };
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `Price` from `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "`";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                ItemPrice = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    ItemPrice[i] = dt.Rows[i].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemPrice;
        }

        public string[] AuctionBuy(int auctionID)
        {
            DataTable dt = new DataTable();
            string[] ItemInfo = new string[7];
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `itemid`,`ItemName`, `Price`, `Quality`, `SellerID`, `SellerName` ,`metadata`  from `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "` where `id` = " + auctionID.ToString() + "";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                for (int i = 0; i < (dt.Columns.Count); i++)
                {
                    ItemInfo[i] = dt.Rows[0].ItemArray[i].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemInfo;
        }

        public string[] AuctionCancel(int auctionID)
        {
            DataTable dt = new DataTable();
            string[] ItemInfo = new string[5];
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `itemid`, `Quality`, `metadata`  from `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "` where `id` = " + auctionID.ToString() + "";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                for (int i = 0; i < (dt.Columns.Count); i++)
                {
                    ItemInfo[i] = dt.Rows[0].ItemArray[i].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemInfo;
        }

        public string[] FindItemByID(string ItemID)
        {
            DataTable dt = new DataTable();
            string[] AuctionID = { };
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `id` from `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "` where `itemid` = '" + ItemID + "'";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                AuctionID = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    AuctionID[i] = dt.Rows[i].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return AuctionID;
        }

        public string[] FindItemByName(string ItemName)
        {
            DataTable dt = new DataTable();
            string[] AuctionID = { };
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `id` from `" + UTools.Instance.Configuration.Instance.DatabaseAuction + "` where `ItemName` = '" + ItemName + "'";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                AuctionID = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    AuctionID[i] = dt.Rows[i].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return AuctionID;
        }

    }
}
