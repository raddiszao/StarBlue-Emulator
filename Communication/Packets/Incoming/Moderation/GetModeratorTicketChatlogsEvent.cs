using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.HabboHotel.Moderation;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Moderation
{
    class GetModeratorTicketChatlogsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_tickets"))
            {
                return;
            }

            int TicketId = Packet.PopInt();

            if (!StarBlueServer.GetGame().GetModerationManager().TryGetTicket(TicketId, out ModerationTicket Ticket) || Ticket.Room == null)
            {
                return;
            }

            RoomData Data = StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(Ticket.Room.Id);
            if (Data == null)
            {
                return;
            }

            Session.SendMessage(new ModeratorTicketChatlogComposer(Ticket, Data, Ticket.Timestamp));
        }
    }
}
