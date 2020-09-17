using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Avatar
{
    public class SleepComposer : MessageComposer
    {
        private RoomUser User { get; }
        private bool IsSleeping { get; }

        public SleepComposer(RoomUser User, bool IsSleeping)
            : base(Composers.SleepMessageComposer)
        {
            this.User = User;
            this.IsSleeping = IsSleeping;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(User.VirtualId);
            packet.WriteBoolean(IsSleeping);
        }
    }
}