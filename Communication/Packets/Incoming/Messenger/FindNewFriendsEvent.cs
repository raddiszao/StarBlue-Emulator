
using StarBlue.Communication.Packets.Outgoing.Messenger;
using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Messenger
{
    internal class FindNewFriendsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            Room Instance = StarBlueServer.GetGame().GetRoomManager().TryGetRandomLoadedRoom(Session);

            if (Instance != null)
            {
                Session.SendMessage(new FindFriendsProcessResultComposer(true));
                Session.SendMessage(new RoomForwardComposer(Instance.Id));
            }
            else
            {
                Session.SendMessage(new FindFriendsProcessResultComposer(false));
            }
        }
    }
}
