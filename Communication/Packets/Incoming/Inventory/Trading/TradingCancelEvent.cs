
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Inventory.Trading
{
    class TradingCancelEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
            {
                return;
            }


            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            if (!Room.CanTradeInRoom)
            {
                return;
            }

            Room.TryStopTrade(Session.GetHabbo().Id);
        }
    }
}