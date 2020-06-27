using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Inventory.Furni;
using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Communication.Packets.Outgoing.Rooms.Camera;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Camera
{
    public class PurchaseCameraPictureMessageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket paket)
        {
            if (!Session.GetHabbo().lastPhotoPreview.Contains("-"))
            {
                return;
            }

            if (Session.GetHabbo().Duckets < 10)
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("definitions", "Necessita ter pelo menos 10 duckets para comprar uma foto.", ""));
                return;
            }

            if (Session.GetHabbo().Credits < 100)
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("definitions", "Necessita ter pelo menos 100 créditos para comprar uma foto.", ""));
                return;
            }

            string roomId = Session.GetHabbo().lastPhotoPreview.Split('-')[0];
            string timestamp = Session.GetHabbo().lastPhotoPreview.Split('-')[1];
            string md5image = URLPost.GetMD5(Session.GetHabbo().lastPhotoPreview);
            if (!StarBlueServer.GetGame().GetItemManager().GetItem(1100001495, out ItemData Item))
            {
                return;
            }

            if (Item == null)
            {
                return;
            }

            Item photoPoster = ItemFactory.CreateSingleItemNullable(Item, Session.GetHabbo(), "{\"timestamp\": " + timestamp + ", \"id\":\"" + md5image + "\"}", "");

            if (photoPoster != null)
            {
                Session.GetHabbo().GetInventoryComponent().TryAddItem(photoPoster);

                Session.GetHabbo().Credits -= 100;
                Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));

                Session.GetHabbo().Duckets -= 10;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));

                Session.SendMessage(new FurniListAddComposer(photoPoster));
                Session.SendMessage(new FurniListUpdateComposer());
                StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CameraPhotoCount", 1);
            }

            Session.SendMessage(new CameraFinishPurchaseMessageComposer());

            Session.GetHabbo().GetInventoryComponent().UpdateItems(false);


            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT IGNORE INTO items_camera VALUES (@id, @habboid, @creator_name, @roomid, @timestamp1, @timestamp2)");
                dbClient.AddParameter("habboid", Session.GetHabbo().Id);
                dbClient.AddParameter("id", md5image);
                dbClient.AddParameter("creator_name", Session.GetHabbo().Username);
                dbClient.AddParameter("roomid", roomId);
                dbClient.AddParameter("timestamp1", timestamp);
                dbClient.AddParameter("timestamp2", StarBlueServer.GetUnixTimestamp());
                dbClient.RunQuery();
            }


        }
    }
}
