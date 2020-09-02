using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.HabboHotel.Items.Wired.Util;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class MoveFurniToAwayBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }

        public WiredBoxType Type => WiredBoxType.EffectMoveFurniToAwayUser;

        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }

        public int Delay { get; set; } = 0 * 500;

        public int TickCount { get; set; }
        public string ItemsData { get; set; }

        private bool Requested;

        private int counter = 0;

        private long _next = 0;

        public MoveFurniToAwayBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
            TickCount = 0;
            Requested = false;
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Unknown2 = Packet.PopString();

            if (SetItems.Count > 0)
            {
                SetItems.Clear();
            }

            int FurniCount = Packet.PopInt();
            for (int i = 0; i < FurniCount; i++)
            {
                Item SelectedItem = Instance.GetRoomItemHandler().GetItem(Packet.PopInt());

                if (SelectedItem != null && !Instance.GetWired().OtherBoxHasItem(this, SelectedItem.Id))
                {
                    SetItems.TryAdd(SelectedItem.Id, SelectedItem);
                }
            }

            Delay = Packet.PopInt() * 500;
            counter = 0;
            TickCount = 0;
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
                counter = 0;
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
            if (counter > Delay)
            {
                counter = 0;
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

                    Item.MoveToDirMovement = Instance.GetGameMap().GetEscapeMovement(Item.GetX, Item.GetY, Item.MoveToDirMovement);
                    if (Item.MoveToDirMovement == MovementDirection.NONE)
                        return false;

                    Point Point = Movement.HandleMovementDir(Item.Coordinate, Item.MoveToDirMovement, Item.Rotation);

                    Instance.GetWired().onUserFurniCollision(Instance, Item);

                    if (!Instance.GetGameMap().ItemCanMove(Item, Point))
                    {
                        continue;
                    }

                    if (Instance.GetGameMap().CanRollItemHere(Point.X, Point.Y) && !Instance.GetGameMap().SquareHasUsers(Point.X, Point.Y))
                    {
                        double NewZ = Instance.GetGameMap().GetHeightForSquareFromData(Point);
                        bool CanBePlaced = true;

                        List<Item> Items = Instance.GetGameMap().GetCoordinatedItems(Point);
                        foreach (Item IItem in Items.ToList())
                        {
                            if (IItem == null || IItem.Id == Item.Id)
                            {
                                continue;
                            }

                            if (!IItem.GetBaseItem().Walkable)
                            {
                                _next = 0;
                                CanBePlaced = false;
                                break;
                            }

                            if (IItem.TotalHeight > NewZ)
                            {
                                NewZ = IItem.TotalHeight;
                            }

                            if (CanBePlaced && !IItem.GetBaseItem().Stackable)
                            {
                                CanBePlaced = false;
                            }

                            List<Item> ItemsForItem = Instance.GetGameMap().GetCoordinatedItems(new Point(IItem.GetX, IItem.GetY));
                            foreach (Item IIItem in ItemsForItem.ToList())
                            {
                                if (IIItem == null || IIItem.Id == Item.Id || IIItem.Id == IItem.Id)
                                {
                                    continue;
                                }

                                if (!IIItem.GetBaseItem().Walkable)
                                {
                                    _next = 0;
                                    CanBePlaced = false;
                                    break;
                                }

                                if (CanBePlaced && !IIItem.GetBaseItem().Stackable)
                                {
                                    CanBePlaced = false;
                                }
                            }
                        }

                        if (CanBePlaced && Point != Item.Coordinate)
                        {
                            Instance.SendMessage(new SlideObjectBundleComposer(Item.GetX, Item.GetY, Item.GetZ, Point.X, Point.Y, NewZ, 0, 0, Item.Id));
                            Instance.GetRoomItemHandler().SetFloorItem(Item, Point.X, Point.Y, NewZ);
                        }
                    }
                }

                Requested = false;
                _next = 0;
                return true;
            }
            return false;
        }
    }
}