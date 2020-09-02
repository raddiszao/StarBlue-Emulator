using StarBlue.HabboHotel.WebClient;

namespace StarBlue.Communication.Packets.Incoming.WebSocket
{
    class SSOWebTicketEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            if (Session == null)
                return;

            string SSOTicket = Packet.PopString();
            Session.TryAuthenticate(SSOTicket);
        }
    }
}
