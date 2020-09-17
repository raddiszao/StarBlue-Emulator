
using StarBlue.Communication.Packets.Outgoing.Marketplace;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Catalog.Utilities;
using StarBlue.HabboHotel.Items;


namespace StarBlue.Communication.Packets.Incoming.Marketplace
{
    internal class MakeOfferEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            int SellingPrice = Packet.PopInt();
            int ComissionPrice = Packet.PopInt();
            int ItemId = Packet.PopInt();

            Item Item = Session.GetHabbo().GetInventoryComponent().GetItem(ItemId);
            if (Item == null)
            {
                Session.SendMessage(new MarketplaceMakeOfferResultComposer(0));
                return;
            }

            if (!ItemUtility.IsRare(Item))
            {
                Session.SendNotification("Sinto muito, os raros não estão permitidos em nosso catálogo!");
                return;
            }

            if (SellingPrice > 70000000 || SellingPrice == 0)
            {
                Session.SendMessage(new MarketplaceMakeOfferResultComposer(0));
                return;
            }

            int Comission = StarBlueServer.GetGame().GetCatalog().GetMarketplace().CalculateComissionPrice(SellingPrice);
            int TotalPrice = SellingPrice + Comission;
            int ItemType = 1;
            if (Item.GetBaseItem().Type == 'i')
            {
                ItemType++;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `catalog_marketplace_offers` (`furni_id`,`item_id`,`user_id`,`asking_price`,total_price,public_name,sprite_id,item_type,timestamp,extra_data,limited_number,limited_stack) VALUES ('" + ItemId + "','" + Item.BaseItem + "','" + Session.GetHabbo().Id + "','" + SellingPrice + "','" + TotalPrice + "',@public_name,'" + Item.GetBaseItem().SpriteId + "','" + ItemType + "','" + StarBlueServer.GetUnixTimestamp() + "',@extra_data, '" + Item.LimitedNo + "', '" + Item.LimitedTot + "')");
                dbClient.AddParameter("public_name", Item.GetBaseItem().PublicName);
                dbClient.AddParameter("extra_data", Item.ExtraData);
                dbClient.RunQuery();

                dbClient.RunFastQuery("DELETE FROM `items` WHERE `id` = '" + ItemId + "' AND `user_id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            Session.GetHabbo().GetInventoryComponent().RemoveItem(ItemId);
            Session.SendMessage(new MarketplaceMakeOfferResultComposer(1));
        }
    }
}