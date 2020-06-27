using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.HabboHotel.Rooms;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    class MatchPositionBox : IWiredItem, IWiredCycle
    {
        private int _delay = 0;
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type { get { return WiredBoxType.EffectMatchPosition; } }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public int Delay { get { return _delay; } set { _delay = value; TickCount = value + 1; } }

        public int TickCount { get; set; }

        public bool MatchPosition { get; set; }

        public bool MatchRotation { get; set; }

        public bool MatchState { get; set; }

        private bool Requested;
        public string ItemsData { get; set; }

        public MatchPositionBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
            TickCount = Delay;
            Requested = false;
        }

        public void HandleSave(ClientPacket Packet)
        {
            if (SetItems.Count > 0)
            {
                SetItems.Clear();
            }

            int Unknown = Packet.PopInt();
            int State = Packet.PopInt();
            int Direction = Packet.PopInt();
            int Placement = Packet.PopInt();
            string Unknown2 = Packet.PopString();

            int FurniCount = Packet.PopInt();
            for (int i = 0; i < FurniCount; i++)
            {
                Item SelectedItem = Instance.GetRoomItemHandler().GetItem(Packet.PopInt());
                if (SelectedItem != null)
                {
                    SetItems.TryAdd(SelectedItem.Id, SelectedItem);
                }
            }

            StringData = State + ";" + Direction + ";" + Placement;

            int Delay = Packet.PopInt();
            this.Delay = Delay;
        }

        public bool Execute(params object[] Params)
        {
            if (!Requested)
            {
                TickCount = Delay;
                Requested = true;
            }
            return true;
        }

        public bool OnCycle()
        {
            if (!Requested || String.IsNullOrEmpty(StringData) || StringData == "0;0;0" || SetItems.Count == 0)
            {
                return false;
            }

            int itemsDataCount = 0;
            foreach (Item item in SetItems.Values.ToList())
            {
                if (Instance.GetRoomItemHandler().GetFloor == null && !Instance.GetRoomItemHandler().GetFloor.Contains(item))
                {
                    continue;
                }

                string I = ItemsData.Split(';')[itemsDataCount];
                itemsDataCount++;

                if (string.IsNullOrEmpty(I))
                {
                    continue;
                }

                int itemId = Convert.ToInt32(I.Split(':')[0]);
                Item II = Instance.GetRoomItemHandler().GetItem(Convert.ToInt32(itemId));
                if (II == null)
                {
                    continue;
                }

                string[] partsString = I.Split(':');
                try
                {
                    if (string.IsNullOrEmpty(partsString[0]) || string.IsNullOrEmpty(partsString[1]))
                    {
                        continue;
                    }
                }
                catch { continue; }

                string[] part = partsString[1].Split(',');

                try
                {
                    if (int.Parse(StringData.Split(';')[0]) == 1)//State
                    {
                        if (part.Count() >= 4)
                        {
                            SetState(II, part[4].ToString());
                        }
                        else
                        {
                            SetState(II, "1");
                        }
                    }
                }
                catch (Exception) { }

                try
                {
                    if (int.Parse(StringData.Split(';')[1]) == 1)//Direction
                    {
                        SetRotation(II, Convert.ToInt32(part[3]));
                    }
                }
                catch (Exception) { }

                try
                {
                    if (int.Parse(StringData.Split(';')[2]) == 1)//Position
                    {
                        SetPosition(II, Convert.ToInt32(part[0].ToString()), Convert.ToInt32(part[1].ToString()), Convert.ToDouble(part[2].ToString()));
                    }
                }
                catch (Exception) { }
            }
            Requested = false;
            return true;
        }

        private void SetState(Item Item, string Extradata)
        {
            if (Item.ExtraData == Extradata)
            {
                return;
            }

            if (Item.GetBaseItem().InteractionType == InteractionType.DICE)
            {
                return;
            }

            Item.ExtraData = Extradata;
            Item.UpdateState(false, true);
        }

        private void SetRotation(Item Item, int Rotation)
        {
            if (Item.Rotation == Rotation)
            {
                return;
            }

            Item.Rotation = Rotation;
            Item.UpdateState(false, true);
        }

        private void SetPosition(Item Item, int CoordX, int CoordY, double CoordZ)
        {
            if (Item.GetX == CoordX && Item.GetY == CoordY && Item.GetZ == CoordZ)
            {
                return;
            }

            Instance.SendMessage(new SlideObjectBundleComposer(Item.GetX, Item.GetY, Item.GetZ, CoordX, CoordY, CoordZ, 0, 0, Item.Id));
            Instance.GetRoomItemHandler().SetFloorItem(Item, CoordX, CoordY, CoordZ);
        }
    }
}
