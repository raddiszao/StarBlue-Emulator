
using StarBlue.Communication.Packets.Outgoing.Avatar;

namespace StarBlue.Communication.Packets.Incoming.Avatar
{
    internal class GetWardrobeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new WardrobeComposer(Session.GetHabbo().Id));
        }
    }
}
