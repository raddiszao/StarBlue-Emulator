using StarBlue.HabboHotel.Groups.Forums;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    class ThreadDataComposer : ServerPacket
    {
        public ThreadDataComposer(GroupForumThread Thread, int StartIndex, int MaxLength)
            : base(ServerPacketHeader.ThreadDataMessageComposer)
        {
            base.WriteInteger(Thread.ParentForum.Id);
            base.WriteInteger(Thread.Id);
            base.WriteInteger(StartIndex);
            base.WriteInteger(Thread.Posts.Count); //Messages count

            foreach (var Post in Thread.Posts)
            {
                Post.SerializeData(this);
            }

        }
    }
}


