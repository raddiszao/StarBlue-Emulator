namespace StarBlue.Communication.Packets.Incoming.Users
{
    internal class SetUsernameEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string Username = Packet.PopString();
        }
    }
}
