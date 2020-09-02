using StarBlue.Communication.Packets.Outgoing.Moderation;

namespace StarBlue.Communication.Packets.Incoming.Moderation
{
    internal class OpenHelpToolEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new OpenHelpToolComposer());
        }
    }
}
