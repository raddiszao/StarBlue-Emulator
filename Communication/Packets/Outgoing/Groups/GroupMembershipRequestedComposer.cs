
using StarBlue.HabboHotel.Users;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class GroupMembershipRequestedComposer : ServerPacket
    {
        public GroupMembershipRequestedComposer(int GroupId, Habbo Habbo, int Type)
            : base(ServerPacketHeader.GroupMembershipRequestedMessageComposer)
        {
            base.WriteInteger(GroupId);//GroupId
            base.WriteInteger(Type);//Type?
            {
                base.WriteInteger(Habbo.Id);//UserId
                base.WriteString(Habbo.Username);
                base.WriteString(Habbo.Look);
                base.WriteString(string.Empty);
            }
        }
    }
}
