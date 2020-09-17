using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Avatar
{
    internal class DanceComposer : MessageComposer
    {
        private RoomUser Avatar { get; }
        private int Dance { get; }

        public DanceComposer(RoomUser Avatar, int Dance)
            : base(Composers.DanceMessageComposer)
        {
            this.Avatar = Avatar;
            this.Dance = Dance;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Avatar.VirtualId);
            packet.WriteInteger(Dance);
        }
    }
}