using StarBlue.Communication.Packets.Outgoing.Groups;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.Groups
{
    class GetGroupCreationWindowEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null)
            {
                return;
            }

            List<RoomData> ValidRooms = new List<RoomData>();
            foreach (RoomData Data in Session.GetHabbo().UsersRooms)
            {
                if (Data.Group == null)
                {
                    ValidRooms.Add(Data);
                }
            }

            Session.SendMessage(new GroupCreationWindowComposer(ValidRooms));
        }
    }
}
