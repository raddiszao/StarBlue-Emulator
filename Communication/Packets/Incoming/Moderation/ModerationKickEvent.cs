using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Moderation
{
    internal class ModerationKickEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_kick"))
            {
                return;
            }

            int UserId = Packet.PopInt();
            string Message = Packet.PopString();

            GameClient Client = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(UserId);
            if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().CurrentRoomId < 1 || Client.GetHabbo().Id == Session.GetHabbo().Id)
            {
                return;
            }

            if (Client.GetHabbo().Rank >= Session.GetHabbo().Rank)
            {
                Session.SendNotification(StarBlueServer.GetLanguageManager().TryGetValue("moderation_kick_permissions"));
                return;
            }

            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            Room.GetRoomUserManager().RemoveUserFromRoom(Client, true, false);
        }
    }
}
