namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    internal class FollowFriendFailedComposer : ServerPacket
    {
        public FollowFriendFailedComposer(int ErrorCode)
            : base(ServerPacketHeader.FollowFriendFailedMessageComposer)
        {
            base.WriteInteger(ErrorCode);
        }
    }
}
