using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Groups.Forums;
using System;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class ThreadsListDataComposer : ServerPacket
    {
        public ThreadsListDataComposer(GroupForum Forum, GameClient Session, int StartIndex = 0, int MaxLength = 20)
            : base(ServerPacketHeader.ThreadsListDataMessageComposer)
        {
            base.WriteInteger(Forum.GroupId);//Group Forum ID
            base.WriteInteger(StartIndex); //Page Index

            List<GroupForumThread> Threads = Forum.Threads;
            if (Threads.Count - 1 >= StartIndex)
            {
                Threads = Threads.GetRange(StartIndex, Math.Min(MaxLength, Threads.Count - StartIndex));
            }

            base.WriteInteger(Threads.Count); //Thread Count

            List<GroupForumThread> UnPinneds = new List<GroupForumThread>();

            foreach (GroupForumThread Thread in Threads)
            {
                if (!Thread.Pinned)
                {
                    UnPinneds.Add(Thread);
                    continue;
                }

                Thread.SerializeData(Session, this);
            }

            foreach (GroupForumThread unPinned in UnPinneds)
            {
                unPinned.SerializeData(Session, this);
            }
        }
    }
}

