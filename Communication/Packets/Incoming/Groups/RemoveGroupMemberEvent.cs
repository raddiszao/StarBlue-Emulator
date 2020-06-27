using Database_Manager.Database.Session_Details.Interfaces;
using Plus.HabboHotel.Groups;
using StarBlue.Communication.Packets.Outgoing.Groups;
using StarBlue.Communication.Packets.Outgoing.Messenger;
using StarBlue.Communication.Packets.Outgoing.Rooms.Permissions;
using StarBlue.HabboHotel.Groups;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Groups
{
    class RemoveGroupMemberEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            if (!StarBlueServer.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if (UserId == Session.GetHabbo().Id)
            {
                if (Group.IsMember(UserId))
                {
                    Group.DeleteMember(UserId);
                }

                if (Group.IsAdmin(UserId))
                {
                    if (Group.IsAdmin(UserId))
                    {
                        Group.TakeAdmin(UserId);
                    }


                    if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room Room))
                    {
                        return;
                    }

                    RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                    if (User != null)
                    {
                        User.RemoveStatus("flatctrl 1");
                        User.UpdateNeeded = true;

                        if (User.GetClient() != null)
                        {
                            User.GetClient().SendMessage(new YouAreControllerComposer(0));
                        }
                    }
                }

                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("DELETE FROM `group_memberships` WHERE `group_id` = @GroupId AND `user_id` = @UserId");
                    dbClient.AddParameter("GroupId", GroupId);
                    dbClient.AddParameter("UserId", UserId);
                    dbClient.RunQuery();
                }

                Session.SendMessage(new GroupInfoComposer(Group, Session));
                if (Session.GetHabbo().GetStats().FavouriteGroupId == GroupId)
                {
                    Session.GetHabbo().GetStats().FavouriteGroupId = 0;
                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunFastQuery("UPDATE `user_stats` SET `groupid` = '0' WHERE `id` = '" + UserId + "' LIMIT 1");
                    }

                    if (Group.AdminOnlyDeco == 0)
                    {
                        if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room Room))
                        {
                            return;
                        }

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                        if (User != null)
                        {
                            User.RemoveStatus("flatctrl 1");
                            User.UpdateNeeded = true;

                            if (User.GetClient() != null)
                            {
                                User.GetClient().SendMessage(new YouAreControllerComposer(0));
                            }
                        }
                    }

                    if (Session.GetHabbo().InRoom && Session.GetHabbo().CurrentRoom != null)
                    {
                        RoomUser User = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                        if (User != null)
                        {
                            Session.GetHabbo().CurrentRoom.SendMessage(new UpdateFavouriteGroupComposer(Session.GetHabbo().Id, Group, User.VirtualId));
                        }

                        Session.GetHabbo().CurrentRoom.SendMessage(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
                    }
                    else
                    {
                        Session.SendMessage(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
                    }
                }
                if (Group.HasChat)
                {
                    var Client = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(UserId);
                    if (Client != null)
                    {
                        Client.SendMessage(new FriendListUpdateComposer(Group, -1));
                    }
                }
                return;
            }

            else
            {
                if (Group.CreatorId == Session.GetHabbo().Id || Group.IsAdmin(Session.GetHabbo().Id))
                {
                    if (!Group.IsMember(UserId))
                    {
                        return;
                    }

                    if (Group.IsAdmin(UserId) && Group.CreatorId != Session.GetHabbo().Id)
                    {
                        Session.SendNotification("Lo sentimos, sólo los creadores del grupo pueden eliminar otros administradores del grupo");
                        return;
                    }

                    if (Group.IsAdmin(UserId))
                    {
                        Group.TakeAdmin(UserId);
                    }

                    if (Group.IsMember(UserId))
                    {
                        Group.DeleteMember(UserId);
                    }

                    List<GroupMember> Members = new List<GroupMember>();

                    foreach (KeyValuePair<int, string> Data in Group.GetAllMembers)
                    {
                        GroupMember GroupMember = Group.getGroupMember(Data.Key, Group.Id);
                        if (GroupMember == null)
                        {
                            continue;
                        }

                        if (!Members.Contains(GroupMember))
                        {
                            Members.Add(GroupMember);
                        }
                    }


                    int FinishIndex = 14 < Members.Count ? 14 : Members.Count;
                    int MembersCount = Members.Count;

                    Session.SendMessage(new GroupMembersComposer(Group, Members.Take(FinishIndex).ToList(), MembersCount, 1, (Group.CreatorId == Session.GetHabbo().Id || Group.IsAdmin(Session.GetHabbo().Id)), 0, ""));
                    if (Group.HasChat)
                    {
                        var Client = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(UserId);
                        if (Client != null)
                        {
                            Client.SendMessage(new FriendListUpdateComposer(Group, -1));
                        }
                    }
                }
            }
        }
    }
}