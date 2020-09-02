using StarBlue.Communication.Packets.Outgoing.Misc;

namespace StarBlue.Communication.Packets.Incoming.Misc
{
    internal class LatencyTestEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new LatencyTestComposer(Packet.PopInt()));
        }
    }
}
