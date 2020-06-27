using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.GameCenter
{
    class UnknownGameCenterEvent5 : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int pop1 = Packet.PopInt();
            int pop2 = Packet.PopInt();
            int pop3 = Packet.PopInt();
            int pop4 = Packet.PopInt();
            int pop5 = Packet.PopInt();

        }
    }
}
