using StarBlue.Communication.Packets.Outgoing.Groups;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Groups.Forums;

namespace StarBlue.Communication.Packets.Incoming.Groups
{
    class GetForumStatsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var GroupForumId = Packet.PopInt();

            if (!StarBlueServer.GetGame().GetGroupForumManager().TryGetForum(GroupForumId, out GroupForum Forum))
            {
                Session.SendNotification("Opss, Forum inexistente!");
                return;
            }

            Session.SendMessage(new GetGroupForumsMessageEvent(Forum, Session));

        }
    }
}
