using StarBlue.Communication.Packets.Outgoing.Rooms.Settings;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Settings
{
    internal class ToggleMuteToolEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
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

            Room.RoomData.RoomMuted = !Room.RoomData.RoomMuted;

            List<RoomUser> roomUsers = Room.GetRoomUserManager().GetRoomUsers();
            foreach (RoomUser roomUser in roomUsers.ToList())
            {
                if (roomUser == null || roomUser.GetClient() == null)
                {
                    continue;
                }

                if (Room.RoomData.RoomMuted)
                {
                    roomUser.GetClient().SendWhisper("Este quarto foi silenciado.", 34);
                }
                else
                {
                    roomUser.GetClient().SendWhisper("Este quarto foi desmutado.", 34);
                }
            }

            Room.SendMessage(new RoomMuteSettingsComposer(Room.RoomData.RoomMuted));
        }
    }
}
