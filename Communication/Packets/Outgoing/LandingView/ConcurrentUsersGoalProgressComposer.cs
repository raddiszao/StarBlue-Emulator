namespace StarBlue.Communication.Packets.Outgoing.LandingView
{
    internal class ConcurrentUsersGoalProgressComposer : MessageComposer
    {
        private int UsersNow { get; }
        private int Type { get; }
        private int Goal { get; }

        public ConcurrentUsersGoalProgressComposer(int UsersNow, int Type, int Goal)
            : base(Composers.ConcurrentUsersGoalProgressMessageComposer)
        {
            this.UsersNow = UsersNow;
            this.Type = Type;
            this.Goal = Goal;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Type);
            packet.WriteInteger(UsersNow);
            packet.WriteInteger(Goal);
        }
    }
}
