﻿using StarBlue.Communication.Packets.Outgoing;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Groups;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users.Messenger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace StarBlue.HabboHotel.Navigator
{
    internal static class NavigatorHandler
    {
        public static void Search(Composer Message, SearchResultList SearchResult, string SearchData, GameClient Session, int FetchLimit)
        {
            //Switching by categorys.
            switch (SearchResult.CategoryType)
            {
                default:
                    Message.WriteInteger(0);
                    break;

                case NavigatorCategoryType.QUERY:
                    {
                        #region Query
                        if (SearchData.ToLower().StartsWith("owner:"))
                        {
                            if (SearchData.Length > 0)
                            {
                                DataTable GetRooms = null;
                                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                                {
                                    if (SearchData.ToLower().StartsWith("owner:"))
                                    {
                                        dbClient.SetQuery("SELECT r.* FROM rooms r, users u WHERE u.username = @username AND r.owner = u.id ORDER BY r.users_now DESC LIMIT 80;");
                                        dbClient.AddParameter("username", SearchData.Remove(0, 6));
                                        GetRooms = dbClient.GetTable();
                                    }
                                }

                                List<RoomData> Results = new List<RoomData>();
                                if (GetRooms != null)
                                {
                                    foreach (DataRow Row in GetRooms.Rows)
                                    {
                                        RoomData RoomData = StarBlueServer.GetGame().GetRoomManager().FetchRoomData(Convert.ToInt32(Row["id"]), Row);
                                        if (RoomData != null && !Results.Contains(RoomData))
                                        {
                                            if (RoomData.Access == RoomAccess.INVISIBLE)
                                            {
                                                Room Room = StarBlueServer.GetGame().GetRoomManager().LoadRoom(RoomData.Id);
                                                if (Room != null && Room.CheckRights(Session, false, true))
                                                {
                                                    Results.Add(RoomData);
                                                }
                                            }
                                            else
                                            {
                                                Results.Add(RoomData);
                                            }
                                        }
                                    }
                                }

                                Message.WriteInteger(Results.Count);
                                foreach (RoomData Data in Results.ToList())
                                {
                                    RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                                }
                            }
                        }
                        else if (SearchData.ToLower().StartsWith("tag:"))
                        {
                            SearchData = SearchData.Remove(0, 4);
                            ICollection<Room> TagMatches = StarBlueServer.GetGame().GetRoomManager().SearchTaggedRooms(Session, SearchData);

                            Message.WriteInteger(TagMatches.Count);
                            foreach (Room Data in TagMatches.ToList())
                            {
                                RoomAppender.WriteRoom(Message, Data.RoomData, Data.RoomData.Promotion);
                            }
                        }
                        else if (SearchData.ToLower().StartsWith("group:"))
                        {
                            SearchData = SearchData.Remove(0, 6);
                            ICollection<Room> GroupRooms = StarBlueServer.GetGame().GetRoomManager().SearchGroupRooms(Session, SearchData);

                            Message.WriteInteger(GroupRooms.Count);
                            foreach (Room Data in GroupRooms.ToList())
                            {
                                RoomAppender.WriteRoom(Message, Data.RoomData, Data.RoomData.Promotion);
                            }
                        }
                        else
                        {
                            if (SearchData.Length > 0)
                            {
                                DataTable Table = null;
                                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT * FROM rooms WHERE `caption` LIKE @query ORDER BY `users_now` DESC LIMIT 50");
                                    if (SearchData.ToLower().StartsWith("roomname:"))
                                    {
                                        dbClient.AddParameter("query", "%" + SearchData.Split(new char[] { ':' }, 2)[1] + "%");
                                    }
                                    else
                                    {
                                        dbClient.AddParameter("query", "%" + SearchData + "%");
                                    }
                                    Table = dbClient.GetTable();
                                }

                                List<RoomData> Results = new List<RoomData>();
                                if (Table != null)
                                {
                                    foreach (DataRow Row in Table.Rows)
                                    {
                                        RoomData RData = StarBlueServer.GetGame().GetRoomManager().FetchRoomData(Convert.ToInt32(Row["id"]), Row);
                                        if (Convert.ToString(Row["state"]) == "invisible")
                                        {
                                            Room Room = StarBlueServer.GetGame().GetRoomManager().LoadRoom(RData.Id);
                                            if (Room != null && !Room.CheckRights(Session))
                                            {
                                                continue;
                                            }
                                        }

                                        if (RData != null && !Results.Contains(RData))
                                        {
                                            Results.Add(RData);
                                        }
                                    }
                                }

                                Message.WriteInteger(Results.Count);
                                foreach (RoomData Data in Results.ToList())
                                {
                                    RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                                }
                            }
                        }
                        #endregion

                        break;
                    }

                case NavigatorCategoryType.FEATURED:
                    #region Featured
                    List<RoomData> Rooms = new List<RoomData>();
                    ICollection<FeaturedRoom> Featured = StarBlueServer.GetGame().GetNavigator().GetFeaturedRooms();
                    foreach (FeaturedRoom FeaturedItem in Featured.ToList())
                    {
                        if (FeaturedItem == null)
                        {
                            continue;
                        }

                        RoomData Data = StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(FeaturedItem.roomId);
                        if (Data == null)
                        {
                            continue;
                        }

                        if (!Rooms.Contains(Data))
                        {
                            Rooms.Add(Data);
                        }
                    }

                    Message.WriteInteger(Rooms.Count);
                    foreach (RoomData Data in Rooms.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                    }
                    #endregion
                    break;

                case NavigatorCategoryType.GAMES:
                    #region Featured
                    List<RoomData> Rooms2 = new List<RoomData>();
                    ICollection<FeaturedRoom2> Featured2 = StarBlueServer.GetGame().GetNavigator().GetFeaturedRooms2();
                    foreach (FeaturedRoom2 FeaturedItem2 in Featured2.ToList())
                    {
                        if (FeaturedItem2 == null)
                        {
                            continue;
                        }

                        RoomData Data = StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(FeaturedItem2.roomId);
                        if (Data == null)
                        {
                            continue;
                        }

                        if (!Rooms2.Contains(Data))
                        {
                            Rooms2.Add(Data);
                        }
                    }

                    Message.WriteInteger(Rooms2.Count);
                    foreach (RoomData Data in Rooms2.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                    }
                    #endregion
                    break;

                case NavigatorCategoryType.STAFF_PICKS:
                    {
                        #region Featured
                        List<RoomData> rooms = new List<RoomData>();

                        ICollection<StaffPick> picks = StarBlueServer.GetGame().GetNavigator().GetStaffPicks();
                        foreach (StaffPick pick in picks.ToList())
                        {
                            if (pick == null)
                            {
                                continue;
                            }

                            RoomData Data = StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(pick.roomId);
                            if (Data == null)
                            {
                                continue;
                            }

                            if (!rooms.Contains(Data))
                            {
                                rooms.Add(Data);
                            }
                        }

                        Message.WriteInteger(rooms.Count);
                        foreach (RoomData data in rooms.ToList())
                        {
                            RoomAppender.WriteRoom(Message, data, data.Promotion);
                        }
                        #endregion
                        break;
                    }

                case NavigatorCategoryType.POPULAR:
                    {
                        List<Room> PopularRooms = StarBlueServer.GetGame().GetRoomManager().GetPopularRooms(Session, -1, FetchLimit);

                        Message.WriteInteger(PopularRooms.Count);
                        foreach (Room Data in PopularRooms.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data.RoomData, Data.RoomData.Promotion);
                        }

                        PopularRooms = null;
                        break;
                    }

                case NavigatorCategoryType.RECOMMENDED:
                    {
                        List<Room> RecommendedRooms = StarBlueServer.GetGame().GetRoomManager().GetRecommendedRooms(Session, FetchLimit);

                        Message.WriteInteger(RecommendedRooms.Count);
                        foreach (Room Data in RecommendedRooms.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data.RoomData, Data.RoomData.Promotion);
                        }
                        break;
                    }

                case NavigatorCategoryType.CATEGORY:
                    {
                        List<Room> GetRoomsByCategory = StarBlueServer.GetGame().GetRoomManager().GetRoomsByCategory(Session, SearchResult.Id, FetchLimit);

                        Message.WriteInteger(GetRoomsByCategory.Count);
                        foreach (Room Data in GetRoomsByCategory.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data.RoomData, Data.RoomData.Promotion);
                        }
                        break;
                    }

                case NavigatorCategoryType.MY_ROOMS:

                    Message.WriteInteger(Session.GetHabbo().UsersRooms.Count);
                    foreach (RoomData Data in Session.GetHabbo().UsersRooms.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                    }
                    break;

                case NavigatorCategoryType.MY_FAVORITES:
                    List<RoomData> Favourites = new List<RoomData>();
                    foreach (int Id in Session.GetHabbo().FavoriteRooms.ToArray())
                    {
                        RoomData Room = StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(Id);
                        if (Room == null)
                        {
                            continue;
                        }

                        if (!Favourites.Contains(Room))
                        {
                            Favourites.Add(Room);
                        }
                    }

                    Favourites = Favourites.Take(FetchLimit).ToList();

                    Message.WriteInteger(Favourites.Count);
                    foreach (RoomData Data in Favourites.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                    }
                    break;

                case NavigatorCategoryType.MY_GROUPS:
                    List<RoomData> MyGroups = new List<RoomData>();

                    foreach (Group Group in StarBlueServer.GetGame().GetGroupManager().GetGroupsForUser(Session.GetHabbo().Id).ToList())
                    {
                        if (Group == null)
                        {
                            continue;
                        }

                        RoomData Data = StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(Group.RoomId);
                        if (Data == null)
                        {
                            continue;
                        }

                        if (!MyGroups.Contains(Data))
                        {
                            MyGroups.Add(Data);
                        }
                    }

                    MyGroups = MyGroups.Take(FetchLimit).ToList();

                    Message.WriteInteger(MyGroups.Count);
                    foreach (RoomData Data in MyGroups.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                    }
                    break;

                case NavigatorCategoryType.MY_FRIENDS_ROOMS:
                    List<RoomData> MyFriendsRooms = new List<RoomData>();
                    foreach (MessengerBuddy buddy in Session.GetHabbo().GetMessenger().GetFriends().Where(p => p.InRoom))
                    {
                        if (buddy == null || !buddy.InRoom || buddy.UserId == Session.GetHabbo().Id)
                        {
                            continue;
                        }

                        if (!MyFriendsRooms.Contains(buddy.CurrentRoom.RoomData))
                        {
                            MyFriendsRooms.Add(buddy.CurrentRoom.RoomData);
                        }
                    }

                    Message.WriteInteger(MyFriendsRooms.Count);
                    foreach (RoomData Data in MyFriendsRooms.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                    }
                    break;

                case NavigatorCategoryType.MY_HISTORY:
                    List<RoomData> MyHistory = new List<RoomData>();

                    DataTable GetHistory = null;
                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("SELECT `room_id` FROM `user_roomvisits` WHERE `user_id` = @UserId ORDER BY `id` DESC LIMIT @FetchLimit");
                        dbClient.AddParameter("UserId", Session.GetHabbo().Id);
                        dbClient.AddParameter("FetchLimit", FetchLimit);
                        GetHistory = dbClient.GetTable();

                        foreach (DataRow Row in GetHistory.Rows)
                        {
                            RoomData Data = StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(Convert.ToInt32(Row["room_id"]));
                            if (Data == null)
                            {
                                continue;
                            }

                            if (!MyHistory.Contains(Data))
                            {
                                MyHistory.Add(Data);
                            }
                        }
                    }
                    Message.WriteInteger(MyHistory.Count);
                    foreach (RoomData Data in MyHistory.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                    }
                    break;

                case NavigatorCategoryType.MY_RIGHTS:
                    List<RoomData> MyRights = new List<RoomData>();

                    DataTable GetRights = null;
                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("SELECT `room_id` FROM `room_rights` WHERE `user_id` = @UserId ORDER BY `id` DESC LIMIT @FetchLimit");
                        dbClient.AddParameter("UserId", Session.GetHabbo().Id);
                        dbClient.AddParameter("FetchLimit", FetchLimit);
                        GetRights = dbClient.GetTable();

                        foreach (DataRow Row in GetRights.Rows)
                        {
                            RoomData Data = StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(Convert.ToInt32(Row["room_id"]));
                            if (Data == null)
                            {
                                continue;
                            }

                            if (!MyRights.Contains(Data))
                            {
                                MyRights.Add(Data);
                            }
                        }
                    }

                    Message.WriteInteger(MyRights.Count);
                    foreach (RoomData Data in MyRights.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                    }
                    break;

                case NavigatorCategoryType.TOP_PROMOTIONS:
                    {
                        List<Room> GetPopularPromotions = StarBlueServer.GetGame().GetRoomManager().GetOnGoingRoomPromotions(Session, 16, FetchLimit);

                        Message.WriteInteger(GetPopularPromotions.Count);
                        foreach (Room Data in GetPopularPromotions.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data.RoomData, Data.RoomData.Promotion);
                        }
                        break;
                    }

                case NavigatorCategoryType.PROMOTION_CATEGORY:
                    {
                        List<Room> GetPromotedRooms = StarBlueServer.GetGame().GetRoomManager().GetPromotedRooms(Session, SearchResult.Id, FetchLimit);

                        Message.WriteInteger(GetPromotedRooms.Count);
                        foreach (Room Data in GetPromotedRooms.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data.RoomData, Data.RoomData.Promotion);
                        }
                        break;
                    }
            }
        }
    }
}