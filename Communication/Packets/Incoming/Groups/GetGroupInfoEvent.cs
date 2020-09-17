using StarBlue.Communication.Packets.Outgoing.Groups;
using StarBlue.HabboHotel.Groups;

namespace StarBlue.Communication.Packets.Incoming.Groups
{
    internal class GetGroupInfoEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            int GroupId = Packet.PopInt();
            bool NewWindow = Packet.PopBoolean();

            if (!StarBlueServer.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            Session.SendMessage(new GroupInfoComposer(Group, Session, NewWindow));
        }
    }
}
