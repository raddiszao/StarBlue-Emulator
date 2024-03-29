﻿
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Inventory.Trading
{
    internal class TradingCancelConfirmEvent : IPacketEvent
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

            Room.TryStopTrade(Session.GetHabbo().Id);
        }
    }
}
