namespace StarBlue.Communication.Packets.Incoming.Misc
{
    internal class DisconnectEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            Session.Disconnect();
        }
    }
}
