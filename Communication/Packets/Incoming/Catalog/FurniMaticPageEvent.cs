using StarBlue.Communication.Packets.Outgoing;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    internal class FurniMaticPageEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            ServerPacket response = new ServerPacket(ServerPacketHeader.FurniMaticNoRoomError);
            response.WriteInteger(1);
            response.WriteInteger(0);
            Session.SendMessage(response);
        }
    }
}