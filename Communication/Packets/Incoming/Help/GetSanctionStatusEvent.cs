
using StarBlue.Communication.Packets.Outgoing.Help;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Help
{
    internal class GetSanctionStatusEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            Session.SendMessage(new SanctionStatusComposer());
        }
    }
}
