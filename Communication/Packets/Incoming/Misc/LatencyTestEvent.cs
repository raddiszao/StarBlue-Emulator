using StarBlue.Communication.Packets.Outgoing.Misc;

namespace StarBlue.Communication.Packets.Incoming.Misc
{
    internal class LatencyTestEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            Session.SendMessage(new PingMessageComposer());
        }
    }
}
