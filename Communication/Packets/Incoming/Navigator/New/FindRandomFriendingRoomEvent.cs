
using StarBlue.Communication.Packets.Outgoing.Nux;
using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Navigator
{
    internal class FindRandomFriendingRoomEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string type = Packet.PopString();
            if (type == "predefined_noob_lobby")
            {
                Session.SendMessage(new NuxAlertComposer("nux/lobbyoffer/hide"));
                Session.SendMessage(new RoomForwardComposer(4));
                return;
            }

            Room Instance = StarBlueServer.GetGame().GetRoomManager().TryGetRandomLoadedRoom(Session);
            if (Instance != null)
            {
                Session.SendMessage(new RoomForwardComposer(Instance.Id));
            }
        }
    }
}
