using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Connection
{
    internal class GoToFlatAsSpectatorEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            //Session.GetHabbo().Spectating = true;
            //Session.SendMessage(new RoomSpectatorComposer());

            //Room roomToSpec = Session.GetHabbo().CurrentRoom;

            //roomToSpec.QueueingUsers.Remove(Session.GetHabbo());
            //foreach (Habbo user in roomToSpec.QueueingUsers)
            //{
            //    if (roomToSpec.QueueingUsers.First().Id == user.Id)
            //    {
            //        user.PrepareRoom(roomToSpec.Id, "");
            //    }
            //    else
            //    {
            //        user.GetClient().SendMessage(new RoomQueueComposer(roomToSpec.QueueingUsers.IndexOf(user)));
            //    }
            //}

            //if (!Session.GetHabbo().EnterRoom(Session.GetHabbo().CurrentRoom))
            // {
            //    Session.SendMessage(new CloseConnectionComposer());
            // }
        }
    }
}
