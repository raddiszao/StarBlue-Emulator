namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    internal class FollowFriendFailedComposer : MessageComposer
    {
        public int ErrorCode { get; }

        public FollowFriendFailedComposer(int ErrorCode)
            : base(Composers.FollowFriendFailedMessageComposer)
        {
            this.ErrorCode = ErrorCode;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(ErrorCode);
        }
    }
}
