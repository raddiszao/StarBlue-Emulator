using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Database.Interfaces;
using System;
using System.Data;


namespace StarBlue.Communication.Packets.Incoming.Marketplace
{
    internal class RedeemOfferCreditsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            int CreditsOwed = 0;

            DataTable Table = null;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `asking_price` FROM `catalog_marketplace_offers` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND state = '2'");
                Table = dbClient.GetTable();
            }

            if (Table != null)
            {
                foreach (DataRow row in Table.Rows)
                {
                    CreditsOwed += Convert.ToInt32(row["asking_price"]);
                }

                if (CreditsOwed >= 1)
                {
                    Session.GetHabbo().Diamonds += CreditsOwed;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, CreditsOwed, 5));
                }

                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("DELETE FROM `catalog_marketplace_offers` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `state` = '2'");
                }
            }
        }
    }
}