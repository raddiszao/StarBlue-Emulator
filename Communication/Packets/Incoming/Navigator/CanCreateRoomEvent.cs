
using StarBlue.Communication.Packets.Outgoing.Navigator;

namespace StarBlue.Communication.Packets.Incoming.Navigator
{
    internal class CanCreateRoomEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            Session.SendMessage(new CanCreateRoomComposer(false, 150));
        }
    }
}
