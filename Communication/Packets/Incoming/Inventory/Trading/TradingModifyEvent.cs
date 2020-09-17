
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.Trading;

namespace StarBlue.Communication.Packets.Incoming.Inventory.Trading
{
    internal class TradingModifyEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
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

            Trade.Unaccept(Session.GetHabbo().Id);
        }
    }
}