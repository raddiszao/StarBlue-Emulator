﻿using StarBlue.Communication.Packets.Outgoing.Inventory.Trading;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Rooms;


namespace StarBlue.Communication.Packets.Incoming.Inventory.Trading
{
    internal class InitTradeEvent : IPacketEvent
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

            if (!Room.CanTradeInRoom || StarBlueServer.GoingIsToBeClose)
            {
                Session.SendNotification("Essa função foi desativada até o servidor for reinicializado.");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            if (Session.GetHabbo().TradingLockExpiry > 0)
            {
                if (Session.GetHabbo().TradingLockExpiry > StarBlueServer.GetUnixTimestamp())
                {
                    Session.SendNotification("Actualmente su cuenta tiene un Baneo para tradear.");
                    return;
                }
                else
                {
                    Session.GetHabbo().TradingLockExpiry = 0;
                    Session.SendNotification("Su baneo para tradear ha expirado, por favor no lo vuelva a hacer.");

                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunFastQuery("UPDATE `user_info` SET `trading_locked` = '0' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    }
                }
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByVirtualId(Packet.PopInt());

            if (TargetUser == null || TargetUser.GetClient() == null || TargetUser.GetClient().GetHabbo() == null)
            {
                return;
            }

            if (TargetUser.IsTrading)
            {
                Session.SendMessage(new TradingErrorComposer(8, TargetUser.GetUsername()));
                return;
            }

            if (!Session.GetHabbo().GetPermissions().HasRight("room_trade_override"))
            {
                if (Room.RoomData.TradeSettings == 1 && Room.RoomData.OwnerId != Session.GetHabbo().Id)//Owner only.
                {
                    Session.SendMessage(new TradingErrorComposer(6, TargetUser.GetUsername()));
                    return;
                }
                else if (Room.RoomData.TradeSettings == 0 && Room.RoomData.OwnerId != Session.GetHabbo().Id)//Trading is disabled.
                {
                    Session.SendMessage(new TradingErrorComposer(6, TargetUser.GetUsername()));
                    return;
                }
            }

            if (TargetUser.GetClient().GetHabbo().TradingLockExpiry > 0)
            {
                Session.SendNotification("Oops, al parecer este usuario tiene un baneo para tradear!");
                return;
            }

            Room.TryStartTrade(User, TargetUser);
        }
    }
}