using StarBlue.Communication.Packets.Outgoing.Catalog;
using StarBlue.Communication.Packets.Outgoing.Inventory.Furni;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Items;
using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator
{
    internal class ForceFurniMaticBoxCommand : IChatCommand
    {
        public string PermissionRequired => "user_16";

        public string Parameters => "";

        public string Description => "TEST Command - Rewards";

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {

            int Boxes = int.Parse(Params[1]);

            for (int i = 1; i <= Boxes; i++)
            {
                Catalog.FurniMatic.FurniMaticRewards reward = StarBlueServer.GetGame().GetFurniMaticRewardsMnager().GetRandomReward();
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
}
