﻿using StarBlue.Communication.Packets.Outgoing.Catalog;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    public class PurchaseRoomPromotionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            int PageId = Packet.PopInt();
            int ItemId = Packet.PopInt();
            int RoomId = Packet.PopInt();
            string Name = Packet.PopString();
            Name = StarBlueServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Name, out string word) ? "Spam" : Name;
            bool junk3 = Packet.PopBoolean();
            string Desc = Packet.PopString();
            Desc = StarBlueServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Desc, out word) ? "Spam" : Desc;
            int CategoryId = Packet.PopInt();

            RoomData Data = StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            if (Data == null)
            {
                return;
            }

            if (Data.OwnerId != Session.GetHabbo().Id)
            {
                return;
            }

            if (Data.Promotion == null)
            {
                Data.Promotion = new RoomPromotion(Name, Desc, CategoryId);
            }
            else
            {
                Data.Promotion.Name = Name;
                Data.Promotion.Description = Desc;
                Data.Promotion.TimestampExpires += 7200;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("REPLACE INTO `room_promotions` (`room_id`,`title`,`description`,`timestamp_start`,`timestamp_expire`,`category_id`) VALUES (@room_id, @title, @description, @start, @expires, @CategoryId)");
                dbClient.AddParameter("room_id", RoomId);
                dbClient.AddParameter("title", Name);
                dbClient.AddParameter("description", Desc);
                dbClient.AddParameter("start", Data.Promotion.TimestampStarted);
                dbClient.AddParameter("expires", Data.Promotion.TimestampExpires);
                dbClient.AddParameter("CategoryId", CategoryId);
                dbClient.RunQuery();
            }

            if (!Session.GetHabbo().GetBadgeComponent().HasBadge("RADZZ"))
            {
                Session.GetHabbo().GetBadgeComponent().GiveBadge("RADZZ", true, Session);
            }

            Session.SendMessage(new PurchaseOKComposer());
            if (Session.GetHabbo().InRoom && Session.GetHabbo().CurrentRoomId == RoomId)
            {
                Session.GetHabbo().CurrentRoom.SendMessage(new RoomEventComposer(Data, Data.Promotion));
            }

            Session.GetHabbo().GetMessenger().BroadcastAchievement(Session.GetHabbo().Id, HabboHotel.Users.Messenger.MessengerEventTypes.EVENT_STARTED, Name);
        }
    }
}