using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class TeleportBotToFurniBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectTeleportBotToFurniBox;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }
        public int Delay { get; set; } = 0 * 500;

        public int TickCount { get; set; }

        private long _next;
        private int counter = 0;
        private bool Requested = false;

        public TeleportBotToFurniBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(MessageEvent Packet)
        {
            int Unknown = Packet.PopInt();
            string BotName = Packet.PopString();

            if (SetItems.Count > 0)
            {
                SetItems.Clear();
            }

            int FurniCount = Packet.PopInt();
            for (int i = 0; i < FurniCount; i++)
            {
                Item SelectedItem = Instance.GetRoomItemHandler().GetItem(Packet.PopInt());
                if (SelectedItem != null)
                {
                    SetItems.TryAdd(SelectedItem.Id, SelectedItem);
                }
            }

            Delay = Packet.PopInt() * 500;
            counter = 0;
            TickCount = 0;

            StringData = BotName;
        }

        public bool Execute(params object[] Params)
        {
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
            if (string.IsNullOrEmpty(StringData) || Instance == null || !Requested || _next == 0)
            {
                return false;
            }

            counter += 500;
            if (counter >= Delay)
            {
                counter = 0;

                RoomUser User = Instance.GetRoomUserManager().GetBotByName(StringData);
                if (User == null)
                {
                    return false;
                }

                Random rand = new Random();
                List<Item> Items = SetItems.Values.ToList();
                Items = Items.OrderBy(x => rand.Next()).ToList();

                if (Items.Count == 0)
                {
                    return false;
                }

                Item Item = Items.First();
                if (Item == null)
                {
                    return false;
                }

                if (!Instance.GetRoomItemHandler().GetFloor.Contains(Item))
                {
                    SetItems.TryRemove(Item.Id, out Item);

                    if (Items.Contains(Item))
                    {
                        Items.Remove(Item);
                    }

                    if (SetItems.Count == 0 || Items.Count == 0)
                    {
                        return false;
                    }

                    Item = Items.First();
                    if (Item == null)
                    {
                        return false;
                    }
                }

                if (Instance.GetGameMap() == null)
                {
                    return false;
                }

                Instance.GetGameMap().TeleportToItem(User, Item);
                Instance.GetRoomUserManager().UpdateUserStatusses();

                Requested = false;
                _next = 0;
                return true;
            }
            return false;
        }
    }
}