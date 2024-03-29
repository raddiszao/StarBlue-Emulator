﻿using StarBlue.Communication.Packets.Outgoing.Rooms.Permissions;
using StarBlue.Communication.Packets.Outgoing.Rooms.Settings;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Cache;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Action
{
    internal class AssignRightsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            int UserId = Packet.PopInt();

            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            if (!Room.CheckRights(Session, true))
            {
                return;
            }

            if (Room.UsersWithRights.Contains(UserId))
            {
                Session.SendNotification(StarBlueServer.GetLanguageManager().TryGetValue("room_rights_has_rights_error"));
                return;
            }

            Room.UsersWithRights.Add(UserId);

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("INSERT INTO `room_rights` (`room_id`,`user_id`) VALUES ('" + Room.Id + "','" + UserId + "')");
            }

            RoomUser RoomUser = Room.GetRoomUserManager().GetRoomUserByHabbo(UserId);
            if (RoomUser != null && !RoomUser.IsBot)
            {
                RoomUser.SetStatus("flatctrl", "1");
                RoomUser.UpdateNeeded = true;
                if (RoomUser.GetClient() != null)
                {
                    RoomUser.GetClient().SendMessage(new YouAreControllerComposer(1));
                }

                Session.SendMessage(new FlatControllerAddedComposer(Room.Id, RoomUser.GetClient().GetHabbo().Id, RoomUser.GetClient().GetHabbo().Username));
            }
            else
            {
                UserCache User = StarBlueServer.GetGame().GetCacheManager().GenerateUser(UserId);
                if (User != null)
                {
                    Session.SendMessage(new FlatControllerAddedComposer(Room.Id, User.Id, User.Username));
                }
            }
        }
    }
}
