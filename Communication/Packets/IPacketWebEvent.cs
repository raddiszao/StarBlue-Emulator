using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.WebClient;

namespace StarBlue.Communication.Packets
{
    public interface IPacketWebEvent
    {
        void Parse(WebClient session, ClientPacket packet);
    }
}
