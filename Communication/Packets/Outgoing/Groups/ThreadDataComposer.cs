using StarBlue.HabboHotel.Groups.Forums;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class ThreadDataComposer : MessageComposer
    {
        private GroupForumThread Thread { get; }
        private int StartIndex { get; }

        public ThreadDataComposer(GroupForumThread Thread, int StartIndex)
            : base(Composers.ThreadDataMessageComposer)
        {
            this.Thread = Thread;
            this.StartIndex = StartIndex;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Thread.ParentForum.Id);
            packet.WriteInteger(Thread.Id);
            packet.WriteInteger(StartIndex);
            packet.WriteInteger(Thread.Posts.Count); //Messages count

            foreach (GroupForumThreadPost Post in Thread.Posts)
            {
                Post.SerializeData(packet);
            }

        }
    }
}


