namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class UnreadForumThreadPostsComposer : ServerPacket
    {
        public UnreadForumThreadPostsComposer(int count)
            : base(ServerPacketHeader.UnreadForumThreadPostsMessageComposer)
        {
            base.WriteInteger(count);
        }
    }
}


