using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Sound
{
    class RemoveDiscFromPlayListEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var room = Session.GetHabbo().CurrentRoom;
            if (!room.CheckRights(Session))
            {
                return;
            }

            var itemindex = Packet.PopInt();

            var trax = room.GetTraxManager();
            if (trax.Playlist.Count < itemindex)
            {
                goto error;
            }

            var item = trax.Playlist[itemindex];
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
