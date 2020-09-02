using StarBlue.Communication.Packets.Outgoing.Groups;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Groups
{
    internal class GetThreadsListDataEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int ForumId = Packet.PopInt(); //Forum ID
            int Int2 = Packet.PopInt(); //Start Index of Thread Count
            int Int3 = Packet.PopInt(); //Length of Thread Count

            HabboHotel.Groups.Forums.GroupForum Forum = StarBlueServer.GetGame().GetGroupForumManager().GetForum(ForumId);
            if (Forum == null)
            {
                return;
            }

            Session.SendMessage(new ThreadsListDataComposer(Forum, Session, Int2, Int3));
        }
    }
}

