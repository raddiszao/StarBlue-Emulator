using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets
{
    public interface IPacketEvent
    {
        void Parse(GameClient Session, ClientPacket Packet);
    }
}