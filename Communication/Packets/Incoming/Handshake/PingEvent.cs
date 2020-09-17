namespace StarBlue.Communication.Packets.Incoming.Handshake
{
    internal class PingEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            Session.PingCount = 0;
        }
    }
}
