using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Sound
{
    internal class RemoveDiscFromPlayListEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            HabboHotel.Rooms.Room room = Session.GetHabbo().CurrentRoom;
            if (!room.CheckRights(Session))
            {
                return;
            }

            int itemindex = Packet.PopInt();

            HabboHotel.Rooms.TraxMachine.RoomTraxManager trax = room.GetTraxManager();
            if (trax.Playlist.Count < itemindex)
            {
                goto error;
            }

            HabboHotel.Items.Item item = trax.Playlist[itemindex];
            if (!trax.RemoveDisc(item))
            {
                goto error;
            }

            return;
            error:
            Session.SendMessage(new RoomNotificationComposer("", "Ocorreu um erro ao remover o disco.", "error", "", ""));
        }
    }
}
