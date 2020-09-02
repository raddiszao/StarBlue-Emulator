using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Sound
{
    internal class AddDiscToPlayListEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            HabboHotel.Rooms.Room room = Session.GetHabbo().CurrentRoom;
            if (!room.CheckRights(Session))
            {
                return;
            }

            int itemid = Packet.PopInt();//item id
            int songid = Packet.PopInt();//Song id

            HabboHotel.Items.Item item = room.GetRoomItemHandler().GetItem(itemid);
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
