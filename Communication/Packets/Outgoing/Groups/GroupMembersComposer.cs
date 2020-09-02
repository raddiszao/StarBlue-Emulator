using StarBlue.HabboHotel.Groups;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class GroupMembersComposer : ServerPacket
    {
        public GroupMembersComposer(Group Group, ICollection<GroupMember> Members, int MembersCount, int Page, bool Admin, int ReqType, string SearchVal)
            : base(ServerPacketHeader.GroupMembersMessageComposer)
        {
            base.WriteInteger(Group.Id);
            base.WriteString(Group.Name);
            base.WriteInteger(Group.RoomId);
            base.WriteString(Group.Badge);
            base.WriteInteger(MembersCount);

            base.WriteInteger(Members.Count);
            if (MembersCount > 0)
            {
                foreach (GroupMember Data in Members)
                {
                    base.WriteInteger(Group.CreatorId == Data.Id ? 0 : Group.IsAdmin(Data.Id) ? 1 : Group.IsMember(Data.Id) ? 2 : 3);
                    base.WriteInteger(Data.Id);
                    base.WriteString(Data.Username);
                    base.WriteString(Data.Look);
                    base.WriteString(Data.JoinedTime);
                }
            }

            base.WriteBoolean(Admin);
            base.WriteInteger(14);
            base.WriteInteger(Page);
            base.WriteInteger(ReqType);
            base.WriteString(SearchVal);
        }
    }
}