
using StarBlue.Communication.Packets.Outgoing.Help;

namespace StarBlue.Communication.Packets.Incoming.Help
{
    internal class SendBullyReportEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            Session.SendMessage(new SendBullyReportComposer());
        }
    }
}
