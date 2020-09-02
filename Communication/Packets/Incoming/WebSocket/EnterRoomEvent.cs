using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.WebClient;

namespace StarBlue.Communication.Packets.Incoming.WebSocket
{
    class EnterRoomEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            if (Session == null)
                return;

            GameClient Client = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null)
                return;

            int RoomId = Packet.PopInt();
            if (Client.GetHabbo().CurrentRoomId == RoomId)
            {
                Client.SendWhisper("Você já está neste quarto.", 34);
                return;
            }

            Client.SendMessage(new RoomForwardComposer(RoomId));
        }
    }
}
