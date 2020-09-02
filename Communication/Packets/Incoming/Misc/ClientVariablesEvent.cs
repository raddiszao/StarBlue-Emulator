namespace StarBlue.Communication.Packets.Incoming.Misc
{
    internal class ClientVariablesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string GordanPath = Packet.PopString();
            string ExternalVariables = Packet.PopString();
        }
    }
}
