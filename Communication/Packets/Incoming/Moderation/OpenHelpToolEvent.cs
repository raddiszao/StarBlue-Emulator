using StarBlue.Communication.Packets.Outgoing.Moderation;

namespace StarBlue.Communication.Packets.Incoming.Moderation
{
    internal class OpenHelpToolEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            Session.SendMessage(new OpenHelpToolComposer());
        }
    }
}
