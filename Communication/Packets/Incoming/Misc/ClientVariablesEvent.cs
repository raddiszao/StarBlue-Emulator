namespace StarBlue.Communication.Packets.Incoming.Misc
{
    internal class ClientVariablesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            int idkYet = Packet.PopInt();
            string GordanPath = Packet.PopString();
            string ExternalVariables = Packet.PopString();
        }
    }
}
