using StarBlue.Communication.Packets.Outgoing.Rooms.Settings;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Settings
{
    class ToggleMuteToolEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            Room.RoomMuted = !Room.RoomMuted;

            List<RoomUser> roomUsers = Room.GetRoomUserManager().GetRoomUsers();
            foreach (RoomUser roomUser in roomUsers.ToList())
            {
                if (roomUser == null || roomUser.GetClient() == null)
                {
                    continue;
                }

                if (Room.RoomMuted)
                {
                    roomUser.GetClient().SendWhisper("Esta sala foi silenciada.");
                }
                else
                {
                    roomUser.GetClient().SendWhisper("A sala foi desmutada.");
                }
            }

            Room.SendMessage(new RoomMuteSettingsComposer(Room.RoomMuted));
        }
    }
}
