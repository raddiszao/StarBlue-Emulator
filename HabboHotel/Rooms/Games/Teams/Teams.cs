﻿using StarBlue.HabboHotel.Items;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Games.Teams
{

    public class TeamManager
    {
        public string Game;
        public List<RoomUser> BlueTeam;
        public List<RoomUser> GreenTeam;
        public List<RoomUser> RedTeam;
        public List<RoomUser> YellowTeam;

        public static TeamManager createTeamforGame(string Game)
        {
            var t = new TeamManager
            {
                Game = Game,
                BlueTeam = new List<RoomUser>(),
                RedTeam = new List<RoomUser>(),
                GreenTeam = new List<RoomUser>(),
                YellowTeam = new List<RoomUser>()
            };
            return t;
        }

        public bool CanEnterOnTeam(TEAM t)
        {
            if (t.Equals(TEAM.BLUE))
            {
                return (BlueTeam.Count < 5);
            }
            else if (t.Equals(TEAM.RED))
            {
                return (RedTeam.Count < 5);
            }
            else if (t.Equals(TEAM.YELLOW))
            {
                return (YellowTeam.Count < 5);
            }
            else if (t.Equals(TEAM.GREEN))
            {
                return (GreenTeam.Count < 5);
            }

            return false;
        }

        public void AddUser(RoomUser user)
        {
            if (user.Team.Equals(TEAM.BLUE) && !BlueTeam.Contains(user))
            {
                BlueTeam.Add(user);
            }
            else if (user.Team.Equals(TEAM.RED) && !RedTeam.Contains(user))
            {
                RedTeam.Add(user);
            }
            else if (user.Team.Equals(TEAM.YELLOW) && !YellowTeam.Contains(user))
            {
                YellowTeam.Add(user);
            }
            else if (user.Team.Equals(TEAM.GREEN) && !GreenTeam.Contains(user))
            {
                GreenTeam.Add(user);
            }

            switch (Game.ToLower())
            {
                case "banzai":
                    {
                        Room room = user.GetClient().GetHabbo().CurrentRoom;
                        if (room == null)
                        {
                            return;
                        }

                        foreach (Item Item in room.GetRoomItemHandler().GetFloor.ToList())
                        {
                            if (Item == null)
                            {
                                continue;
                            }

                            if (Item.GetBaseItem().InteractionType.Equals(InteractionType.banzaigateblue))
                            {
                                Item.ExtraData = BlueTeam.Count.ToString();
                                Item.UpdateState();
                                if (BlueTeam.Count == 5)
                                {
                                    foreach (RoomUser sser in room.GetGameMap().GetRoomUsers(new Point(Item.GetX, Item.GetY)))
                                    {
                                        sser.SqState = 0;
                                    }

                                    room.GetGameMap().GameMap[Item.GetX, Item.GetY] = 0;
                                }
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.banzaigatered))
                            {
                                Item.ExtraData = RedTeam.Count.ToString();
                                Item.UpdateState();
                                if (RedTeam.Count == 5)
                                {
                                    foreach (RoomUser sser in room.GetGameMap().GetRoomUsers(new Point(Item.GetX, Item.GetY)))
                                    {
                                        sser.SqState = 0;
                                    }

                                    room.GetGameMap().GameMap[Item.GetX, Item.GetY] = 0;
                                }
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.banzaigategreen))
                            {
                                Item.ExtraData = GreenTeam.Count.ToString();
                                Item.UpdateState();
                                if (GreenTeam.Count == 5)
                                {
                                    foreach (RoomUser sser in room.GetGameMap().GetRoomUsers(new Point(Item.GetX, Item.GetY)))
                                    {
                                        sser.SqState = 0;
                                    }

                                    room.GetGameMap().GameMap[Item.GetX, Item.GetY] = 0;
                                }
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.banzaigateyellow))
                            {
                                Item.ExtraData = YellowTeam.Count.ToString();
                                Item.UpdateState();
                                if (YellowTeam.Count == 5)
                                {
                                    foreach (RoomUser sser in room.GetGameMap().GetRoomUsers(new Point(Item.GetX, Item.GetY)))
                                    {
                                        sser.SqState = 0;
                                    }

                                    room.GetGameMap().GameMap[Item.GetX, Item.GetY] = 0;
                                }
                            }
                        }
                        break;
                    }

                case "freeze":
                    {
                        Room room = user.GetClient().GetHabbo().CurrentRoom;
                        if (room == null)
                        {
                            return;
                        }

                        foreach (Item Item in room.GetRoomItemHandler().GetFloor.ToList())
                        {
                            if (Item == null)
                            {
                                continue;
                            }

                            if (Item.GetBaseItem().InteractionType.Equals(InteractionType.FREEZE_BLUE_GATE))
                            {
                                Item.ExtraData = BlueTeam.Count.ToString();
                                Item.UpdateState();
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.FREEZE_RED_GATE))
                            {
                                Item.ExtraData = RedTeam.Count.ToString();
                                Item.UpdateState();
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.FREEZE_GREEN_GATE))
                            {
                                Item.ExtraData = GreenTeam.Count.ToString();
                                Item.UpdateState();
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.FREEZE_YELLOW_GATE))
                            {
                                Item.ExtraData = YellowTeam.Count.ToString();
                                Item.UpdateState();
                            }
                        }
                        break;
                    }
            }
        }

        public void OnUserLeave(RoomUser user)
        {
            if (user.Team.Equals(TEAM.BLUE) && BlueTeam.Contains(user))
            {
                BlueTeam.Remove(user);
            }
            else if (user.Team.Equals(TEAM.RED) && RedTeam.Contains(user))
            {
                RedTeam.Remove(user);
            }
            else if (user.Team.Equals(TEAM.YELLOW) && YellowTeam.Contains(user))
            {
                YellowTeam.Remove(user);
            }
            else if (user.Team.Equals(TEAM.GREEN) && GreenTeam.Contains(user))
            {
                GreenTeam.Remove(user);
            }

            switch (Game.ToLower())
            {
                case "banzai":
                    {
                        Room room = user.GetClient().GetHabbo().CurrentRoom;
                        if (room == null)
                        {
                            return;
                        }

                        foreach (Item Item in room.GetRoomItemHandler().GetFloor.ToList())
                        {
                            if (Item == null)
                            {
                                continue;
                            }

                            if (Item.GetBaseItem().InteractionType.Equals(InteractionType.banzaigateblue))
                            {
                                Item.ExtraData = BlueTeam.Count.ToString();
                                Item.UpdateState();
                                if (room.GetGameMap().GameMap[Item.GetX, Item.GetY] == 0)
                                {
                                    foreach (RoomUser sser in room.GetGameMap().GetRoomUsers(new Point(Item.GetX, Item.GetY)))
                                    {
                                        sser.SqState = 1;
                                    }

                                    room.GetGameMap().GameMap[Item.GetX, Item.GetY] = 1;
                                }
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.banzaigatered))
                            {
                                Item.ExtraData = RedTeam.Count.ToString();
                                Item.UpdateState();
                                if (room.GetGameMap().GameMap[Item.GetX, Item.GetY] == 0)
                                {
                                    foreach (RoomUser sser in room.GetGameMap().GetRoomUsers(new Point(Item.GetX, Item.GetY)))
                                    {
                                        sser.SqState = 1;
                                    }

                                    room.GetGameMap().GameMap[Item.GetX, Item.GetY] = 1;
                                }
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.banzaigategreen))
                            {
                                Item.ExtraData = GreenTeam.Count.ToString();
                                Item.UpdateState();
                                if (room.GetGameMap().GameMap[Item.GetX, Item.GetY] == 0)
                                {
                                    foreach (RoomUser sser in room.GetGameMap().GetRoomUsers(new Point(Item.GetX, Item.GetY)))
                                    {
                                        sser.SqState = 1;
                                    }

                                    room.GetGameMap().GameMap[Item.GetX, Item.GetY] = 1;
                                }
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.banzaigateyellow))
                            {
                                Item.ExtraData = YellowTeam.Count.ToString();
                                Item.UpdateState();
                                if (room.GetGameMap().GameMap[Item.GetX, Item.GetY] == 0)
                                {
                                    foreach (RoomUser sser in room.GetGameMap().GetRoomUsers(new Point(Item.GetX, Item.GetY)))
                                    {
                                        sser.SqState = 1;
                                    }

                                    room.GetGameMap().GameMap[Item.GetX, Item.GetY] = 1;
                                }
                            }
                        }
                        break;
                    }
                case "freeze":
                    {
                        Room room = user.GetClient().GetHabbo().CurrentRoom;
                        if (room == null)
                        {
                            return;
                        }

                        foreach (Item Item in room.GetRoomItemHandler().GetFloor.ToList())
                        {
                            if (Item == null)
                            {
                                continue;
                            }

                            if (Item.GetBaseItem().InteractionType.Equals(InteractionType.FREEZE_BLUE_GATE))
                            {
                                Item.ExtraData = BlueTeam.Count.ToString();
                                Item.UpdateState();
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.FREEZE_RED_GATE))
                            {
                                Item.ExtraData = RedTeam.Count.ToString();
                                Item.UpdateState();
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.FREEZE_GREEN_GATE))
                            {
                                Item.ExtraData = GreenTeam.Count.ToString();
                                Item.UpdateState();
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.FREEZE_YELLOW_GATE))
                            {
                                Item.ExtraData = YellowTeam.Count.ToString();
                                Item.UpdateState();
                            }
                        }
                        break;
                    }
            }
        }
    }
}