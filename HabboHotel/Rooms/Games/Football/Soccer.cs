using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Football;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Items.Wired;
using StarBlue.HabboHotel.Rooms.Games.Teams;
using StarBlue.HabboHotel.Rooms.PathFinding;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace StarBlue.HabboHotel.Rooms.Games.Football
{
    public class Soccer
    {
        private Room _room;
        private Item[] gates;
        private ConcurrentDictionary<int, Item> _balls;
        private bool _gameStarted;

        public Soccer(Room room)
        {
            _room = room;
            gates = new Item[4];
            _balls = new ConcurrentDictionary<int, Item>();
            _gameStarted = false;
        }
        public bool GameIsStarted
        {
            get { return _gameStarted; }
        }
        public void StopGame(bool userTriggered = false)
        {
            _gameStarted = false;

            if (!userTriggered)
            {
                _room.GetWired().TriggerEvent(WiredBoxType.TriggerGameEnds, null);
            }
        }

        public void StartGame()
        {
            _gameStarted = true;
            _room.GetWired().TriggerEvent(WiredBoxType.TriggerGameStarts, null);
        }

        public void AddBall(Item item)
        {
            _balls.TryAdd(item.Id, item);
        }

        public void RemoveBall(int itemID)
        {
            _balls.TryRemove(itemID, out Item Item);
        }

        public void OnUserWalk(RoomUser User)
        {
            if (User == null)
            {
                return;
            }

            foreach (Item item in _balls.Values.ToList())
            {
                int NewX = 0;
                int NewY = 0;
                int differenceX = User.X - item.GetX;
                int differenceY = User.Y - item.GetY;

                if (differenceX == 0 && differenceY == 0)
                {
                    if (User.RotBody == 4)
                    {
                        NewX = User.X;
                        NewY = User.Y + 2;
                        item.ExtraData = "55";
                        item.BallIsMoving = true;
                        item.BallValue = 1;

                    }
                    else if (User.RotBody == 6)
                    {
                        NewX = User.X - 2;
                        NewY = User.Y;
                        item.ExtraData = "55";
                        item.BallIsMoving = true;
                        item.BallValue = 1;

                    }
                    else if (User.RotBody == 0)
                    {
                        NewX = User.X;
                        NewY = User.Y - 2;
                        item.ExtraData = "55";
                        item.BallIsMoving = true;
                        item.BallValue = 1;

                    }
                    else if (User.RotBody == 2)
                    {
                        NewX = User.X + 2;
                        NewY = User.Y;
                        item.ExtraData = "55";
                        item.BallIsMoving = true;
                        item.BallValue = 1;

                    }
                    else if (User.RotBody == 1)
                    {
                        NewX = User.X + 2;
                        NewY = User.Y - 2;
                        item.ExtraData = "55";
                        item.BallIsMoving = true;
                        item.BallValue = 1;

                    }
                    else if (User.RotBody == 7)
                    {
                        NewX = User.X - 2;
                        NewY = User.Y - 2;
                        item.ExtraData = "55";
                        item.BallIsMoving = true;
                        item.BallValue = 1;

                    }
                    else if (User.RotBody == 3)
                    {
                        NewX = User.X + 2;
                        NewY = User.Y + 2;
                        item.ExtraData = "55";
                        item.BallIsMoving = true;
                        item.BallValue = 1;

                    }
                    else if (User.RotBody == 5)
                    {
                        NewX = User.X - 2;
                        NewY = User.Y + 2;
                        item.ExtraData = "55";
                        item.BallIsMoving = true;
                        item.BallValue = 1;
                    }

                    if (!_room.GetRoomItemHandler().CheckPosItem(User.GetClient(), item, NewX, NewY, item.Rotation, false, false))
                    {
                        if (User.RotBody == 0)
                        {
                            NewX = User.X;
                            NewY = User.Y + 1;
                        }
                        else if (User.RotBody == 2)
                        {
                            NewX = User.X - 1;
                            NewY = User.Y;
                        }
                        else if (User.RotBody == 4)
                        {
                            NewX = User.X;
                            NewY = User.Y - 1;
                        }
                        else if (User.RotBody == 6)
                        {
                            NewX = User.X + 1;
                            NewY = User.Y;
                        }
                        else if (User.RotBody == 5)
                        {
                            NewX = User.X + 1;
                            NewY = User.Y - 1;
                        }
                        else if (User.RotBody == 3)
                        {
                            NewX = User.X - 1;
                            NewY = User.Y - 1;
                        }
                        else if (User.RotBody == 7)
                        {
                            NewX = User.X + 1;
                            NewY = User.Y + 1;
                        }
                        else if (User.RotBody == 1)
                        {
                            NewX = User.X - 1;
                            NewY = User.Y + 1;
                        }
                    }
                }
                else if (differenceX <= 1 && differenceX >= -1 && differenceY <= 1 && differenceY >= -1 && VerifyBall(User, item.Coordinate.X, item.Coordinate.Y))//VERYFIC BALL CHECAR SI ESTA EN DIRECCION ASIA LA PELOTA
                {
                    NewX = differenceX * -1;
                    NewY = differenceY * -1;

                    NewX = NewX + item.GetX;
                    NewY = NewY + item.GetY;
                }

                if (item.GetRoom().GetGameMap().ValidTile(NewX, NewY))
                {
                    MoveBall(item, User.GetClient(), NewX, NewY);
                }
            }
        }

        private bool VerifyBall(RoomUser user, int actualx, int actualy)
        {
            return Rotation.Calculate(user.X, user.Y, actualx, actualy) == user.RotBody;
        }

        public void RegisterGate(Item item)
        {
            if (gates[0] == null)
            {
                item.team = TEAM.BLUE;
                gates[0] = item;
            }
            else if (gates[1] == null)
            {
                item.team = TEAM.RED;
                gates[1] = item;
            }
            else if (gates[2] == null)
            {
                item.team = TEAM.GREEN;
                gates[2] = item;
            }
            else if (gates[3] == null)
            {
                item.team = TEAM.YELLOW;
                gates[3] = item;
            }
        }

        public void UnRegisterGate(Item item)
        {
            switch (item.team)
            {
                case TEAM.BLUE:
                    {
                        gates[0] = null;
                        break;
                    }
                case TEAM.RED:
                    {
                        gates[1] = null;
                        break;
                    }
                case TEAM.GREEN:
                    {
                        gates[2] = null;
                        break;
                    }
                case TEAM.YELLOW:
                    {
                        gates[3] = null;
                        break;
                    }
            }
        }

        public void onGateRemove(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.FOOTBALL_GOAL_RED:
                case InteractionType.footballcounterred:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.RED);
                        break;
                    }
                case InteractionType.FOOTBALL_GOAL_GREEN:
                case InteractionType.footballcountergreen:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.GREEN);
                        break;
                    }
                case InteractionType.FOOTBALL_GOAL_BLUE:
                case InteractionType.footballcounterblue:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.BLUE);
                        break;
                    }
                case InteractionType.FOOTBALL_GOAL_YELLOW:
                case InteractionType.footballcounteryellow:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.YELLOW);
                        break;
                    }
            }
        }

        private IEnumerable<Item> GetFootballItemsForAllTeams()
        {
            List<Item> items = _room.GetGameManager().GetFurniItems(TEAM.RED).Values.ToList();
            items.AddRange(_room.GetGameManager().GetFurniItems(TEAM.GREEN).Values);

            items.AddRange(_room.GetGameManager().GetFurniItems(TEAM.BLUE).Values);

            items.AddRange(_room.GetGameManager().GetFurniItems(TEAM.YELLOW).Values);

            return items;
        }

        private bool GameItemOverlaps(Item gameItem)
        {
            Point gameItemCoord = gameItem.Coordinate;
            return
                GetFootballItemsForAllTeams()
                    .Any(
                        item =>
                            item.GetAffectedTiles.Values.Any(
                                tile => tile.X == gameItemCoord.X && tile.Y == gameItemCoord.Y));
        }

        public bool MoveBall(Item item, GameClient mover, int newX, int newY)
        {
            if (item == null || item.GetBaseItem() == null /*|| mover == null || mover.GetHabbo() == null*/)
            {
                return false;
            }

            if (!_room.GetGameMap().itemCanBePlacedHere(newX, newY))
            {
                return false;
            }

            var oldRoomCoord = item.Coordinate;
            var itemIsOnGameItem = GameItemOverlaps(item);
            double newZ = _room.GetGameMap().Model.SqFloorHeight[newX, newY];


            _room.SendMessage(new UpdateFootBallComposer(item, newX, newY));

            /*var mMessage = new ServerPacket(ServerPacketHeader.ObjectUpdateMessageComposer);
            mMessage.WriteInteger(item.Coordinate.X);
            mMessage.WriteInteger(item.Coordinate.Y);
            mMessage.WriteInteger(newX);
            mMessage.WriteInteger(newY);
            mMessage.WriteInteger(1);
            mMessage.WriteInteger(item.Id);
            mMessage.WriteString(TextHandling.GetString(item.GetZ));
            mMessage.WriteString(TextHandling.GetString(NewZ));
            mMessage.WriteInteger(item.Id);
            _room.SendMessage(mMessage);*/

            //_room.SendMessage(new SlideObjectBundleComposer(item.Coordinate.X, item.Coordinate.Y, item.GetZ, newX, newY, NewZ, item.Id, item.Id, item.Id));

            if (oldRoomCoord.X == newX && oldRoomCoord.Y == newY)
            {
                return false;
            }

            item.SetState(newX, newY, item.GetZ, Gamemap.GetAffectedTiles(item.GetBaseItem().Length, item.GetBaseItem().Width, newX, newY, item.Rotation));

            if (itemIsOnGameItem || mover == null || mover.GetHabbo() == null)
            {
                return false;
            }

            _room.OnUserShoot(mover, item);

            return true;
        }

        public void MoveBall(Item item, GameClient client, Point user, bool skip = false)
        {
            try
            {
                item.comeDirection = ComeDirection.GetComeDirection(user, item.Coordinate);

                if (item.comeDirection != IComeDirection.Null)
                {
                    // item.ballMover = client;
                    new TaskFactory().StartNew(() => MoveBallProcess(item, client));
                }
            }
            catch
            {
            }
        }

        public async void MoveBallProcess(Item item, GameClient client)
        {

            var tryes = 0;
            var newX = item.Coordinate.X;
            var newY = item.Coordinate.Y;

            //while (tryes < 3)
            {
                if (_room == null || _room.GetGameMap() == null)
                {
                    return;
                }

                var total = item.ExtraData == "55" ? 6 : 1;
                for (var i = 0; i != total; i++)
                {
                    if (item.comeDirection == IComeDirection.Null)
                    {
                        item.BallIsMoving = false;
                        break;
                    }

                    var resetX = newX;
                    var resetY = newY;

                    ComeDirection.GetNewCoords(item.comeDirection, ref newX, ref newY);

                    var ignoreUsers = false;

                    if (_room.GetGameMap().SquareHasUsers(newX, newY))
                    {
                        if (item.ExtraData != "55" && item.ExtraData != "44")
                        {
                            item.BallIsMoving = false;
                            break;
                        }
                        ignoreUsers = true;
                    }

                    if (ignoreUsers == false)
                    {
                        if (!_room.GetGameMap().itemCanBePlacedHere(newX, newY))
                        {
                            item.comeDirection = ComeDirection.InverseDirections(_room, item.comeDirection, newX, newY);
                            newX = resetX;
                            newY = resetY;
                            tryes++;
                            if (tryes > 2)
                            {
                                item.BallIsMoving = false;
                            }

                            continue;
                        }
                    }

                    if (MoveBall(item, client, newX, newY))
                    {
                        item.BallIsMoving = false;
                        break;
                    }

                    int.TryParse(item.ExtraData, out int number);
                    if (number > 11)
                    {
                        item.ExtraData = (int.Parse(item.ExtraData) - 11).ToString();
                    }

                    await Task.Delay(90);
                }

                item.BallValue++;

                if (item.BallValue <= 6)
                {
                    return;
                }

                item.BallIsMoving = false;
                item.BallValue = 1;
                //break;
            }
        }

        public void Dispose()
        {
            Array.Clear(gates, 0, gates.Length);
            gates = null;
            _room = null;
            _balls.Clear();
            _balls = null;
        }
    }
}