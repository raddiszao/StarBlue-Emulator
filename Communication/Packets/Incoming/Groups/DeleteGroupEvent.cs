﻿using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Messenger;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Groups;
using StarBlue.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Groups
{
    class DeleteGroupEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!StarBlueServer.GetGame().GetGroupManager().TryGetGroup(Packet.PopInt(), out Group Group))
            {
                Session.SendMessage(new RoomNotificationComposer("Oops!",
                 "Não foi encontrado este grupo!", "nothing", ""));
                return;
            }

            if (Group.CreatorId != Session.GetHabbo().Id && !Session.GetHabbo().GetPermissions().HasRight("group_delete_override"))
            {
                Session.SendMessage(new RoomNotificationComposer("Oops!",
                 "¡Sólo el dueño del grupo puede eliminarlo!", "nothing", ""));
                return;
            }

            if (Group.MemberCount >= Convert.ToInt32(StarBlueServer.GetConfig().data["group.member.deletion.limit"]) && !Session.GetHabbo().GetPermissions().HasRight("group_delete_limit_override"))
            {
                Session.SendMessage(new RoomNotificationComposer("Sucesso",
                 "O grupo excede o número de membros permitidos (" + Convert.ToInt32(StarBlueServer.GetConfig().data["group.member.deletion.limit"]) + ").", "nothing", ""));
                return;
            }

            Room Room = StarBlueServer.GetGame().GetRoomManager().LoadRoom(Group.RoomId);

            if (Room != null)
            {
                Room.Group = null;
                Room.RoomData.Group = null;//Eu não tenho certeza se isso é necessário ou não, por causa da herança, mas tudo bem.
            }

            List<GameClient> GroupMembers = (from Client in StarBlueServer.GetGame().GetClientManager().GetClients.ToList() where Client != null && Client.GetHabbo() != null select Client).ToList();
            foreach (GameClient Client in GroupMembers)
            {
                if (Client != null)
                {
                    continue;
                }

                Client.SendMessage(new FriendListUpdateComposer(Room.Group, -1));
            }

            //Removê-lo do cache.
            StarBlueServer.GetGame().GetGroupManager().DeleteGroup(Group.Id);

            //Agora as coisas.
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("DELETE FROM `groups` WHERE `id` = '" + Group.Id + "'");
                dbClient.RunFastQuery("DELETE FROM `group_memberships` WHERE `group_id` = '" + Group.Id + "'");
                dbClient.RunFastQuery("DELETE FROM `group_requests` WHERE `group_id` = '" + Group.Id + "'");
                dbClient.RunFastQuery("UPDATE `rooms` SET `group_id` = '0' WHERE `group_id` = '" + Group.Id + "' LIMIT 1");
                dbClient.RunFastQuery("UPDATE `user_stats` SET `groupid` = '0' WHERE `groupid` = '" + Group.Id + "' LIMIT 1");
                dbClient.RunFastQuery("DELETE FROM `items_groups` WHERE `group_id` = '" + Group.Id + "'");
            }

            //Descarregá-lo pela última vez.
            StarBlueServer.GetGame().GetRoomManager().UnloadRoom(Room.Id, true);

            //Wulles Rainha
            Session.SendMessage(new RoomNotificationComposer("Sucesso", "Você excluiu com sucesso seu grupo.", "nothing", ""));
            return;
        }
    }
}
