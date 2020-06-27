using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Sound
{
    class AddDiscToPlayListEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var room = Session.GetHabbo().CurrentRoom;
            if (!room.CheckRights(Session))
            {
                return;
            }

            var itemid = Packet.PopInt();//item id
            var songid = Packet.PopInt();//Song id

            var item = room.GetRoomItemHandler().GetItem(itemid);
            if (item == null)
            {
                return;
            }

            if (!room.GetTraxManager().AddDisc(item))
            {
                Session.SendMessage(new RoomNotificationComposer("", "Ocorreu um erro ao adicionar o disco.", "error", "", ""));
            }
        }
    }
}
