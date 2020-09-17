using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Groups.Forums;
using System;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class ThreadsListDataComposer : MessageComposer
    {
        private GroupForum Forum { get; }
        private int StartIndex { get; }
        private int MaxLength { get; }
        private GameClient Session { get; }

        public ThreadsListDataComposer(GroupForum Forum, GameClient Session, int StartIndex = 0, int MaxLength = 20)
            : base(Composers.ThreadsListDataMessageComposer)
        {
            this.Forum = Forum;
            this.Session = Session;
            this.StartIndex = StartIndex;
            this.MaxLength = MaxLength;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Forum.GroupId);//Group Forum ID
            packet.WriteInteger(StartIndex); //Page Index

            List<GroupForumThread> Threads = Forum.Threads;
            if (Threads.Count - 1 >= StartIndex)
            {
                Threads = Threads.GetRange(StartIndex, Math.Min(MaxLength, Threads.Count - StartIndex));
            }

            packet.WriteInteger(Threads.Count); //Thread Count

            List<GroupForumThread> UnPinneds = new List<GroupForumThread>();

            foreach (GroupForumThread Thread in Threads)
            {
                if (!Thread.Pinned)
                {
                    UnPinneds.Add(Thread);
                    continue;
                }

                Thread.SerializeData(Session, packet);
            }

            foreach (GroupForumThread unPinned in UnPinneds)
            {
                unPinned.SerializeData(Session, packet);
            }
        }
    }
}

