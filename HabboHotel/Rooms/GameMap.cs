using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Core;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Groups;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Items.RentableSpaces;
using StarBlue.HabboHotel.Items.Wired.Util;
using StarBlue.HabboHotel.Rooms.Games.Teams;
using StarBlue.HabboHotel.Rooms.PathFinding;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms
{
    public class Gamemap
    {
        private Room _room;
        private byte[,] mGameMap;

        private RoomModel mStaticModel;
        private byte[,] mUserItemEffect;
        private double[,] mItemHeightMap;
        private DynamicRoomModel mDynamicModel;
        private ConcurrentDictionary<Point, List<int>> mCoordinatedItems;
        private ConcurrentDictionary<Point, List<int>> mCoordinatedMagicTileItems;
        public ConcurrentDictionary<Point, List<RoomUser>> userMap;

        public Gamemap(Room room)
        {
            _room = room;
            mStaticModel = StarBlueServer.GetGame().GetRoomManager().GetModel(room.RoomData.ModelName);
            if (mStaticModel == null)
            {
                StarBlueServer.GetGame().GetRoomManager().LoadModel(room.RoomData.ModelName);
                mStaticModel = StarBlueServer.GetGame().GetRoomManager().GetModel(room.RoomData.ModelName);
            }

            if (mStaticModel == null)
            {
                return;
            }

            mDynamicModel = new DynamicRoomModel(mStaticModel);

            mCoordinatedItems = new ConcurrentDictionary<Point, List<int>>();
            mCoordinatedMagicTileItems = new ConcurrentDictionary<Point, List<int>>();

            mGameMap = new byte[Model.MapSizeX, Model.MapSizeY];
            mItemHeightMap = new double[Model.MapSizeX, Model.MapSizeY];

            userMap = new ConcurrentDictionary<Point, List<RoomUser>>();
        }

        public void AddUserToMap(RoomUser user, Point coord)
        {
            if (userMap.ContainsKey(coord))
            {
                userMap[coord].Add(user);
            }
            else
            {
                List<RoomUser> users = new List<RoomUser>
                {
                    user
                };
                userMap.TryAdd(coord, users);
            }
        }

        public bool SquareIsTeleporter(int X, int Y)
        {
            if (SquareHasUsers(X, Y))
            {
                return true;
            }
            else if (mGameMap[X, Y] == 0)
            {
                return false;
            }
            return true;
        }

        public void TeleportToItem(RoomUser user, Item item)
        {
            if (item == null || user == null)
                return;

            GameMap[user.SetStep ? user.SetX : user.X, user.SetStep ? user.SetY : user.Y] = user.SqState;
            UpdateUserMovement(new Point(user.Coordinate.X, user.Coordinate.Y), new Point(item.Coordinate.X, item.Coordinate.Y), user);
            user.X = item.GetX;
            user.Y = item.GetY;
            user.Z = item.GetZ;

            user.SqState = GameMap[item.GetX, item.GetY];
            GameMap[user.X, user.Y] = 1;
            user.RotBody = item.Rotation;
            user.RotHead = item.Rotation;

            user.GoalX = user.X;
            user.GoalY = user.Y;
            user.SetStep = false;
            user.IsWalking = false;
            user.UpdateNeeded = true;
        }

        public bool SquareHasFurniNoWalkable(int X, int Y, bool Override)
        {
            if (Override)
                return false;

            var point = new Point(X, Y);
            if (!mCoordinatedItems.ContainsKey(point))
                return false;

            var list = GetItemsFromIds((List<int>)mCoordinatedItems[point]);
            return list.Any(Item => Item.Coordinate.X == point.X && Item.Coordinate.Y == point.Y && !Item.GetBaseItem().Walkable && !Item.GetBaseItem().IsSeat && Item.Data.InteractionType != InteractionType.GATE && Item.Data.InteractionType != InteractionType.GUILD_GATE);

        }

        public void UpdateUserMovement(Point oldCoord, Point newCoord, RoomUser user)
        {
            RemoveUserFromMap(user, oldCoord);
            AddUserToMap(user, newCoord);
        }

        public bool NextSquareHasUser(int x, int y, RoomUser User)
        {
            int newx = 0, newy = 0;

            if (User.RotBody == 0)
            {
                newx = x;
                newy = y - 1;
            }
            if (User.RotBody == 1)
            {
                newx = x + 1;
                newy = y - 1;
            }
            if (User.RotBody == 2)
            {
                newx = x + 1;
                newy = y;
            }
            if (User.RotBody == 3)
            {
                newx = x + 1;
                newy = y + 1;

            }
            if (User.RotBody == 4)
            {
                newx = x;
                newy = y + 1;
            }
            if (User.RotBody == 5)
            {
                newx = x - 1;
                newy = y + 1;

            }
            if (User.RotBody == 6)
            {
                newx = x - 1;
                newy = y;
            }
            if (User.RotBody == 7)
            {
                newx = x - 1;
                newy = y - 1;
            }
            if (SquareHasUsers(newx, newy))
            {
                mGameMap[newx, newy] = 0;
            }
            return false;
        }

        public void RemoveUserFromMap(RoomUser user, Point coord)
        {
            if (userMap.ContainsKey(coord))
            {
                userMap[coord].RemoveAll(x => x != null && x.VirtualId == user.VirtualId);
            }
        }

        public bool MapGotUser(Point coord)
        {
            return (GetRoomUsers(coord).Count > 0);
        }

        public List<RoomUser> GetRoomUsers(Point coord)
        {
            if (userMap.ContainsKey(coord))
            {
                return userMap[coord];
            }
            else
            {
                return new List<RoomUser>();
            }
        }

        public Point getRandomWalkableSquare()
        {
            List<Point> walkableSquares = new List<Point>();
            for (int y = 0; y < mGameMap.GetUpperBound(1); y++)
            {
                for (int x = 0; x < mGameMap.GetUpperBound(0); x++)
                {
                    if (mStaticModel.DoorX != x && mStaticModel.DoorY != y && mGameMap[x, y] == 1)
                    {
                        walkableSquares.Add(new Point(x, y));
                    }
                }
            }

            int RandomNumber = StarBlueServer.GetRandomNumber(0, walkableSquares.Count);
            int i = 0;

            foreach (Point coord in walkableSquares.ToList())
            {
                if (i == RandomNumber)
                {
                    return coord;
                }

                i++;
            }

            return new Point(0, 0);
        }


        public bool isInMap(int X, int Y)
        {
            List<Point> walkableSquares = new List<Point>();
            for (int y = 0; y < mGameMap.GetUpperBound(1); y++)
            {
                for (int x = 0; x < mGameMap.GetUpperBound(0); x++)
                {
                    if (mStaticModel.DoorX != x && mStaticModel.DoorY != y && mGameMap[x, y] == 1)
                    {
                        walkableSquares.Add(new Point(x, y));
                    }
                }
            }

            if (walkableSquares.Contains(new Point(X, Y)))
            {
                return true;
            }

            return false;
        }

        public void AddToMap(Item item)
        {
            AddItemToMap(item);
        }

        public void SetDefaultValue(int x, int y)
        {
            mGameMap[x, y] = 0;
            mUserItemEffect[x, y] = 0;
            mItemHeightMap[x, y] = 0.0;

            if (x == Model.DoorX && y == Model.DoorY)
            {
                mGameMap[x, y] = 3;
            }
            else if (Model.SqState[x, y] == SquareState.OPEN)
            {
                mGameMap[x, y] = 1;
            }
            else if (Model.SqState[x, y] == SquareState.SEAT)
            {
                mGameMap[x, y] = 2;
            }
        }

        public void updateMapForItem(Item item)
        {
            RemoveFromMap(item);
            AddToMap(item);
        }

        public void GenerateMaps(bool checkLines = true)
        {
            if (this.mCoordinatedItems.Count > 0)
                this.mCoordinatedItems.Clear();

            int MaxX = 0;
            int MaxY = 0;

            if (checkLines)
            {
                Item[] items = _room.GetRoomItemHandler().GetFloor.ToArray();
                foreach (Item item in items.ToList())
                {
                    if (item == null)
                        continue;

                    if (item.GetX > Model.MapSizeX && item.GetX > MaxX)
                        MaxX = item.GetX;
                    if (item.GetY > Model.MapSizeY && item.GetY > MaxY)
                        MaxY = item.GetY;
                }

                Array.Clear(items, 0, items.Length);
                items = null;
            }

            #region Dynamic game map handling

            if (MaxY > (Model.MapSizeY - 1) || MaxX > (Model.MapSizeX - 1))
            {
                if (MaxX < Model.MapSizeX)
                    MaxX = Model.MapSizeX;
                if (MaxY < Model.MapSizeY)
                    MaxY = Model.MapSizeY;

                Model.SetMapsize(MaxX + 7, MaxY + 7);
                GenerateMaps(false);
                return;
            }

            if (MaxX != StaticModel.MapSizeX || MaxY != StaticModel.MapSizeY)
            {
                mUserItemEffect = new byte[Model.MapSizeX, Model.MapSizeY];
                mGameMap = new byte[Model.MapSizeX, Model.MapSizeY];

                mItemHeightMap = new double[Model.MapSizeX, Model.MapSizeY];
                //if (modelRemap)
                //    Model.Generate(); //Clears model

                for (int line = 0; line < Model.MapSizeY; line++)
                {
                    for (int chr = 0; chr < Model.MapSizeX; chr++)
                    {
                        mGameMap[chr, line] = 0;
                        mUserItemEffect[chr, line] = 0;

                        if (chr == Model.DoorX && line == Model.DoorY)
                        {
                            mGameMap[chr, line] = 3;
                        }
                        else if (Model.SqState[chr, line] == SquareState.OPEN)
                        {
                            mGameMap[chr, line] = 1;
                        }
                        else if (Model.SqState[chr, line] == SquareState.SEAT)
                        {
                            mGameMap[chr, line] = 2;
                        }
                        else if (Model.SqState[chr, line] == SquareState.POOL)
                        {
                            mUserItemEffect[chr, line] = 6;
                        }
                    }
                }
            }
            #endregion

            #region Static game map handling

            else
            {
                //mGameMap
                //mUserItemEffect
                mUserItemEffect = new byte[Model.MapSizeX, Model.MapSizeY];
                mGameMap = new byte[Model.MapSizeX, Model.MapSizeY];


                mItemHeightMap = new double[Model.MapSizeX, Model.MapSizeY];
                //if (modelRemap)
                //    Model.Generate(); //Clears model

                for (int line = 0; line < Model.MapSizeY; line++)
                {
                    for (int chr = 0; chr < Model.MapSizeX; chr++)
                    {
                        mGameMap[chr, line] = 0;
                        mUserItemEffect[chr, line] = 0;

                        if (chr == Model.DoorX && line == Model.DoorY)
                        {
                            mGameMap[chr, line] = 3;
                        }
                        else if (Model.SqState[chr, line] == SquareState.OPEN)
                        {
                            mGameMap[chr, line] = 1;
                        }
                        else if (Model.SqState[chr, line] == SquareState.SEAT)
                        {
                            mGameMap[chr, line] = 2;
                        }
                        else if (Model.SqState[chr, line] == SquareState.POOL)
                        {
                            mUserItemEffect[chr, line] = 6;
                        }
                    }
                }
            }

            #endregion

            Item[] tmpItems = _room.GetRoomItemHandler().GetFloor.ToArray();
            foreach (Item Item in tmpItems.ToList())
            {
                if (Item == null)
                    continue;

                if (!AddItemToMap(Item))
                    continue;
            }
            Array.Clear(tmpItems, 0, tmpItems.Length);
            tmpItems = null;

            if (_room.RoomData.RoomBlockingEnabled == 0)
            {
                foreach (RoomUser user in _room.GetRoomUserManager().GetUserList().ToList())
                {
                    if (user == null)
                        continue;

                    user.SqState = mGameMap[user.SetStep ? user.SetX : user.X, user.SetStep ? user.SetY : user.Y];
                    mGameMap[user.SetStep ? user.SetX : user.X, user.SetStep ? user.SetY : user.Y] = 0;
                }
            }

            try
            {
                mGameMap[Model.DoorX, Model.DoorY] = 3;
            }
            catch { }
        }

        public bool ConstructMapForItem(Item Item, Point Coord)
        {
            try
            {
                if (Coord.X > (Model.MapSizeX - 1))
                {
                    Model.AddX();
                    GenerateMaps();
                    return false;
                }

                if (Coord.Y > (Model.MapSizeY - 1))
                {
                    Model.AddY();
                    GenerateMaps();
                    return false;
                }

                if (Model.SqState[Coord.X, Coord.Y] == SquareState.BLOCKED)
                {
                    Model.OpenSquare(Coord.X, Coord.Y, Item.GetZ);
                }
                if (mItemHeightMap[Coord.X, Coord.Y] <= Item.TotalHeight)
                {
                    mItemHeightMap[Coord.X, Coord.Y] = Item.TotalHeight - mDynamicModel.SqFloorHeight[Item.GetX, Item.GetY];
                    mUserItemEffect[Coord.X, Coord.Y] = 0;

                    switch (Item.GetBaseItem().InteractionType)
                    {
                        case InteractionType.POOL:
                            mUserItemEffect[Coord.X, Coord.Y] = 1;
                            break;
                        case InteractionType.NORMAL_SKATES:
                            mUserItemEffect[Coord.X, Coord.Y] = 2;
                            break;
                        case InteractionType.ICE_SKATES:
                            mUserItemEffect[Coord.X, Coord.Y] = 3;
                            break;
                        case InteractionType.lowpool:
                            mUserItemEffect[Coord.X, Coord.Y] = 4;
                            break;
                        case InteractionType.haloweenpool:
                            mUserItemEffect[Coord.X, Coord.Y] = 5;
                            break;
                        case InteractionType.SILLAGUIA:
                            mUserItemEffect[Coord.X, Coord.Y] = 7;
                            break;
                    }

                    //SwimHalloween
                    if (Item.GetBaseItem().Walkable)    // If this item is walkable and on the floor, allow users to walk here.
                    {
                        if (mGameMap[Coord.X, Coord.Y] != 3)
                            mGameMap[Coord.X, Coord.Y] = 1;
                    }
                    else if (Item.GetBaseItem().InteractionType == InteractionType.GATE && Item.ExtraData == "1")// If this item is a gate, open, and on the floor, allow users to walk here.
                    {
                        if (mGameMap[Coord.X, Coord.Y] != 3)
                            mGameMap[Coord.X, Coord.Y] = 1;
                    }
                    else if (Item.GetBaseItem().IsSeat || Item.GetBaseItem().InteractionType == InteractionType.BED || Item.GetBaseItem().InteractionType == InteractionType.TENT_SMALL)
                    {
                        mGameMap[Coord.X, Coord.Y] = 3;
                    }
                    else // Finally, if it's none of those, block the square.
                    {
                        if (mGameMap[Coord.X, Coord.Y] != 3)
                            mGameMap[Coord.X, Coord.Y] = 0;
                    }
                }

                // Set bad maps
                if (Item.GetBaseItem().InteractionType == InteractionType.BED || Item.GetBaseItem().InteractionType == InteractionType.TENT_SMALL)
                {
                    mGameMap[Coord.X, Coord.Y] = 3;
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e.ToString());
            }
            return true;
        }

        public void AddCoordinatedItem(Item item, Point coord)
        {
            List<int> Items = new List<int>();

            if (!mCoordinatedItems.TryGetValue(coord, out Items))
            {
                Items = new List<int>();

                if (!Items.Contains(item.Id))
                {
                    Items.Add(item.Id);
                }

                if (!mCoordinatedItems.ContainsKey(coord))
                {
                    mCoordinatedItems.TryAdd(coord, Items);
                }
            }
            else
            {
                if (!Items.Contains(item.Id))
                {
                    Items.Add(item.Id);
                    mCoordinatedItems[coord] = Items;
                }
            }
        }

        public void AddCoordinatedMagicTileItem(Item item, Point coord)
        {
            List<int> Items = new List<int>(); //mCoordinatedItems[CoordForItem];

            if (!mCoordinatedMagicTileItems.TryGetValue(coord, out Items))
            {
                Items = new List<int>();

                if (!Items.Contains(item.Id))
                {
                    Items.Add(item.Id);
                }

                if (!mCoordinatedMagicTileItems.ContainsKey(coord))
                {
                    mCoordinatedMagicTileItems.TryAdd(coord, Items);
                }
            }
            else
            {
                if (!Items.Contains(item.Id))
                {
                    Items.Add(item.Id);
                    mCoordinatedMagicTileItems[coord] = Items;
                }
            }
        }

        public List<Item> GetCoordinatedItems(Point coord)
        {
            Point point = new Point(coord.X, coord.Y);
            List<Item> Items = new List<Item>();

            if (mCoordinatedItems.ContainsKey(point))
            {
                List<int> Ids = mCoordinatedItems[point];
                Items = GetItemsFromIds(Ids);
                return Items;
            }

            return new List<Item>();
        }

        public List<Item> GetCoordinatedMagicTileItems(Point coord)
        {
            Point point = new Point(coord.X, coord.Y);
            List<Item> Items = new List<Item>();

            if (mCoordinatedMagicTileItems.ContainsKey(point))
            {
                List<int> Ids = mCoordinatedMagicTileItems[point];
                Items = GetItemsFromIds(Ids);
                return Items;
            }

            return new List<Item>();
        }

        public bool RemoveCoordinatedItem(Item item, Point coord)
        {
            Point point = new Point(coord.X, coord.Y);
            if (mCoordinatedItems != null && mCoordinatedItems.ContainsKey(point))
            {
                mCoordinatedItems[point].RemoveAll(x => x == item.Id);
                return true;
            }
            return false;
        }

        public bool RemoveCoordinatedMagicTileItem(Item item, Point coord)
        {
            Point point = new Point(coord.X, coord.Y);
            if (mCoordinatedMagicTileItems != null && mCoordinatedMagicTileItems.ContainsKey(point))
            {
                mCoordinatedMagicTileItems[point].RemoveAll(x => x == item.Id);
                return true;
            }
            return false;
        }

        private void AddSpecialItems(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.FOOTBALL_GATE:
                    //IsTrans = true;
                    _room.GetSoccer().RegisterGate(item);


                    string[] splittedExtraData = item.ExtraData.Split(':');

                    if (string.IsNullOrEmpty(item.ExtraData) || splittedExtraData.Length <= 1)
                    {
                        item.Gender = "M";
                        switch (item.team)
                        {
                            case TEAM.YELLOW:
                                item.Figure = "lg-275-93.hr-115-61.hd-207-14.ch-265-93.sh-305-62";
                                break;
                            case TEAM.RED:
                                item.Figure = "lg-275-96.hr-115-61.hd-180-3.ch-265-96.sh-305-62";
                                break;
                            case TEAM.GREEN:
                                item.Figure = "lg-275-102.hr-115-61.hd-180-3.ch-265-102.sh-305-62";
                                break;
                            case TEAM.BLUE:
                                item.Figure = "lg-275-108.hr-115-61.hd-180-3.ch-265-108.sh-305-62";
                                break;
                        }
                    }
                    else
                    {
                        item.Gender = splittedExtraData[0];
                        item.Figure = splittedExtraData[1];
                    }
                    break;

                case InteractionType.banzaifloor:
                    {
                        _room.GetBanzai().AddTile(item, item.Id);
                        break;
                    }

                case InteractionType.banzaipyramid:
                    {
                        _room.GetGameItemHandler().AddPyramid(item, item.Id);
                        break;
                    }

                case InteractionType.banzaitele:
                    {
                        _room.GetGameItemHandler().AddTeleport(item, item.Id);
                        item.ExtraData = "";
                        break;
                    }
                case InteractionType.banzaipuck:
                    {
                        _room.GetBanzai().AddPuck(item);
                        break;
                    }

                case InteractionType.FOOTBALL:
                    {
                        _room.GetSoccer().AddBall(item);
                        break;
                    }
                case InteractionType.FREEZE_TILE_BLOCK:
                    {
                        _room.GetFreeze().AddFreezeBlock(item);
                        break;
                    }
                case InteractionType.FREEZE_TILE:
                    {
                        _room.GetFreeze().AddFreezeTile(item);
                        break;
                    }
                case InteractionType.freezeexit:
                    {
                        _room.GetFreeze().AddExitTile(item);
                        break;
                    }
            }
        }

        private void RemoveSpecialItem(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.FOOTBALL_GATE:
                    _room.GetSoccer().UnRegisterGate(item);
                    break;
                case InteractionType.banzaifloor:
                    _room.GetBanzai().RemoveTile(item.Id);
                    break;
                case InteractionType.banzaipuck:
                    _room.GetBanzai().RemovePuck(item.Id);
                    break;
                case InteractionType.banzaipyramid:
                    _room.GetGameItemHandler().RemovePyramid(item.Id);
                    break;
                case InteractionType.banzaitele:
                    _room.GetGameItemHandler().RemoveTeleport(item.Id);
                    break;
                case InteractionType.FOOTBALL:
                    _room.GetSoccer().RemoveBall(item.Id);
                    break;
                case InteractionType.FREEZE_TILE:
                    _room.GetFreeze().RemoveFreezeTile(item.Id);
                    break;
                case InteractionType.FREEZE_TILE_BLOCK:
                    _room.GetFreeze().RemoveFreezeBlock(item.Id);
                    break;
                case InteractionType.freezeexit:
                    _room.GetFreeze().RemoveExitTile(item.Id);
                    break;
            }
        }

        public bool RemoveFromMap(Item item, bool handleGameItem)
        {
            if (handleGameItem)
            {
                RemoveSpecialItem(item);
            }

            if (_room.GotSoccer())
            {
                _room.GetSoccer().onGateRemove(item);
            }

            bool isRemoved = false;
            foreach (Point coord in item.GetCoords.ToList())
            {
                if (RemoveCoordinatedItem(item, coord))
                {
                    isRemoved = true;
                }
            }

            ConcurrentDictionary<Point, List<Item>> items = new ConcurrentDictionary<Point, List<Item>>();
            foreach (Point Tile in item.GetCoords.ToList())
            {
                Point point = new Point(Tile.X, Tile.Y);
                if (mCoordinatedItems.ContainsKey(point))
                {
                    List<int> Ids = mCoordinatedItems[point];
                    List<Item> __items = GetItemsFromIds(Ids);

                    if (!items.ContainsKey(Tile))
                    {
                        items.TryAdd(Tile, __items);
                    }
                }

                SetDefaultValue(Tile.X, Tile.Y);
            }

            foreach (Point Coord in items.Keys.ToList())
            {
                if (!items.ContainsKey(Coord))
                {
                    continue;
                }

                List<Item> SubItems = items[Coord];
                foreach (Item Item in SubItems.ToList())
                {
                    ConstructMapForItem(Item, Coord);
                }
            }


            items.Clear();

            return isRemoved;
        }

        public bool RemoveFromMap(Item item)
        {
            return RemoveFromMap(item, true);
        }

        public bool AddItemToMap(Item Item, bool handleGameItem, bool NewItem = true)
        {

            if (handleGameItem)
            {
                AddSpecialItems(Item);

                switch (Item.GetBaseItem().InteractionType)
                {
                    case InteractionType.FOOTBALL_GOAL_RED:
                    case InteractionType.footballcounterred:
                    case InteractionType.banzaiscorered:
                    case InteractionType.banzaigatered:
                    case InteractionType.freezeredcounter:
                    case InteractionType.FREEZE_RED_GATE:
                        {
                            if (!_room.GetRoomItemHandler().GetFloor.Contains(Item))
                            {
                                _room.GetGameManager().AddFurnitureToTeam(Item, TEAM.RED);
                            }

                            break;
                        }
                    case InteractionType.FOOTBALL_GOAL_GREEN:
                    case InteractionType.footballcountergreen:
                    case InteractionType.banzaiscoregreen:
                    case InteractionType.banzaigategreen:
                    case InteractionType.freezegreencounter:
                    case InteractionType.FREEZE_GREEN_GATE:
                        {
                            if (!_room.GetRoomItemHandler().GetFloor.Contains(Item))
                            {
                                _room.GetGameManager().AddFurnitureToTeam(Item, TEAM.GREEN);
                            }

                            break;
                        }
                    case InteractionType.FOOTBALL_GOAL_BLUE:
                    case InteractionType.footballcounterblue:
                    case InteractionType.banzaiscoreblue:
                    case InteractionType.banzaigateblue:
                    case InteractionType.freezebluecounter:
                    case InteractionType.FREEZE_BLUE_GATE:
                        {
                            if (!_room.GetRoomItemHandler().GetFloor.Contains(Item))
                            {
                                _room.GetGameManager().AddFurnitureToTeam(Item, TEAM.BLUE);
                            }

                            break;
                        }
                    case InteractionType.FOOTBALL_GOAL_YELLOW:
                    case InteractionType.footballcounteryellow:
                    case InteractionType.banzaiscoreyellow:
                    case InteractionType.banzaigateyellow:
                    case InteractionType.freezeyellowcounter:
                    case InteractionType.FREEZE_YELLOW_GATE:
                        {
                            if (!_room.GetRoomItemHandler().GetFloor.Contains(Item))
                            {
                                _room.GetGameManager().AddFurnitureToTeam(Item, TEAM.YELLOW);
                            }

                            break;
                        }
                    case InteractionType.freezeexit:
                        {
                            _room.GetFreeze().AddExitTile(Item);
                            break;
                        }
                    case InteractionType.ROLLER:
                        {
                            if (!_room.GetRoomItemHandler().GetRollers().Contains(Item))
                            {
                                _room.GetRoomItemHandler().TryAddRoller(Item.Id, Item);
                            }

                            break;
                        }
                }
            }

            if (Item.GetBaseItem().Type != 's')
            {
                return true;
            }

            foreach (Point coord in Item.GetCoords.ToList())
            {
                AddCoordinatedItem(Item, new Point(coord.X, coord.Y));
            }

            if (Item.GetX > (Model.MapSizeX - 1))
            {
                Model.AddX();
                GenerateMaps();
                return false;
            }

            if (Item.GetY > (Model.MapSizeY - 1))
            {
                Model.AddY();
                GenerateMaps();
                return false;
            }

            bool Return = true;

            foreach (Point coord in Item.GetCoords)
            {
                if (!ConstructMapForItem(Item, coord))
                {
                    Return = false;
                }
                else
                {
                    Return = true;
                }
            }

            return Return;
        }

        public bool CanWalk(int X, int Y, bool Override)
        {

            if (Override)
            {
                return true;
            }

            if (_room.GetRoomUserManager().GetUserForSquare(X, Y) != null && _room.RoomData.RoomBlockingEnabled == 0)
            {
                return false;
            }

            return true;
        }

        public bool AddItemToMap(Item Item, bool NewItem = true)
        {
            return AddItemToMap(Item, true, NewItem);
        }

        public bool ItemCanMove(Item Item, Point MoveTo)
        {
            List<ThreeDCoord> Points = GetAffectedTiles(Item.GetBaseItem().Length, Item.GetBaseItem().Width, MoveTo.X, MoveTo.Y, Item.Rotation).Values.ToList();

            if (Points == null || Points.Count == 0)
            {
                return true;
            }

            foreach (ThreeDCoord Coord in Points)
            {
                if (Coord.X >= Model.MapSizeX || Coord.Y >= Model.MapSizeY)
                {
                    return false;
                }

                if (!SquareIsOpen(Coord.X, Coord.Y, false))
                {
                    return false;
                }

                continue;
            }

            return true;
        }

        public byte GetFloorStatus(Point coord)
        {
            if (coord.X > mGameMap.GetUpperBound(0) || coord.Y > mGameMap.GetUpperBound(1))
            {
                return 1;
            }

            return mGameMap[coord.X, coord.Y];
        }

        public void SetFloorStatus(int X, int Y, byte Status)
        {
            mGameMap[X, Y] = Status;
        }

        public double GetHeightForSquareFromData(Point coord)
        {
            if (coord.X > mDynamicModel.SqFloorHeight.GetUpperBound(0) ||  coord.Y > mDynamicModel.SqFloorHeight.GetUpperBound(1) || coord.X < 0 || coord.Y < 0)
            {
                return 1;
            }

            return mDynamicModel.SqFloorHeight[coord.X, coord.Y];
        }

        public bool CanRollItemHere(int x, int y)
        {
            if (!ValidTile(x, y))
            {
                return false;
            }

            if (Model.SqState[x, y] == SquareState.BLOCKED)
            {
                return false;
            }

            return true;
        }

        public bool SquareIsOpen(int x, int y, bool pOverride)
        {
            if ((mDynamicModel.MapSizeX - 1) < x || (mDynamicModel.MapSizeY - 1) < y)
            {
                return false;
            }

            return CanWalk(mGameMap[x, y], pOverride);
        }

        public bool GetHighestItemForSquare(Point Square, out Item Item)
        {
            List<Item> Items = GetAllRoomItemForSquare(Square.X, Square.Y);
            Item = null;
            double HighestZ = -1;

            if (Items != null && Items.Count() > 0)
            {
                foreach (Item uItem in Items.ToList())
                {
                    if (uItem == null)
                    {
                        continue;
                    }

                    if (uItem.TotalHeight > HighestZ)
                    {
                        HighestZ = uItem.TotalHeight;
                        Item = uItem;
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public double GetHeightForSquare(Point Coord)
        {

            if (GetHighestItemForSquare(Coord, out Item rItem))
            {
                if (rItem != null)
                {
                    return rItem.TotalHeight;
                }
            }

            return 0.0;
        }

        public MovementDirection GetChaseMovement(int X, int Y, MovementDirection OldMouvement)
        {
            bool moveToLeft = true;
            bool moveToRight = true;
            bool moveToUp = true;
            bool moveToDown = true;
            bool moveToUpLeft = true;
            bool moveToUpRight = true;
            bool moveToDownLeft = true;
            bool moveToDownRight = true;

            for (int y = 0; y < Model.MapSizeY; y++)
            {
                for (int i = 0; i < Model.MapSizeX; i++)
                {
                    // Left
                    if (i == 1 && !CanRollItemHere(X - i, Y))
                        moveToLeft = false;
                    else if (moveToLeft && SquareHasUsers(X - i, Y))
                        return MovementDirection.LEFT;

                    // Right
                    if (i == 1 && !CanRollItemHere(X + i, Y))
                        moveToRight = false;
                    else if (moveToRight && SquareHasUsers(X + i, Y))
                        return MovementDirection.RIGHT;

                    // Up
                    if (i == 1 && !CanRollItemHere(X, Y - i))
                        moveToUp = false;
                    else if (moveToUp && SquareHasUsers(X, Y - i))
                        return MovementDirection.UP;

                    // Down
                    if (i == 1 && !CanRollItemHere(X, Y + i))
                        moveToDown = false;
                    else if (moveToDown && SquareHasUsers(X, Y + i))
                        return MovementDirection.DOWN;

                    if (i == 1 && !CanRollItemHere(X - i, Y - i))
                        moveToUpLeft = false;
                    else if (moveToUpLeft && SquareHasUsers(X - i, Y - i))
                        return MovementDirection.UP_LEFT;

                    if (i == 1 && !CanRollItemHere(X + i, Y - i))
                        moveToUpRight = false;
                    else if (moveToUpRight && SquareHasUsers(X + i, Y - i))
                        return MovementDirection.UP_RIGHT;

                    if (i == 1 && !CanRollItemHere(X - i, Y + i))
                        moveToDownLeft = false;
                    else if (moveToDownLeft && SquareHasUsers(X - i, Y + i))
                        return MovementDirection.DOWN_LEFT;

                    if (i == 1 && !CanRollItemHere(X + i, Y + i))
                        moveToDownRight = false;
                    else if (moveToDownRight && SquareHasUsers(X + i, Y + i))
                        return MovementDirection.DOWN_RIGHT;

                    // Breaking bucle
                    if (i == 1 && !moveToLeft && !moveToRight && !moveToUp && !moveToDown && !moveToDownRight && !moveToDownLeft && !moveToUpLeft && !moveToUpRight)
                        return MovementDirection.NONE;
                }
            }

            List<MovementDirection> movements = new List<MovementDirection>();
            if (moveToLeft && OldMouvement != MovementDirection.RIGHT)
                movements.Add(MovementDirection.LEFT);
            if (moveToRight && OldMouvement != MovementDirection.LEFT)
                movements.Add(MovementDirection.RIGHT);
            if (moveToUp && OldMouvement != MovementDirection.DOWN)
                movements.Add(MovementDirection.UP);
            if (moveToDown && OldMouvement != MovementDirection.UP)
                movements.Add(MovementDirection.DOWN);
            if (moveToUpLeft && OldMouvement != MovementDirection.DOWN_LEFT)
                movements.Add(MovementDirection.UP_LEFT);
            if (moveToUpRight && OldMouvement != MovementDirection.DOWN_RIGHT)
                movements.Add(MovementDirection.UP_RIGHT);
            if (moveToDownLeft && OldMouvement != MovementDirection.UP_LEFT)
                movements.Add(MovementDirection.DOWN_LEFT);
            if (moveToDownRight && OldMouvement != MovementDirection.UP_RIGHT)
                movements.Add(MovementDirection.DOWN_RIGHT);

            if (movements.Count > 0)
                return movements[new Random().Next(0, movements.Count)];
            else
            {
                if (moveToLeft && OldMouvement == MovementDirection.LEFT)
                    return MovementDirection.LEFT;
                if (moveToRight && OldMouvement == MovementDirection.RIGHT)
                    return MovementDirection.RIGHT;
                if (moveToUp && OldMouvement == MovementDirection.UP)
                    return MovementDirection.UP;
                if (moveToDown && OldMouvement == MovementDirection.DOWN)
                    return MovementDirection.DOWN;
                if (moveToDownLeft && OldMouvement == MovementDirection.DOWN_LEFT)
                    return MovementDirection.DOWN_LEFT;
                if (moveToDownRight && OldMouvement == MovementDirection.DOWN_RIGHT)
                    return MovementDirection.DOWN_RIGHT;
                if (moveToUpLeft && OldMouvement == MovementDirection.UP_LEFT)
                    return MovementDirection.UP_LEFT;
                if (moveToUpRight && OldMouvement == MovementDirection.UP_RIGHT)
                    return MovementDirection.UP_RIGHT;
            }

            List<MovementDirection> movements2 = new List<MovementDirection>();
            if (moveToLeft)
                movements2.Add(MovementDirection.LEFT);
            if (moveToRight)
                movements2.Add(MovementDirection.RIGHT);
            if (moveToUp)
                movements2.Add(MovementDirection.UP);
            if (moveToDown)
                movements2.Add(MovementDirection.DOWN);
            if (moveToUpLeft)
                movements2.Add(MovementDirection.UP_LEFT);
            if (moveToUpRight)
                movements2.Add(MovementDirection.UP_RIGHT);
            if (moveToDownLeft)
                movements2.Add(MovementDirection.DOWN_LEFT);
            if (moveToDownRight)
                movements2.Add(MovementDirection.DOWN_RIGHT);

            if (movements2.Count > 0)
                return movements2[new Random().Next(0, movements2.Count)];

            return MovementDirection.NONE;
        }

        public MovementDirection GetEscapeMovement(int X, int Y, MovementDirection OldMouvement)
        {
            bool moveToLeft = true;
            bool moveToRight = true;
            bool moveToUp = true;
            bool moveToDown = true;
            bool moveToUpLeft = true;
            bool moveToUpRight = true;
            bool moveToDownLeft = true;
            bool moveToDownRight = true;

            for (int y = 0; y < Model.MapSizeY; y++)
            {
                for (int i = 0; i < Model.MapSizeX; i++)
                {
                    // Left
                    if (i == 1 && !CanRollItemHere(X - i, Y) || SquareHasUsers(X - i, Y) && i == 1)
                        moveToLeft = false;
                    else if (moveToLeft && SquareHasUsers(X - i, Y))
                        moveToLeft = false;

                    // Right
                    if (i == 1 && !CanRollItemHere(X + i, Y) || SquareHasUsers(X + i, Y) && i == 1)
                        moveToRight = false;
                    else if (moveToRight && SquareHasUsers(X + i, Y))
                        moveToRight = false;

                    // Up
                    if (i == 1 && !CanRollItemHere(X, Y - i) || SquareHasUsers(X, Y - i) && i == 1)
                        moveToUp = false;
                    else if (moveToUp && SquareHasUsers(X, Y - i))
                        moveToUp = false;

                    // Down
                    if (i == 1 && !CanRollItemHere(X, Y + i) || SquareHasUsers(X, Y + i) && i == 1)
                        moveToDown = false;
                    else if (moveToDown && SquareHasUsers(X, Y + i))
                        moveToDown = false;

                    if (i == 1 && !CanRollItemHere(X - i, Y + i) || SquareHasUsers(X - i, Y + i) && i == 1)
                        moveToDownLeft = false;
                    else if (moveToDownLeft && SquareHasUsers(X - i, Y + i))
                        moveToDownLeft = false;

                    if (i == 1 && !CanRollItemHere(X + i, Y + i) || SquareHasUsers(X + i, Y + i) && i == 1)
                        moveToDownRight = false;
                    else if (moveToDownRight && SquareHasUsers(X + i, Y + i))
                        moveToDownRight = false;

                    if (i == 1 && !CanRollItemHere(X - i, Y - i) || SquareHasUsers(X - i, Y - i) && i == 1)
                        moveToUpLeft = false;
                    else if (moveToUpLeft && SquareHasUsers(X - i, Y - i))
                        moveToUpLeft = false;

                    if (i == 1 && !CanRollItemHere(X + i, Y - i) || SquareHasUsers(X + i, Y - i) && i == 1)
                        moveToUpRight = false;
                    else if (moveToUpRight && SquareHasUsers(X + i, Y - i))
                        moveToUpRight = false;

                    // Breaking bucle
                    if (i == 1 && !moveToLeft && !moveToRight && !moveToUp && !moveToDown)
                        return MovementDirection.NONE;
                }
            }

            List<MovementDirection> movements = new List<MovementDirection>();
            if (moveToLeft && OldMouvement != MovementDirection.RIGHT)
                movements.Add(MovementDirection.LEFT);
            if (moveToRight && OldMouvement != MovementDirection.LEFT)
                movements.Add(MovementDirection.RIGHT);
            if (moveToUp && OldMouvement != MovementDirection.DOWN)
                movements.Add(MovementDirection.UP);
            if (moveToDown && OldMouvement != MovementDirection.UP)
                movements.Add(MovementDirection.DOWN);
            if (moveToUpLeft && OldMouvement != MovementDirection.DOWN_LEFT)
                movements.Add(MovementDirection.UP_LEFT);
            if (moveToUpRight && OldMouvement != MovementDirection.DOWN_RIGHT)
                movements.Add(MovementDirection.UP_RIGHT);
            if (moveToDownLeft && OldMouvement != MovementDirection.UP_LEFT)
                movements.Add(MovementDirection.DOWN_LEFT);
            if (moveToDownRight && OldMouvement != MovementDirection.UP_RIGHT)
                movements.Add(MovementDirection.DOWN_RIGHT);

            if (movements.Count > 0)
                return movements[new Random().Next(0, movements.Count)];
            else
            {
                if (moveToLeft && OldMouvement == MovementDirection.LEFT)
                    return MovementDirection.LEFT;
                if (moveToRight && OldMouvement == MovementDirection.RIGHT)
                    return MovementDirection.RIGHT;
                if (moveToUp && OldMouvement == MovementDirection.UP)
                    return MovementDirection.UP;
                if (moveToDown && OldMouvement == MovementDirection.DOWN)
                    return MovementDirection.DOWN;
                if (moveToDownLeft && OldMouvement == MovementDirection.DOWN_LEFT)
                    return MovementDirection.DOWN_LEFT;
                if (moveToDownRight && OldMouvement == MovementDirection.DOWN_RIGHT)
                    return MovementDirection.DOWN_RIGHT;
                if (moveToUpLeft && OldMouvement == MovementDirection.UP_LEFT)
                    return MovementDirection.UP_LEFT;
                if (moveToUpRight && OldMouvement == MovementDirection.UP_RIGHT)
                    return MovementDirection.UP_RIGHT;
            }

            List<MovementDirection> movements2 = new List<MovementDirection>();
            if (moveToLeft)
                movements2.Add(MovementDirection.LEFT);
            if (moveToRight)
                movements2.Add(MovementDirection.RIGHT);
            if (moveToUp)
                movements2.Add(MovementDirection.UP);
            if (moveToDown)
                movements2.Add(MovementDirection.DOWN);
            if (moveToUpLeft)
                movements2.Add(MovementDirection.UP_LEFT);
            if (moveToUpRight)
                movements2.Add(MovementDirection.UP_RIGHT);
            if (moveToDownLeft)
                movements2.Add(MovementDirection.DOWN_LEFT);
            if (moveToDownRight)
                movements2.Add(MovementDirection.DOWN_RIGHT);

            if (movements2.Count > 0)
                return movements2[new Random().Next(0, movements2.Count)];

            return MovementDirection.NONE;
        }

        public bool IsValidMovement(int CoordX, int CoordY)
        {
            if (CoordX < 0 || CoordY < 0 || CoordX >= Model.MapSizeX || CoordY >= Model.MapSizeY)
            {
                return false;
            }

            if (SquareHasUsers(CoordX, CoordY))
            {
                return false;
            }

            if (GetCoordinatedItems(new Point(CoordX, CoordY)).Count > 0 && !SquareIsOpen(CoordX, CoordY, false))
            {
                return false;
            }

            return Model.SqState[CoordX, CoordY] == SquareState.OPEN;
        }

        public bool IsValidStep2(RoomUser User, Vector2D From, Vector2D To, bool EndOfPath, bool Override)
        {
            if (User == null)
                return false;

            if (!ValidTile(To.X, To.Y))
                return false;

            if (Override)
                return true;

            /*
             * 0 = blocked
             * 1 = open
             * 2 = last step
             * 3 = door
             * */


            RoomUser Userx = _room.GetRoomUserManager().GetUserForSquare(To.X, To.Y);
            if (Userx != null && _room.RoomData.RoomBlockingEnabled == 0)
                return false;

            List<Item> Items = _room.GetGameMap().GetAllRoomItemForSquare(To.X, To.Y);
            if (Items.Count > 0)
            {
                var HasGate = Items.Where(x => x.GetBaseItem().InteractionType == InteractionType.GATE).FirstOrDefault();
                if ((HasGate != null && HasGate.ExtraData == "0") || (Userx != null && Userx.SqState == 3))
                {
                    return false;
                }

                var HasGroupGate = Items.Any(x => x.GetBaseItem().InteractionType == InteractionType.GUILD_GATE);
                if (HasGroupGate)
                {
                    var I = Items.FirstOrDefault(x => x.GetBaseItem().InteractionType == InteractionType.GUILD_GATE);
                    if (I != null)
                    {
                        Group gp = null;
                        var Group = StarBlueServer.GetGame().GetGroupManager().TryGetGroup(I.GroupId, out gp);
                        if (gp == null)
                            return false;

                        if (User.GetClient() == null || User.GetClient().GetHabbo() == null)
                            return false;

                        if (gp.IsMember(User.GetClient().GetHabbo().Id))
                        {
                            I.InteractingUser = User.GetClient().GetHabbo().Id;
                            I.ExtraData = "1";
                            I.UpdateState(false, true);

                            I.RequestUpdate(4, true);

                            return true;
                        }
                        if (User.Path.Count > 0)
                            User.Path.Clear();
                        User.PathRecalcNeeded = false;
                        return false;
                    }
                }
            }

            bool Chair = false;
            double HighestZ = -1;
            foreach (Item Item in Items.ToList())
            {
                if (Item == null)
                    continue;

                if (Item.GetZ < HighestZ)
                {
                    Chair = false;
                    continue;
                }

                HighestZ = Item.GetZ;
                if (Item.GetBaseItem().IsSeat)
                    Chair = true;
            }

            if ((mGameMap[To.X, To.Y] == 3 && !EndOfPath && !Chair) || (mGameMap[To.X, To.Y] == 0) || (mGameMap[To.X, To.Y] == 2 && !EndOfPath))
            {
                if (User.Path.Count > 0)
                    User.Path.Clear();
                User.PathRecalcNeeded = true;
            }

            double HeightDiff = SqAbsoluteHeight(To.X, To.Y) - SqAbsoluteHeight(From.X, From.Y);
            if (HeightDiff > 1.5 && !User.RidingHorse)
                return false;

            if (Userx != null)
            {
                if (!Userx.IsWalking && EndOfPath)
                    return false;
            }
            return true;
        }

        public bool IsValidStep(RoomUser User, Vector2D From, Vector2D To, bool EndOfPath, bool Override, bool DiagonalEnabled = false, bool DiagMovement = false, bool Roller = false)
        {
            if (!ValidTile(To.X, To.Y))
                return false;

            if (Override)
                return true;

            if (DiagMovement && !DiagonalEnabled)
            {
                int XValue = To.X - From.X;
                int YValue = To.Y - From.Y;

                if (XValue == -1 && YValue == -1)
                {
                    if (mGameMap[To.X + 1, To.Y] != 1 && mGameMap[To.X, To.Y + 1] != 1)
                        return false;
                }
                else if (XValue == 1 && YValue == -1)
                {
                    if (mGameMap[To.X - 1, To.Y] != 1 && mGameMap[To.X, To.Y + 1] != 1)
                        return false;
                }
                else if (XValue == 1 && YValue == 1)
                {
                    if (mGameMap[To.X - 1, To.Y] != 1 && mGameMap[To.X, To.Y - 1] != 1)
                        return false;
                }
                else if (XValue == -1 && YValue == 1)
                {
                    if (mGameMap[To.X + 1, To.Y] != 1 && mGameMap[To.X, To.Y - 1] != 1)
                        return false;
                }
            }

            RoomUser Userx = _room.GetRoomUserManager().GetUserForSquare(To.X, To.Y);
            if (_room.RoomData.RoomBlockingEnabled == 0 && Userx != null)
                return false;

            List<Item> Items = _room.GetGameMap().GetAllRoomItemForSquare(To.X, To.Y);
            if (Items.Count > 0)
            {
                var HasGate = Items.Where(x => x.GetBaseItem().InteractionType == InteractionType.GATE).FirstOrDefault();
                if ((HasGate != null && HasGate.ExtraData == "0") || (Userx != null && Userx.SqState == 3))
                {
                    return false;
                }

                var HasGroupGate = Items.Any(x => x.GetBaseItem().InteractionType == InteractionType.GUILD_GATE);
                if (HasGroupGate)
                {
                    var I = Items.FirstOrDefault(x => x.GetBaseItem().InteractionType == InteractionType.GUILD_GATE);
                    if (I != null)
                    {
                        Group gp = null;
                        var Group = StarBlueServer.GetGame().GetGroupManager().TryGetGroup(I.GroupId, out gp);
                        if (gp == null)
                            return false;

                        if (User.GetClient() == null || User.GetClient().GetHabbo() == null)
                            return false;

                        if (gp.IsMember(User.GetClient().GetHabbo().Id))
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }

            if ((mGameMap[To.X, To.Y] == 3 && !EndOfPath) || (mGameMap[To.X, To.Y] == 0) || (mGameMap[To.X, To.Y] == 2 && !EndOfPath))
                return false;

            if (!Roller)
            {
                double HeightDiff = SqAbsoluteHeight(To.X, To.Y) - SqAbsoluteHeight(From.X, From.Y);
                if (HeightDiff > 1.5)
                    return false;
            }

            return true;
        }

        public bool TileIsWalkable(int pX, int pY, bool isUser, RoomUser User, bool endPath = false)
        {
            if (!isUser)
            {
                if (SquareHasUsers(pX, pY))
                    return false;

                if (!ValidTile(pX, pY) || GameMap[pX, pY] != 1)
                    return false;
            }
            else
            {
                if (SquareHasUsers(pX, pY))
                {
                    RoomUser Userx = _room.GetRoomUserManager().GetUserForSquare(pX, pY);
                    if (endPath)
                        return true;//??keycode

                    if (Userx.IsWalking && User.IsWalking)
                    {
                        return true;
                    }
                    if (_room.RoomData.RoomBlockingEnabled == 0 && !(_room.GetGameMap().Model.DoorX == pX && _room.GetGameMap().Model.DoorY == pY))
                        return false;
                }

                if (!ValidTile(pX, pY))
                    return false;

                if (GameMap[pX, pY] == 0)
                    return false;
            }
            return Model.SqState[pX, pY] == SquareState.OPEN;
        }

        public static bool CanWalk(byte pState, bool pOverride)
        {
            if (!pOverride)
            {
                if (pState == 3)
                {
                    return true;
                }

                if (pState == 1)
                {
                    return true;
                }

                return false;
            }
            return true;
        }

        public List<Item> GetRoomItemForMinZ(int pX, int pY, double pZ)
        {
            List<Item> itemsToReturn = new List<Item>();
            Point coord = new Point(pX, pY);

            if (mCoordinatedItems.ContainsKey(coord))
            {
                List<Item> itemsFromSquare = GetItemsFromIds(mCoordinatedItems[coord]);
                foreach (Item item in itemsFromSquare)
                {
                    if (pZ < item.GetZ)
                    {
                        itemsToReturn.Add(item);
                    }
                }
            }

            return itemsToReturn;
        }

        public int outgoing_dir(int inconming_dir, bool ejey, bool ejex)
        {
            if (ejex && ejey)
            {
                if (inconming_dir == 1)
                {
                    return 5;
                }

                if (inconming_dir == 5)
                {
                    return 1;
                }

                if (inconming_dir == 7)
                {
                    return 3;
                }

                if (inconming_dir == 3)
                {
                    return 7;
                }
            }
            else if (ejex && !ejey)
            {
                if (inconming_dir == 6)
                {
                    return 2;
                }

                if (inconming_dir == 2)
                {
                    return 6;
                }

                if (inconming_dir == 7)
                {
                    return 1;
                }

                if (inconming_dir == 3)
                {
                    return 5;
                }

                if (inconming_dir == 5)
                {
                    return 3;
                }

                if (inconming_dir == 1)
                {
                    return 7;
                }
            }
            else if (!ejex && ejey)
            {
                if (inconming_dir == 4)
                {
                    return 0;
                }

                if (inconming_dir == 0)
                {
                    return 4;
                }

                if (inconming_dir == 5)
                {
                    return 7;
                }

                if (inconming_dir == 1)
                {
                    return 3;
                }

                if (inconming_dir == 3)
                {
                    return 1;
                }

                if (inconming_dir == 7)
                {
                    return 5;
                }
            }
            return -1;
        }

        public bool itemCanBePlacedHere(int x, int y, Item ball = null)
        {

            if (mDynamicModel.MapSizeX - 1 < x || mDynamicModel.MapSizeY - 1 < y || (x == mDynamicModel.DoorX && y == mDynamicModel.DoorY))
            {
                if (ball != null)
                {
                    if (mDynamicModel.MapSizeX - 1 < x && mDynamicModel.MapSizeY - 1 < y)
                    {
                        ball.ejey = true;
                        ball.ejex = true;
                    }
                    else if (mDynamicModel.MapSizeX - 1 < x && !(mDynamicModel.MapSizeY - 1 < y))
                    {
                        ball.ejex = true;
                        ball.ejey = false;
                    }
                    else if (mDynamicModel.MapSizeY - 1 < y && !(mDynamicModel.MapSizeX - 1 < x))
                    {
                        ball.ejey = true;
                        ball.ejex = false;
                    }

                }

                return false;
            }
            if (mGameMap[x, y] != 1 && ball != null)
            {
                bool done = false;
                int incoming_dir = ball.BallDireccion;

                try
                {
                    if (mGameMap[x - 1, y] == 1 && !done) //ya
                    {
                        done = true;
                        ball.ejex = true;
                        ball.ejey = false;
                        ball.BallDireccion = outgoing_dir(incoming_dir, ball.ejey, ball.ejex);
                    }
                }
                catch (Exception)
                {
                    ball.ejex = true;
                    ball.ejey = false;
                    ball.BallDireccion = outgoing_dir(incoming_dir, ball.ejey, ball.ejex);
                    return false;
                }

                try
                {
                    if (mGameMap[x + 1, y] == 1 && !done) //ya
                    {
                        done = true;
                        ball.ejex = true;
                        ball.ejey = false;
                        ball.BallDireccion = outgoing_dir(incoming_dir, ball.ejey, ball.ejex);
                    }

                }
                catch (Exception)
                {
                    ball.ejex = true;
                    ball.ejey = false;
                    ball.BallDireccion = outgoing_dir(incoming_dir, ball.ejey, ball.ejex);
                    return false;
                }

                try
                {
                    if (mGameMap[x, y + 1] == 1 && !done) //ya
                    {
                        done = true;
                        ball.ejex = false;
                        ball.ejey = true;
                        ball.BallDireccion = outgoing_dir(incoming_dir, ball.ejey, ball.ejex);
                    }
                }
                catch (Exception)
                {
                    ball.ejex = false;
                    ball.ejey = true;
                    ball.BallDireccion = outgoing_dir(incoming_dir, ball.ejey, ball.ejex);
                    return false;
                }

                try
                {
                    if (mGameMap[x, y - 1] == 1 && !done) //ya
                    {
                        done = true;
                        ball.ejex = false;
                        ball.ejey = true;
                        ball.BallDireccion = outgoing_dir(incoming_dir, ball.ejey, ball.ejex);
                    }

                }
                catch (Exception)
                {
                    ball.ejex = false;
                    ball.ejey = true;
                    ball.BallDireccion = outgoing_dir(incoming_dir, ball.ejey, ball.ejex);
                    return false;
                }

                try
                {
                    if (mGameMap[x + 1, y + 1] == 1 && !done)
                    {
                        done = true;
                        ball.ejex = true;
                        ball.ejey = true;
                        ball.BallDireccion = outgoing_dir(incoming_dir, ball.ejey, ball.ejex);
                    }

                }
                catch (Exception)
                {
                    ball.ejex = true;
                    ball.ejey = true;
                    ball.BallDireccion = outgoing_dir(incoming_dir, ball.ejey, ball.ejex);
                    return false;

                }

                try
                {
                    if (mGameMap[x - 1, y - 1] == 1 && !done)
                    {
                        done = true;
                        ball.ejex = true;
                        ball.ejey = true;
                        ball.BallDireccion = outgoing_dir(incoming_dir, ball.ejey, ball.ejex);
                    }
                }
                catch (Exception)
                {
                    ball.ejex = true;
                    ball.ejey = true;
                    ball.BallDireccion = outgoing_dir(incoming_dir, ball.ejey, ball.ejex);
                    return false;
                }

                try
                {
                    if (mGameMap[x + 1, y - 1] == 1 && !done)
                    {
                        done = true;
                        ball.ejex = true;
                        ball.ejey = true;
                        ball.BallDireccion = outgoing_dir(incoming_dir, ball.ejey, ball.ejex);
                    }
                }
                catch (Exception)
                {
                    ball.ejex = true;
                    ball.ejey = true;
                    ball.BallDireccion = outgoing_dir(incoming_dir, ball.ejey, ball.ejex);
                    return false;
                }

                try
                {
                    if (mGameMap[x - 1, y + 1] == 1 && !done)
                    {
                        done = true;
                        ball.ejex = true;
                        ball.ejey = true;
                        ball.BallDireccion = outgoing_dir(incoming_dir, ball.ejey, ball.ejex);
                    }

                }
                catch (Exception)
                {
                    ball.ejex = true;
                    ball.ejey = true;
                    ball.BallDireccion = outgoing_dir(incoming_dir, ball.ejey, ball.ejex);
                    return false;
                }
            }

            return mGameMap[x, y] == 1;
        }

        public bool StackTable(int CoordX, int CoordY)
        {
            if (!ValidTile(CoordX, CoordY))
            {
                return false;
            }

            if (!itemCanBePlacedHere(CoordX, CoordY))
            {
                return false;
            }

            List<Item> Items = _room.GetGameMap().GetAllRoomItemForSquare(CoordX, CoordY);
            if (Items.Count > 0)
            {
                foreach (Item Item in Items)
                {
                    if (Item == null || Item.Data == null)
                    {
                        continue;
                    }

                    if (!Item.Data.Stackable)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public double SqAbsoluteHeight(int X, int Y)
        {
            Point Points = new Point(X, Y);


            if (mCoordinatedItems.TryGetValue(Points, out List<int> Ids))
            {
                List<Item> Items = GetItemsFromIds(Ids);

                return SqAbsoluteHeight(X, Y, Items);
            }
            else
            {
                return GetHeightForSquareFromData(new Point(X, Y));
            }
        }

        public static Item GetItemFromTileMaxHeight(List<Item> items)
        {
            double actualHeight = 0;
            Item currentItem = null;

            foreach (var i in items)
                if (i.TotalHeight > actualHeight)
                {
                    actualHeight = i.TotalHeight;
                    currentItem = i;
                }
            return currentItem;
        }

        public double SqAbsoluteHeight(int X, int Y, List<Item> ItemsOnSquare)
        {
            if (!this.ValidTile(X, Y))
                return 0.0;

            double HighestStack = 0.0;
            bool deduct = false;
            double deductable = 0.0;
            foreach (Item roomItem in ItemsOnSquare)
            {
                if (roomItem.TotalHeight > HighestStack)
                {
                    if (roomItem.GetBaseItem().IsSeat || roomItem.GetBaseItem().InteractionType == InteractionType.BED || roomItem.GetBaseItem().InteractionType == InteractionType.TENT_SMALL)
                    {
                        deduct = true;
                        deductable = roomItem.GetBaseItem().Height;
                    }
                    else
                        deduct = false;

                    HighestStack = roomItem.TotalHeight;
                }
            }
            double floorHeight = (double)this.Model.SqFloorHeight[X, Y];
            double stackHeight = HighestStack - (double)this.Model.SqFloorHeight[X, Y];
            if (deduct)
                stackHeight -= deductable;
            if (stackHeight < 0.0)
                stackHeight = 0.0;

            return floorHeight + stackHeight;
        }

        public bool ValidTile(int X, int Y, Item ball = null)
        {
            if (X < 0 || Y < 0 || X >= Model.MapSizeX || Y >= Model.MapSizeY)
            {
                if (ball != null)
                {
                    if ((X < 0 || X >= Model.MapSizeX) && (Y < 0 || Y >= Model.MapSizeY))
                    {
                        ball.ejex = true;
                        ball.ejey = true;
                    }
                    else if ((X < 0 || X >= Model.MapSizeX) && !(Y < 0 || Y >= Model.MapSizeY))
                    {
                        ball.ejex = true;
                        ball.ejey = false;
                    }
                    else if (!(X < 0 || X >= Model.MapSizeX) && (Y < 0 || Y >= Model.MapSizeY))
                    {
                        ball.ejex = false;
                        ball.ejey = true;
                    }
                    int incoming_dir = ball.BallDireccion;
                    ball.BallDireccion = outgoing_dir(incoming_dir, ball.ejey, ball.ejex);
                }
                return false;
            }
            return true;
        }

        public static Dictionary<int, ThreeDCoord> GetAffectedTiles(int Length, int Width, int PosX, int PosY, int Rotation)
        {
            int x = 0;

            Dictionary<int, ThreeDCoord> PointList = new Dictionary<int, ThreeDCoord>();

            if (Length > 0)
            {
                if (Rotation == 0 || Rotation == 4)
                {
                    for (int i = 0; i < Length; i++)
                    {
                        PointList.Add(x++, new ThreeDCoord(PosX, PosY + i, i));

                        for (int j = 0; j < Width; j++)
                        {
                            PointList.Add(x++, new ThreeDCoord(PosX + j, PosY + i, (i < j) ? j : i));
                        }
                    }
                }
                else if (Rotation == 2 || Rotation == 6)
                {
                    for (int i = 0; i < Length; i++)
                    {
                        PointList.Add(x++, new ThreeDCoord(PosX + i, PosY, i));

                        for (int j = 0; j < Width; j++)
                        {
                            PointList.Add(x++, new ThreeDCoord(PosX + i, PosY + j, (i < j) ? j : i));
                        }
                    }
                }
            }

            if (Width > 0)
            {
                if (Rotation == 0 || Rotation == 4)
                {
                    for (int i = 0; i < Width; i++)
                    {
                        PointList.Add(x++, new ThreeDCoord(PosX + i, PosY, i));

                        for (int j = 0; j < Length; j++)
                        {
                            PointList.Add(x++, new ThreeDCoord(PosX + i, PosY + j, (i < j) ? j : i));
                        }
                    }
                }
                else if (Rotation == 2 || Rotation == 6)
                {
                    for (int i = 0; i < Width; i++)
                    {
                        PointList.Add(x++, new ThreeDCoord(PosX, PosY + i, i));

                        for (int j = 0; j < Length; j++)
                        {
                            PointList.Add(x++, new ThreeDCoord(PosX + j, PosY + i, (i < j) ? j : i));
                        }
                    }
                }
            }

            return PointList;
        }

        public List<Item> GetItemsFromIds(List<int> Input)
        {
            if (Input == null || Input.Count == 0)
            {
                return new List<Item>();
            }

            List<Item> Items = new List<Item>();

            lock (Input)
            {
                foreach (int Id in Input.ToList())
                {
                    Item Itm = _room.GetRoomItemHandler().GetItem(Id);
                    if (Itm != null && !Items.Contains(Itm))
                    {
                        Items.Add(Itm);
                    }
                }
            }

            return Items.ToList();
        }

        public List<Item> GetRoomItemForSquare(int pX, int pY, double minZ)
        {
            List<Item> itemsToReturn = new List<Item>();

            Point coord = new Point(pX, pY);
            if (mCoordinatedItems.ContainsKey(coord))
            {
                List<Item> itemsFromSquare = GetItemsFromIds(mCoordinatedItems[coord]);

                foreach (Item item in itemsFromSquare)
                {
                    if (item.GetZ > minZ)
                    {
                        if (item.GetX == pX && item.GetY == pY)
                        {
                            itemsToReturn.Add(item);
                        }
                    }
                }
            }

            return itemsToReturn;
        }

        public List<Item> GetRoomItemForSquare(int pX, int pY)
        {
            Point coord = new Point(pX, pY);
            //List<RoomItem> itemsFromSquare = new List<RoomItem>();
            List<Item> itemsToReturn = new List<Item>();

            if (mCoordinatedItems.ContainsKey(coord))
            {
                List<Item> itemsFromSquare = GetItemsFromIds(mCoordinatedItems[coord]);

                foreach (Item item in itemsFromSquare)
                {
                    if (item.Coordinate.X == coord.X && item.Coordinate.Y == coord.Y)
                    {
                        itemsToReturn.Add(item);
                    }
                }
            }

            return itemsToReturn;
        }

        public bool GetRoomItemForSquare2(int pX, int pY)
        {
            Point coord = new Point(pX, pY);

            if (mCoordinatedItems.ContainsKey(coord))
            {
                List<Item> itemsFromSquare = GetItemsFromIds(mCoordinatedItems[coord]);

                foreach (Item item in itemsFromSquare)
                {
                    if (!item.GetBaseItem().Stackable)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool HasStackTool(int pX, int pY)
        {
            Point coord = new Point(pX, pY);

            if (mCoordinatedItems.ContainsKey(coord))
            {
                List<Item> itemsFromSquare = GetItemsFromIds(mCoordinatedItems[coord]);

                foreach (Item item in itemsFromSquare)
                {
                    if (item.GetBaseItem().InteractionType == InteractionType.STACKTOOL)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public bool IsRentableSpace(int pX, int pY, GameClient Session)
        {
            Point coord = new Point(pX, pY);

            if (mCoordinatedItems.ContainsKey(coord))
            {
                List<Item> itemsFromSquare = GetItemsFromIds(mCoordinatedItems[coord]);

                foreach (Item item in itemsFromSquare)
                {
                    if (item.GetBaseItem().InteractionType == InteractionType.RENTABLE_SPACE)
                    {
                        if (StarBlueServer.GetGame().GetRentableSpaceManager().GetRentableSpaceItem(item.Id, out RentableSpaceItem _rentableSpace))
                        {
                            Room room = Session.GetHabbo().CurrentRoom;

                            Item items = room.GetRoomItemHandler().GetItem(_rentableSpace.ItemId);

                            if (items == null)
                            {
                                return false;
                            }

                            if (items.GetBaseItem() == null)
                            {
                                return false;
                            }

                            if (items.GetBaseItem().InteractionType != InteractionType.RENTABLE_SPACE)
                            {
                                return false;
                            }

                            if (_rentableSpace.OwnerId == Session.GetHabbo().Id && StarBlueServer.GetUnixTimestamp() < _rentableSpace.ExpireStamp)
                            {
                                item.ExtraData = "Funcionando";
                                Session.SendMessage(new ObjectUpdateComposer(items, Session.GetHabbo().Id));
                                return true;
                            }
                            else
                            {
                                item.ExtraData = "Expirado";
                                Session.SendMessage(new ObjectUpdateComposer(items, Session.GetHabbo().Id));
                                return false;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public List<Item> GetAllRoomItemForSquare(int pX, int pY)
        {
            Point Coord = new Point(pX, pY);

            List<Item> Items = new List<Item>();

            if (mCoordinatedItems.TryGetValue(Coord, out List<int> Ids))
            {
                Items = GetItemsFromIds(Ids);
            }
            else
            {
                Items = new List<Item>();
            }

            return Items;
        }

        public bool SquareHasUser2(int X, int Y, RoomUser UserClicking)
        {
            Point coord = new Point(X, Y);
            List<RoomUser> UsersInSquare = GetRoomUsers(coord);
            if (UsersInSquare.Count == 1)
            {
                return !UsersInSquare.Contains(UserClicking);
            }
            else
            {
                return (UsersInSquare.Count > 0);
            }


        }

        public bool SquareHasUsers(int X, int Y)
        {
            return MapGotUser(new Point(X, Y));
        }

        public bool SquareHasUsers(int X, int Y, RoomUser MyUser)
        {
            List<RoomUser> users = GetRoomUsers(new Point(X, Y));
            if (users.Count > 0)
            {
                return !users.Exists(u => u.UserId == MyUser.UserId);
            }

            return false;
        }

        public RoomUser SquareHasUser4(int X, int Y, RoomUser MyUser)
        {
            List<RoomUser> users = GetRoomUsers(new Point(X, Y)).Where(u => u.UserId != MyUser.UserId).ToList();
            return users.FirstOrDefault();
        }

        public bool SquareHasUser3(int X, int Y, RoomUser UserClicking)
        {
            Point coord = new Point(X, Y);
            List<RoomUser> UsersInSquare = GetRoomUsers(coord);
            int i = 0;
            foreach (RoomUser Userin in UsersInSquare)
            {
                if (Userin != UserClicking)
                {
                    i++;
                }
            }
            if (i > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TilesTouching(Point p1, Point p2)
        {
            return TilesTouching(p1.X, p1.Y, p2.X, p2.Y);
        }

        public static bool TilesTouching(int X1, int Y1, int X2, int Y2)
        {
            if (!(Math.Abs(X1 - X2) > 1 || Math.Abs(Y1 - Y2) > 1))
            {
                return true;
            }

            if (X1 == X2 && Y1 == Y2)
            {
                return true;
            }

            return false;
        }

        public static int TileDistance(int X1, int Y1, int X2, int Y2)
        {
            return Math.Abs(X1 - X2) + Math.Abs(Y1 - Y2);
        }

        public DynamicRoomModel Model => mDynamicModel;

        public RoomModel StaticModel => mStaticModel;

        public byte[,] EffectMap => mUserItemEffect;

        public byte[,] GameMap => mGameMap;

        public double[,] ItemHeightMap => mItemHeightMap;

        public void Dispose()
        {
            userMap.Clear();
            mDynamicModel.Destroy();
            mCoordinatedItems.Clear();
            mCoordinatedMagicTileItems.Clear();

            Array.Clear(mGameMap, 0, mGameMap.Length);
            Array.Clear(mUserItemEffect, 0, mUserItemEffect.Length);
            Array.Clear(mItemHeightMap, 0, mItemHeightMap.Length);

            userMap = null;
            mGameMap = null;
            mUserItemEffect = null;
            mItemHeightMap = null;
            mCoordinatedItems = null;
            mCoordinatedMagicTileItems = null;

            mDynamicModel = null;
            _room = null;
            mStaticModel = null;
        }
    }
}
