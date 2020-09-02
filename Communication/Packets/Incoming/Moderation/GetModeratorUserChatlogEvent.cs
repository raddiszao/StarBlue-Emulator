
using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.HabboHotel.Users;

namespace StarBlue.Communication.Packets.Incoming.Moderation
{
    internal class GetModeratorUserChatlogEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                return;
            }

            int UserId = Packet.PopInt();
            Habbo Habbo = StarBlueServer.GetHabboById(UserId);

            if (Habbo == null)
            {
                Session.SendNotification("Não foi possível encontrar esse usuário.");
                return;
            }

            try
            {
                Session.SendMessage(new ModeratorUserChatlogComposer(UserId));
            }
            catch { Session.SendNotification("Overflow :/"); }
        }
    }
}
