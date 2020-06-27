using Fleck;

using StarBlue.HabboHotel.GameClients;

namespace StarBlue.WebSocket
{
    public interface IWebEvent
    {
        void Execute(GameClient Client, string Data, IWebSocketConnection Socket);
    }
}
