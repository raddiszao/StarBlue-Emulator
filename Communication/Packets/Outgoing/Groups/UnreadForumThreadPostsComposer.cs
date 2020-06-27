namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    class UnreadForumThreadPostsComposer : ServerPacket
    {
        public UnreadForumThreadPostsComposer(int count)
            : base(ServerPacketHeader.UnreadForumThreadPostsMessageComposer)
        {
            base.WriteInteger(count);
        }
    }
}


