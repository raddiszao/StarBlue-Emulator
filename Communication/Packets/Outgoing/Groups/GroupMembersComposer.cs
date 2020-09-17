using StarBlue.HabboHotel.Groups;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class GroupMembersComposer : MessageComposer
    {
        public Group Group { get; }
        public ICollection<GroupMember> Members { get; }
        public int MembersCount { get; }
        public int Page { get; }
        public bool Admin { get; }
        public int ReqType { get; }
        public string SearchVal { get; }

        public GroupMembersComposer(Group Group, ICollection<GroupMember> Members, int MembersCount, int Page, bool Admin, int ReqType, string SearchVal)
            : base(Composers.GroupMembersMessageComposer)
        {
            this.Group = Group;
            this.Members = Members;
            this.MembersCount = MembersCount;
            this.Page = Page;
            this.Admin = Admin;
            this.ReqType = ReqType;
            this.SearchVal = SearchVal;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Group.Id);
            packet.WriteString(Group.Name);
            packet.WriteInteger(Group.RoomId);
            packet.WriteString(Group.Badge);
            packet.WriteInteger(MembersCount);

            packet.WriteInteger(Members.Count);
            if (MembersCount > 0)
            {
                foreach (GroupMember Data in Members)
                {
                    packet.WriteInteger(Group.CreatorId == Data.Id ? 0 : Group.IsAdmin(Data.Id) ? 1 : Group.IsMember(Data.Id) ? 2 : 3);
                    packet.WriteInteger(Data.Id);
                    packet.WriteString(Data.Username);
                    packet.WriteString(Data.Look);
                    packet.WriteString(Data.JoinedTime);
                }
            }

            packet.WriteBoolean(Admin);
            packet.WriteInteger(14);
            packet.WriteInteger(Page);
            packet.WriteInteger(ReqType);
            packet.WriteString(SearchVal);
        }
    }
}