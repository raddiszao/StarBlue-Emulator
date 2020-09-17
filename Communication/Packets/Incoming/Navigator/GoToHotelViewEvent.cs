using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Navigator
{
    internal class GoToHotelViewEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            if (Session.GetHabbo().InRoom)
            {
                if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room OldRoom))
                {
                    return;
                }

                if (OldRoom.GetRoomUserManager() != null)
                {
                    OldRoom.GetRoomUserManager().RemoveUserFromRoom(Session, true, false);
                }
            }

            Session.GetHabbo().CurrentRoomId = 0;
        }
    }
}
