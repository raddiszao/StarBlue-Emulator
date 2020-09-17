using StarBlue.HabboHotel.WebClient;

namespace StarBlue.Communication.WebSocket
{
    public interface IPacketWebEvent
    {
        void Parse(WebClient session, MessageWebEvent packet);
    }
}
