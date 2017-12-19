using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using fr34kyn01535.Uconomy;
using System;

namespace UTools.Commands
{
    class CommandAuction : IRocketCommand
    {
        public string Name
        {
            get { return "auction"; }
        }

        public string Help
        {
            get { return "Allows you to auction your items from your inventory."; }
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
            get { return new List<string>() { "auction" }; }
        }

        public string Syntax
        {
            get { return "<name or id>"; }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (!UTools.Instance.Configuration.Instance.AllowAuction)
            {
                UnturnedChat.Say(caller, UTools.Instance.Translate("auction_disabled"));
                return;
            }

            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 0)
            {
                UnturnedChat.Say(player, UTools.Instance.Translate("auction_command_usage"));
                return;
            }
            if (command.Length == 1)
            {
                switch (command[0])
                {
                    case ("add"):
                        UnturnedChat.Say(player, UTools.Instance.Translate("auction_addcommand_usage"));
                        return;
                    case ("list"):
                        string Message = "";
                        string[] ItemNameAndQuality = UTools.Instance.DatabaseAuction.GetAllItemNameWithQuality();
                        string[] AuctionID = UTools.Instance.DatabaseAuction.GetAllAuctionID();
                        string[] ItemPrice = UTools.Instance.DatabaseAuction.GetAllItemPrice();
                        int count = 0;
                        for (int x = 0; x < ItemNameAndQuality.Length; x++)
                        {
                            if (x < ItemNameAndQuality.Length - 1)
                                Message += "[" + AuctionID[x] + "]: " + ItemNameAndQuality[x] + " for " + ItemPrice[x] + Uconomy.Instance.Configuration.Instance.MoneyName + ", ";
                            else
                                Message += "[" + AuctionID[x] + "]: " + ItemNameAndQuality[x] + " for " + ItemPrice[x] + Uconomy.Instance.Configuration.Instance.MoneyName;
                            count++;
                            if (count == 2)
                            {
                                UnturnedChat.Say(player, Message);
                                Message = "";
                                count = 0;
                            }
                        }
                        if (Message != "")
                            UnturnedChat.Say(player, Message);
                        break;
                    case ("buy"):
                        UnturnedChat.Say(player, UTools.Instance.Translate("auction_buycommand_usage"));
                        return;
                    case ("cancel"):
                        UnturnedChat.Say(player, UTools.Instance.Translate("auction_cancelcommand_usage"));
                        return;
                    case ("find"):
                        UnturnedChat.Say(player, UTools.Instance.Translate("auction_findcommand_usage"));
                        return;
                }
            }
            if (command.Length == 2)
            {
                int auctionid = 0;
                switch (command[0])
                {
                    case ("add"):
                        UnturnedChat.Say(player, UTools.Instance.Translate("auction_addcommand_usage2"));
                        return;
                    case ("buy"):
                        if (int.TryParse(command[1], out auctionid))
                        {
                            try
                            {
                                string[] itemInfo = UTools.Instance.DatabaseAuction.AuctionBuy(auctionid);
                                decimal balance = Uconomy.Instance.Database.GetBalance(player.Id);
                                decimal cost = 1.00m;
                                decimal.TryParse(itemInfo[2], out cost);
                                if (balance < cost)
                                {
                                    UnturnedChat.Say(player, UTools.Instance.DefaultTranslations.Translate("not_enough_currency_msg", Uconomy.Instance.Configuration.Instance.MoneyName, itemInfo[1]));
                                    return;
                                }
                                SDG.Unturned.Item item = new SDG.Unturned.Item(ushort.Parse(itemInfo[0]), 1, byte.Parse(itemInfo[3]), Convert.FromBase64String(itemInfo[6]));
                                player.Inventory.forceAddItem(item, true);
                                UTools.Instance.DatabaseAuction.DeleteAuction(command[1]);
                                decimal newbal = Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.ToString(), (cost * -1));
                                UnturnedChat.Say(player, UTools.Instance.Translate("auction_buy_msg", itemInfo[1], cost, Uconomy.Instance.Configuration.Instance.MoneyName, newbal, Uconomy.Instance.Configuration.Instance.MoneyName));
                                decimal sellernewbalance = Uconomy.Instance.Database.IncreaseBalance(itemInfo[4], (cost * 1));
                            }
                            catch
                            {
                                UnturnedChat.Say(player, UTools.Instance.Translate("auction_addcommand_idnotexist"));
                                return;
                            }

                        }
                        else
                        {
                            UnturnedChat.Say(player, UTools.Instance.Translate("auction_addcommand_usage2"));
                            return;
                        }
                        break;
                    case ("cancel"):
                        if (int.TryParse(command[1], out auctionid))
                        {
                            if (UTools.Instance.DatabaseAuction.checkAuctionExist(auctionid))
                            {
                                string OwnerID = UTools.Instance.DatabaseAuction.GetOwner(auctionid);
                                if (OwnerID.Trim() == player.Id.Trim())
                                {
                                    string[] itemInfo = UTools.Instance.DatabaseAuction.AuctionCancel(auctionid);
                                    SDG.Unturned.Item item = new SDG.Unturned.Item(ushort.Parse(itemInfo[0]), 1, byte.Parse(itemInfo[1]), Convert.FromBase64String(itemInfo[2]));
                                    player.Inventory.forceAddItem(item, true);
                                    UTools.Instance.DatabaseAuction.DeleteAuction(auctionid.ToString());
                                    UnturnedChat.Say(player, UTools.Instance.Translate("auction_cancelled", auctionid));
                                }
                                else
                                {
                                    UnturnedChat.Say(player, UTools.Instance.Translate("auction_notown"));
                                    return;
                                }
                            }
                            else
                            {
                                UnturnedChat.Say(player, UTools.Instance.Translate("auction_notexist"));
                                return;
                            }
                        }
                        else
                        {
                            UnturnedChat.Say(player, UTools.Instance.Translate("auction_notexist"));
                            return;
                        }
                        break;
                    case ("find"):
                        uint ItemID;
                        if (uint.TryParse(command[1], out ItemID))
                        {
                            string[] AuctionID = UTools.Instance.DatabaseAuction.FindItemByID(ItemID.ToString());
                            string Message = "";
                            string[] ItemNameAndQuality = UTools.Instance.DatabaseAuction.FindAllItemNameWithQualityByID(ItemID.ToString());
                            string[] ItemPrice = UTools.Instance.DatabaseAuction.FindAllItemPriceByID(ItemID.ToString());
                            int count = 0;
                            for (int x = 0; x < ItemNameAndQuality.Length; x++)
                            {
                                if (x < ItemNameAndQuality.Length - 1)
                                    Message += "[" + AuctionID[x] + "]: " + ItemNameAndQuality[x] + " for " + ItemPrice[x] + " " + Uconomy.Instance.Configuration.Instance.MoneyName + ", ";
                                else
                                    Message += "[" + AuctionID[x] + "]: " + ItemNameAndQuality[x] + " for " + ItemPrice[x] + " " + Uconomy.Instance.Configuration.Instance.MoneyName;
                                count++;
                                if (count == 2)
                                {
                                    UnturnedChat.Say(player, Message);
                                    Message = " ";
                                    count = 0;
                                }
                            }
                            if (Message != null)
                                UnturnedChat.Say(player, Message);
                            else
                            {
                                UnturnedChat.Say(player, UTools.Instance.Translate("auction_find_failed"));
                                return;
                            }
                        }
                        else
                        {
                            Asset[] array = Assets.find(EAssetType.ITEM);
                            Asset[] array2 = array;
                            ushort id;
                            string ItemName = "";
                            for (int i = 0; i < array2.Length; i++)
                            {
                                ItemAsset vAsset = (ItemAsset)array2[i];
                                if (vAsset != null && vAsset.itemName != null && vAsset.itemName.ToLower().Contains(command[1].ToLower()))
                                {
                                    id = vAsset.id;
                                    ItemName = vAsset.itemName;
                                    break;
                                }
                            }
                            if (ItemName != "")
                            {
                                string[] AuctionID = UTools.Instance.DatabaseAuction.FindItemByName(ItemName);
                                string Message = "";
                                string[] ItemNameAndQuality = UTools.Instance.DatabaseAuction.FindAllItemNameWithQualityByItemName(ItemName);
                                string[] ItemPrice = UTools.Instance.DatabaseAuction.FindAllItemPriceByItemName(ItemName);
                                int count = 0;
                                for (int x = 0; x < ItemNameAndQuality.Length; x++)
                                {
                                    if (x < ItemNameAndQuality.Length - 1)
                                        Message += "[" + AuctionID[x] + "]: " + ItemNameAndQuality[x] + " for " + ItemPrice[x] + Uconomy.Instance.Configuration.Instance.MoneyName + ", ";
                                    else
                                        Message += "[" + AuctionID[x] + "]: " + ItemNameAndQuality[x] + " for " + ItemPrice[x] + Uconomy.Instance.Configuration.Instance.MoneyName;
                                    count++;
                                    if (count == 2)
                                    {
                                        UnturnedChat.Say(player, Message);
                                        Message = "";
                                        count = 0;
                                    }
                                }
                                if (Message != "")
                                    UnturnedChat.Say(player, Message);
                                else
                                {
                                    UnturnedChat.Say(player, UTools.Instance.Translate("auction_find_failed"));
                                    return;
                                }
                            }
                            else
                            {
                                UnturnedChat.Say(player, UTools.Instance.Translate("auction_find_failed"));
                                return;
                            }
                        }
                        break;
                }
            }
            if (command.Length > 2 && player.HasPermission("auction.add"))
            {
                switch (command[0])
                {
                    case ("add"):
                        byte amt = 1;
                        ushort id;
                        string name = null;
                        ItemAsset vAsset = null;
                        string itemname = "";
                        for (int x = 1; x < command.Length - 1; x++)
                        {
                            itemname += command[x] + " ";
                        }
                        itemname = itemname.Trim();
                        if (!ushort.TryParse(itemname, out id))
                        {
                            Asset[] array = Assets.find(EAssetType.ITEM);
                            Asset[] array2 = array;
                            for (int i = 0; i < array2.Length; i++)
                            {
                                vAsset = (ItemAsset)array2[i];
                                if (vAsset != null && vAsset.itemName != null && vAsset.itemName.ToLower().Contains(itemname.ToLower()))
                                {
                                    id = vAsset.id;
                                    name = vAsset.itemName;
                                    if (name == "Hell's Fury") { name = "Hells Fury"; }
                                    break;
                                }
                            }
                        }
                        if (name == null && id == 0)
                        {
                            UnturnedChat.Say(player, UTools.Instance.Translate("could_not_find", itemname));
                            return;
                        }
                        else if (name == null && id != 0)
                        {
                            try
                            {
                                vAsset = (ItemAsset)Assets.find(EAssetType.ITEM, id);
                                name = vAsset.itemName;
                                if (name == "Hell's Fury") { name = "Hells Fury"; }
                            }
                            catch
                            {
                                UnturnedChat.Say(player, UTools.Instance.Translate("item_invalid"));
                                return;
                            }
                        }
                        if (player.Inventory.has(id) == null)
                        {
                            UnturnedChat.Say(player, UTools.Instance.Translate("not_have_item_auction", name));
                            return;
                        }
                        List<InventorySearch> list = player.Inventory.search(id, true, true);
                        if (vAsset.amount > 1)
                        {
                            UnturnedChat.Say(player, UTools.Instance.Translate("auction_item_mag_ammo", name));
                            return;
                        }
                        decimal price = 0.00m;
                        if (UTools.Instance.Configuration.Instance.EnableShopPriceCheck)
                        {
                            price = ZaupShop.ZaupShop.Instance.ShopDB.GetItemCost(id);
                            if (price <= 0.00m)
                            {
                                UnturnedChat.Say(player, UTools.Instance.Translate("auction_item_notinshop", name));
                                price = 0.00m;
                            }
                        }


                        if (ZaupShop.ZaupShop.Instance.ShopDB.GetItemCost(id) == 0 || (ZaupShop.ZaupShop.Instance.ShopDB.GetItemCost(id) != 0 && Convert.ToDecimal(command[command.Length - 1]) >= (ZaupShop.ZaupShop.Instance.ShopDB.GetItemCost(id) * 0.3m) && Convert.ToDecimal(command[command.Length - 1]) <= (ZaupShop.ZaupShop.Instance.ShopDB.GetItemCost(id) * 1.5m)))
                        {
                            int quality = 100;
                            string metadata = null;
                            switch (vAsset.amount)
                            {
                                case 1:
                                    while (amt > 0)
                                    {
                                        try
                                        {
                                            if (player.Player.equipment.checkSelection(list[0].page, list[0].jar.x, list[0].jar.y))
                                            {
                                                player.Player.equipment.dequip();
                                            }
                                        }
                                        catch
                                        {
                                            UnturnedChat.Say(player, UTools.Instance.Translate("auction_unequip_item", name));
                                            return;
                                        }
                                        quality = list[0].jar.item.durability;
                                        metadata = (list[0].jar.item.metadata != null ? Convert.ToBase64String(list[0].jar.item.metadata) : string.Empty);
                                        player.Inventory.removeItem(list[0].page, player.Inventory.getIndex(list[0].page, list[0].jar.x, list[0].jar.y));
                                        list.RemoveAt(0);
                                        amt--;
                                    }
                                    break;
                                default:
                                    UnturnedChat.Say(player, UTools.Instance.Translate("auction_item_mag_ammo", name));
                                    return;
                            }
                            decimal SetPrice;
                            if (!decimal.TryParse(command[command.Length - 1], out SetPrice))
                                SetPrice = price;
                            if (UTools.Instance.DatabaseAuction.AddAuctionItem(UTools.Instance.DatabaseAuction.GetLastAuctionNo(), id.ToString(), name, SetPrice, price, quality, metadata, player.Id, player.DisplayName.ToString()))
                                UnturnedChat.Say(player, UTools.Instance.Translate("auction_item_succes", name, SetPrice, Uconomy.Instance.Configuration.Instance.MoneyName));
                            else
                                UnturnedChat.Say(player, UTools.Instance.Translate("auction_item_failed"));
                        }
                        else
                        {
                            UnturnedChat.Say(player, UTools.Instance.Translate("auction_price_low_high", Math.Floor((ZaupShop.ZaupShop.Instance.ShopDB.GetItemCost(id) * 0.3m)), Math.Floor((ZaupShop.ZaupShop.Instance.ShopDB.GetItemCost(id) * 1.5m))));
                        }
                        break;
                }
            }
            if (command.Length > 2 && !player.HasPermission("auction.add"))
            {
                UnturnedChat.Say(player, UTools.Instance.Translate("auction_add_no_perm"));
            }
        }

    }

}
