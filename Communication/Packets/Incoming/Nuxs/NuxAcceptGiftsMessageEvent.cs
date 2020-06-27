using StarBlue.Communication.Packets.Outgoing.Rooms.Nux;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Nux
{
    class NuxAcceptGiftsMessageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new NuxItemListComposer());
        }
    }
}