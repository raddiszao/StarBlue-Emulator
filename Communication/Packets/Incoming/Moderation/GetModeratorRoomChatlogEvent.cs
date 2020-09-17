
using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Moderation
{
    internal class GetModeratorRoomChatlogEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                return;
            }

            int Junk = Packet.PopInt();

            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Packet.PopInt(), out Room Room))
            {
                return;
            }

            try
            {
                Session.SendMessage(new ModeratorRoomChatlogComposer(Room));
            }
            catch { Session.SendNotification("Overflow :/"); }
        }
    }
}