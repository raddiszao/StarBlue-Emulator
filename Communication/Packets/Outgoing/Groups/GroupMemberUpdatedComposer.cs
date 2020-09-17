
using StarBlue.HabboHotel.Users;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class GroupMemberUpdatedComposer : MessageComposer
    {
        public int GroupId { get; }
        public Habbo Habbo { get; }
        public int Type { get; }

        public GroupMemberUpdatedComposer(int GroupId, Habbo Habbo, int Type)
            : base(Composers.GroupMemberUpdatedMessageComposer)
        {
            this.GroupId = GroupId;
            this.Habbo = Habbo;
            this.Type = Type;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(GroupId);//GroupId
            packet.WriteInteger(Type);//Type?
            {
                packet.WriteInteger(Habbo.Id);//UserId
                packet.WriteString(Habbo.Username);
                packet.WriteString(Habbo.Look);
                packet.WriteString(string.Empty);
            }
        }
    }
}
