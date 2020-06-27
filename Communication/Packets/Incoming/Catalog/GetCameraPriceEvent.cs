using StarBlue.Communication.Packets.Outgoing.Rooms.Camera;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    class GetCameraPriceEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new SetCameraPicturePriceMessageComposer(100, 10, 0));
        }
    }
}
