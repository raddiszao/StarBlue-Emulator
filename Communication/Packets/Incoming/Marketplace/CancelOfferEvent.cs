using StarBlue.Communication.Packets.Outgoing.Inventory.Furni;
using StarBlue.Communication.Packets.Outgoing.Marketplace;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Items;
using System;
using System.Data;


namespace StarBlue.Communication.Packets.Incoming.Marketplace
{
    internal class CancelOfferEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            DataRow Row = null;
            int OfferId = Packet.PopInt();

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT furni_id, item_id, user_id, extra_data, offer_id, state, timestamp, limited_number, limited_stack FROM catalog_marketplace_offers WHERE offer_id = @OfferId LIMIT 1");
                dbClient.AddParameter("OfferId", OfferId);
                Row = dbClient.GetRow();
            }

            if (Row == null)
            {
                Session.SendMessage(new MarketplaceCancelOfferResultComposer(OfferId, false));
                return;
            }

            if (Convert.ToInt32(Row["user_id"]) != Session.GetHabbo().Id)
            {
                Session.SendMessage(new MarketplaceCancelOfferResultComposer(OfferId, false));
                return;
            }

            if (!StarBlueServer.GetGame().GetItemManager().GetItem(Convert.ToInt32(Row["item_id"]), out ItemData Item))
            {
                Session.SendMessage(new MarketplaceCancelOfferResultComposer(OfferId, false));
                return;
            }

            //StarBlueServer.GetGame().GetCatalog().DeliverItems(Session, Item, 1, Convert.ToString(Row["extra_data"]), Convert.ToInt32(Row["limited_number"]), Convert.ToInt32(Row["limited_stack"]), Convert.ToInt32(Row["furni_id"]));

            Item GiveItem = ItemFactory.CreateSingleItem(Item, Session.GetHabbo(), Convert.ToString(Row["extra_data"]), Convert.ToString(Row["extra_data"]), Convert.ToInt32(Row["furni_id"]), Convert.ToInt32(Row["limited_number"]), Convert.ToInt32(Row["limited_stack"]));
            Session.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));
            Session.SendMessage(new FurniListUpdateComposer());

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM `catalog_marketplace_offers` WHERE `offer_id` = @OfferId AND `user_id` = @UserId LIMIT 1");
                dbClient.AddParameter("OfferId", OfferId);
                dbClient.AddParameter("UserId", Session.GetHabbo().Id);
                dbClient.RunQuery();
            }

            Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
            Session.SendMessage(new MarketplaceCancelOfferResultComposer(OfferId, true));
        }
    }
}
