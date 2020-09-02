
using StarBlue.Communication.Packets.Outgoing.Catalog;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    internal class GetGroupFurniConfigEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new GroupFurniConfigComposer(StarBlueServer.GetGame().GetGroupManager().GetGroupsForUser(Session.GetHabbo().Id)));
        }
    }
}
