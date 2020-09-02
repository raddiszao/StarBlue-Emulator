using StarBlue.Communication.Packets.Outgoing.Inventory.Furni;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    public class RedeemHCGiftEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string item = Packet.PopString();

            ItemData gift = StarBlueServer.GetGame().GetItemManager().GetItemByName(item);

            Session.GetHabbo().GetInventoryComponent().AddNewItem(0, gift.Id, "", 0, true, false, 0, 0);
            Session.SendMessage(new FurniListUpdateComposer());
            Session.GetHabbo().GetInventoryComponent().UpdateItems(true);

            Session.GetHabbo().GetStats().vipGifts--;

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE `user_stats` SET `vip_gifts` = '" + Session.GetHabbo().GetStats().vipGifts + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }
        }
    }
}