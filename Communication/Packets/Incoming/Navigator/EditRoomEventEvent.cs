﻿using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.HabboHotel.Rooms;
using System;
using System.Text;

namespace StarBlue.Communication.Packets.Incoming.Navigator
{
    class EditRoomEventEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int RoomId = Packet.PopInt();
            string Name = Packet.PopString();
            Name = StarBlueServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Name, out string word) ? "Spam" : Name;
            string Desc = Packet.PopString();
            Desc = StarBlueServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Desc, out word) ? "Spam" : Desc;

            RoomData Data = StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            if (Data == null)
            {
                return;
            }

            if (Data.OwnerId != Session.GetHabbo().Id)
            {
                return; //HAX
            }

            if (Data.Promotion == null)
            {
                Session.SendNotification("Oops, não existe uma promoção nesta sala.");
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `room_promotions` SET `title` = @title, `description` = @desc WHERE `room_id` = " + RoomId + " LIMIT 1");
                dbClient.AddParameter("title", Encoding.UTF8.GetString(Encoding.Default.GetBytes(Name)));
                dbClient.AddParameter("desc", Encoding.UTF8.GetString(Encoding.Default.GetBytes(Desc)));
                dbClient.RunQuery();
            }

            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Convert.ToInt32(RoomId), out Room Room))
            {
                return;
            }

            Data.Promotion.Name = Name;
            Data.Promotion.Description = Desc;
            Room.SendMessage(new RoomEventComposer(Data, Data.Promotion));
        }
    }
}