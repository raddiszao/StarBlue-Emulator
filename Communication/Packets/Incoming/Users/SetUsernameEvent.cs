namespace StarBlue.Communication.Packets.Incoming.Users
{
    internal class SetUsernameEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            string Username = Packet.PopString();
        }
    }
}
