
using StarBlue.HabboHotel.Groups;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class GroupFurniSettingsComposer : MessageComposer
    {
        public Group Group { get; }
        public int ItemId { get; }
        public int UserId { get; }

        public GroupFurniSettingsComposer(Group Group, int ItemId, int UserId)
            : base(Composers.GroupFurniSettingsMessageComposer)
        {
            this.Group = Group;
            this.ItemId = ItemId;
            this.UserId = UserId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(ItemId);//Item Id
            packet.WriteInteger(Group.Id);//Group Id?
            packet.WriteString(Group.Name);
            packet.WriteInteger(Group.RoomId);//RoomId
            packet.WriteBoolean(Group.IsMember(UserId));//Member?
            packet.WriteBoolean(Group.ForumEnabled);//Has a forum
        }
    }
}