
using StarBlue.HabboHotel.Groups;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class UpdateFavouriteGroupComposer : MessageComposer
    {
        public Group Group { get; }
        public int VirtualId { get; }

        public UpdateFavouriteGroupComposer(Group Group, int VirtualId)
            : base(Composers.UpdateFavouriteGroupMessageComposer)
        {
            this.Group = Group;
            this.VirtualId = VirtualId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(VirtualId);//Sends 0 on .COM
            packet.WriteInteger(Group != null ? Group.Id : 0);
            packet.WriteInteger(3);
            packet.WriteString(Group != null ? Group.Name : string.Empty);
        }
    }
}
