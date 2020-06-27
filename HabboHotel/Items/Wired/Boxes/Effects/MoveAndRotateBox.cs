﻿using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.HabboHotel.Rooms;
using StarBlue.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class MoveAndRotateBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }

        public WiredBoxType Type
        {
            get { return WiredBoxType.EffectMoveAndRotate; }
        }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }

        public int Delay { get; set; } = 0 * 500;

        public int TickCount { get; set; }
        public string ItemsData { get; set; }
        private bool Requested;
        private int counter = 0;
        private long _next;

        public MoveAndRotateBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
            Requested = false;
        }

        public void HandleSave(ClientPacket Packet)
        {
            if (SetItems.Count > 0)
            {
                SetItems.Clear();
            }

            int Unknown = Packet.PopInt();
            int Movement = Packet.PopInt();
            int Rotation = Packet.PopInt();

            string Unknown1 = Packet.PopString();

            int FurniCount = Packet.PopInt();
            for (int i = 0; i < FurniCount; i++)
            {
                Item SelectedItem = Instance.GetRoomItemHandler().GetItem(Packet.PopInt());

                if (SelectedItem != null && !Instance.GetWired().OtherBoxHasItem(this, SelectedItem.Id))
                {
                    SetItems.TryAdd(SelectedItem.Id, SelectedItem);
                }
            }

            StringData = Movement + ";" + Rotation;
            Delay = Packet.PopInt() * 500;
            TickCount = 0;
            counter = 0;
        }

        public bool Execute(params object[] Params)
        {
            if (SetItems.Count == 0)
            {
                return false;
            }

            if (_next == 0 || _next < StarBlueServer.Now())
            {
                _next = StarBlueServer.Now() + Delay;
            }

            if (!Requested)
            {
                counter = Delay;
                Requested = true;
            }

            return true;
        }

        public bool OnCycle()
        {
            if (Instance == null || !Requested || _next == 0)
            {
                return false;
            }

            counter += 500;
            if (counter >= Delay)
            {
                counter = 0;
                _next = 0;
                foreach (Item Item in SetItems.Values.ToList())
                {
                    if (Item == null)
                    {
                        continue;
                    }

                    if (!Instance.GetRoomItemHandler().GetFloor.Contains(Item))
                    {
                        continue;
                    }

                    Item toRemove = null;

                    if (Instance.GetWired().OtherBoxHasItem(this, Item.Id))
                    {
                        SetItems.TryRemove(Item.Id, out toRemove);
                    }

                    // Prueba de Pusheo para el Salinas, que no le funciona.
                    Point Point = HandleMovement(Convert.ToInt32(StringData.Split(';')[0]), new Point(Item.GetX, Item.GetY));
                    int newRot = HandleRotation(Convert.ToInt32(StringData.Split(';')[1]), Item.Rotation);

                    if (!Instance.GetGameMap().ItemCanMove(Item, Point))
                    {
                        continue;
                    }

                    if (Instance.GetGameMap().CanRollItemHere(Point.X, Point.Y) && !Instance.GetGameMap().SquareHasUsers(Point.X, Point.Y))
                    {
                        Double NewZ = Instance.GetGameMap().GetHeightForSquareFromData(Point);
                        Boolean CanBePlaced = true;

                        List<Item> Items = Instance.GetGameMap().GetCoordinatedItems(Point);
                        foreach (Item IItem in Items.ToList())
                        {
                            if (IItem == null || IItem.Id == Item.Id)
                            {
                                continue;
                            }

                            if (!IItem.GetBaseItem().Walkable)
                            {
                                counter = 0;
                                CanBePlaced = false;
                                break;
                            }

                            if (IItem.TotalHeight > NewZ)
                            {
                                NewZ = IItem.TotalHeight;
                            }

                            if (CanBePlaced == true && !IItem.GetBaseItem().Stackable)
                            {
                                CanBePlaced = false;
                            }
                        }

                        if (newRot != Item.Rotation)
                        {
                            Item.Rotation = newRot;
                            Item.UpdateState(false, true);
                        }

                        if (CanBePlaced && Point != Item.Coordinate)
                        {
                            Instance.SendMessage(new SlideObjectBundleComposer(Item.GetX, Item.GetY, Item.GetZ, Point.X,
                                Point.Y, NewZ, 0, 0, Item.Id));
                            Instance.GetRoomItemHandler().SetFloorItem(Item, Point.X, Point.Y, NewZ);
                        }
                    }
                }

                return true;
            }
            return false;
        }

        private int HandleRotation(int mode, int rotation)
        {
            switch (mode)
            {
                case 1:
                    {
                        rotation += 2;
                        if (rotation > 6)
                        {
                            rotation = 0;
                        }
                        break;
                    }

                case 2:
                    {
                        rotation -= 2;
                        if (rotation < 0)
                        {
                            rotation = 6;
                        }
                        break;
                    }

                case 3:
                    {
                        if (RandomNumber.GenerateRandom(0, 2) == 0)
                        {
                            rotation += 2;
                            if (rotation > 6)
                            {
                                rotation = 0;
                            }
                        }
                        else
                        {
                            rotation -= 2;
                            if (rotation < 0)
                            {
                                rotation = 6;
                            }
                        }
                        break;
                    }
            }
            return rotation;
        }

        private Point HandleMovement(int Mode, Point Position)
        {
            Point NewPos = new Point();
            switch (Mode)
            {
                case 0:
                    {
                        NewPos = Position;
                        break;
                    }
                case 1:
                    {
                        switch (RandomNumber.GenerateRandom(1, 8))
                        {
                            case 1:
                                NewPos = new Point(Position.X + 1, Position.Y);
                                break;
                            case 2:
                                NewPos = new Point(Position.X - 1, Position.Y);
                                break;
                            case 3:
                                NewPos = new Point(Position.X, Position.Y + 1);
                                break;
                            case 4:
                                NewPos = new Point(Position.X, Position.Y - 1);
                                break;
                            case 5:
                                NewPos = new Point(Position.X + 1, Position.Y + 1);
                                break;
                            case 6:
                                NewPos = new Point(Position.X - 1, Position.Y - 1);
                                break;
                            case 7:
                                NewPos = new Point(Position.X - 1, Position.Y + 1);
                                break;
                            case 8:
                                NewPos = new Point(Position.X + 1, Position.Y - 1);
                                break;
                        }
                        break;
                    }
                case 2:
                    {
                        if (RandomNumber.GenerateRandom(0, 2) == 1)
                        {
                            NewPos = new Point(Position.X - 1, Position.Y);
                        }
                        else
                        {
                            NewPos = new Point(Position.X + 1, Position.Y);
                        }
                        break;
                    }
                case 3:
                    {
                        if (RandomNumber.GenerateRandom(0, 2) == 1)
                        {
                            NewPos = new Point(Position.X, Position.Y - 1);
                        }
                        else
                        {
                            NewPos = new Point(Position.X, Position.Y + 1);
                        }
                        break;
                    }
                case 4:
                    {
                        NewPos = new Point(Position.X, Position.Y - 1);
                        break;
                    }
                case 5:
                    {
                        NewPos = new Point(Position.X + 1, Position.Y);
                        break;
                    }
                case 6:
                    {
                        NewPos = new Point(Position.X, Position.Y + 1);
                        break;
                    }
                case 7:
                    {
                        NewPos = new Point(Position.X - 1, Position.Y);
                        break;
                    }
            }

            return NewPos;
        }
    }
}