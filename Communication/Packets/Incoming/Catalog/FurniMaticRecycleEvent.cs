using StarBlue.Communication.Packets.Outgoing.Catalog;
using StarBlue.Communication.Packets.Outgoing.Inventory.Furni;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Items;
using System;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    internal class FurniMaticRecycleEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            int itemsCount = Packet.PopInt();
            for (int i = 0; i < itemsCount; i++)
            {
                int itemId = Packet.PopInt();
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("DELETE FROM `items` WHERE `id` = '" + itemId + "' AND `user_id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                }

                Session.GetHabbo().GetInventoryComponent().RemoveItem(itemId);
            }

            HabboHotel.Catalog.FurniMatic.FurniMaticRewards reward = StarBlueServer.GetGame().GetFurniMaticRewardsMnager().GetRandomReward();
            if (reward == null)
            {
                return;
            }

            int rewardId;
            int furniMaticBoxId = 4692;
            StarBlueServer.GetGame().GetItemManager().GetItem(furniMaticBoxId, out ItemData data);
            string maticData = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `items` (`base_item`,`user_id`,`extra_data`) VALUES ('" + data.Id + "', '" + Session.GetHabbo().Id + "', @extra_data)");
                dbClient.AddParameter("extra_data", maticData);
                rewardId = Convert.ToInt32(dbClient.InsertQuery());
                dbClient.RunFastQuery("INSERT INTO `user_presents` (`item_id`,`base_id`,`extra_data`) VALUES ('" + rewardId + "', '" + reward.GetBaseItem().Id + "', '')");
                dbClient.RunFastQuery("DELETE FROM `items` WHERE `id` = " + rewardId + " LIMIT 1;");
            }

            Item GiveItem = ItemFactory.CreateGiftItem(data, Session.GetHabbo(), maticData, maticData, rewardId, 0, 0);
            if (GiveItem != null)
            {
                Session.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);
                Session.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));
                Session.SendMessage(new PurchaseOKComposer());
                Session.SendMessage(new FurniListAddComposer(GiveItem));
                Session.SendMessage(new FurniListUpdateComposer());
            }

            Session.SendMessage(new FurniMaticReceiveItem(GiveItem.Id));
        }
    }
}