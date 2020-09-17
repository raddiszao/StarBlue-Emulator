using StarBlue.Communication.Packets.Outgoing.Groups;
using StarBlue.HabboHotel.Groups;

namespace StarBlue.Communication.Packets.Incoming.Groups
{
    internal class DeclineGroupMembershipEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            if (!StarBlueServer.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if (Session.GetHabbo().Id != Group.CreatorId && !Group.IsAdmin(Session.GetHabbo().Id))
            {
                return;
            }

            if (!Group.HasRequest(UserId))
            {
                return;
            }

            Group.HandleRequest(UserId, false);
            Session.SendMessage(new UnknownGroupComposer(Group.Id, UserId));
        }
    }
}