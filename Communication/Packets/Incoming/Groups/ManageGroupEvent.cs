
using StarBlue.Communication.Packets.Outgoing.Groups;
using StarBlue.HabboHotel.Groups;

namespace StarBlue.Communication.Packets.Incoming.Groups
{
    internal class ManageGroupEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();

            if (!StarBlueServer.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if (Group.CreatorId != Session.GetHabbo().Id && !Session.GetHabbo().GetPermissions().HasRight("group_management_override"))
            {
                return;
            }

            Session.SendMessage(new ManageGroupComposer(Group));
        }
    }
}
