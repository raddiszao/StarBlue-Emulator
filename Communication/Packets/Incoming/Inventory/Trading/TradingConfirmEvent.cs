
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.Trading;

namespace StarBlue.Communication.Packets.Incoming.Inventory.Trading
{
    class TradingConfirmEvent : IPacketEvent
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

            Trade Trade = Room.GetUserTrade(Session.GetHabbo().Id);
            if (Trade == null)
            {
                return;
            }

            Trade.CompleteTrade(Session.GetHabbo().Id);
        }
    }
}