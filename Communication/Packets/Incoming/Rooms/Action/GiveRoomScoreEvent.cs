
using StarBlue.Communication.Packets.Outgoing.Navigator;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Rooms;


namespace StarBlue.Communication.Packets.Incoming.Rooms.Action
{
    internal class GiveRoomScoreEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }


            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            if (Session.GetHabbo().RatedRooms.Contains(Room.Id))
            {
                return;
            }

            int Rating = Packet.PopInt();
            switch (Rating)
            {
                case -1:
                    Room.RoomData.Score--;
                    break;

                case 1:

                    Room.RoomData.Score++;
                    break;

                default:

                    return;
            }


            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE rooms SET score = '" + Room.RoomData.Score + "' WHERE id = '" + Room.Id + "' LIMIT 1");
            }

            Session.GetHabbo().RatedRooms.Add(Room.Id);
            Session.SendMessage(new RoomRatingComposer(Room.RoomData.Score, !(Session.GetHabbo().RatedRooms.Contains(Room.Id) || Room.CheckRights(Session, true))));
        }
    }
}
