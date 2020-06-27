using Fleck;
using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.WebSocket
{
    class EnterRoom : IWebEvent
    {

        public void Execute(GameClient Client, string Data, IWebSocketConnection Socket)
        {
            if (!int.TryParse(Data, out int RoomId))
            {
                return;
            }

            if (Client.GetHabbo().CurrentRoomId == RoomId)
            {
                Client.SendWhisper("Você já está neste quarto.", 34);
                return;
            }

            Client.SendMessage(new RoomForwardComposer(RoomId));
        }
    }
}
