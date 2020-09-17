using StarBlue.Communication.Packets.Outgoing.Catalog;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    internal class FurniMaticPageEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            Session.SendMessage(new FurniMaticNoRoomError());
        }
    }
}