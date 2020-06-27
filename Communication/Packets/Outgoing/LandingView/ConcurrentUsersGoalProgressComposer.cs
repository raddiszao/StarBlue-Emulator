namespace StarBlue.Communication.Packets.Outgoing.LandingView
{
    class ConcurrentUsersGoalProgressComposer : ServerPacket
    {
        public ConcurrentUsersGoalProgressComposer(int UsersNow, int Type, int Goal)
            : base(ServerPacketHeader.ConcurrentUsersGoalProgressMessageComposer)
        {
            base.WriteInteger(Type);
            base.WriteInteger(UsersNow);
            base.WriteInteger(Goal);
        }
    }
}
