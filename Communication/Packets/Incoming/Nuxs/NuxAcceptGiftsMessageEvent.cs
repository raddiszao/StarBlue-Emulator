using StarBlue.Communication.Packets.Outgoing.Rooms.Nux;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Nux
{
    internal class NuxAcceptGiftsMessageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            Session.SendMessage(new NuxItemListComposer());
        }
    }
}