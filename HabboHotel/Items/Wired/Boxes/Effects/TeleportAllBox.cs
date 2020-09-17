using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class TeleportAllBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectTeleportAll;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public string Message2 { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }
        public int Delay { get => _delay; set { _delay = value; TickCount = value + 1; } }
        public int TickCount { get; set; }
        private int _delay = 0;
        private Queue _queue;

        public TeleportAllBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
            TickCount = Delay;
            _queue = new Queue();
        }

        public void HandleSave(MessageEvent Packet)
        {
            Packet.PopInt();
            Packet.PopString();
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

            Delay = Packet.PopInt();
        }

        public bool OnCycle()
        {
            if (_queue.Count == 0)
            {
                _queue.Clear();
                TickCount = Delay;
                return true;
            }

            while (_queue.Count > 0)
            {
                Habbo Player = (Habbo)_queue.Dequeue();
                if (Player == null || Player.CurrentRoom != Instance)
                {
                    continue;
                }

                TeleportAll();
            }

            TickCount = Delay;
            return true;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
            {
                return false;
            }

            Habbo Player = (Habbo)Params[0];

            if (Player == null)
            {
                return false;
            }

            TickCount = Delay;
            _queue.Enqueue(Player);
            return true;

        }

        private void TeleportAll()
        {
            foreach (RoomUser User in Instance.GetRoomUserManager().GetRoomUsers())
            {
                if (User.GetClient().GetRoomUser() == null)
                {
                    continue;
                }

                Room Room = User.GetClient().GetHabbo().CurrentRoom;
                if (Room == null)
                {
                    continue;
                }

                /*if (User.GetClient().GetHabbo().IsTeleporting || User.GetClient().GetHabbo().IsHopping || User.GetClient().GetHabbo().TeleporterId != 0)
                {
                    continue;
                }*/

                Random rand = new Random();
                List<Item> Items = SetItems.Values.ToList();
                Items = Items.OrderBy(x => rand.Next()).ToList();

                if (Items.Count == 0)
                {
                    return;
                }

                Item Item = Items.First();
                if (Item == null)
                {
                    return;
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
                        return;
                    }

                    Item = Items.First();
                    if (Item == null)
                    {
                        return;
                    }
                }

                if (Room.GetGameMap() == null)
                {
                    return;
                }

                if (Item.Coordinate != User.Coordinate)
                {
                    Room.GetGameMap().TeleportToItem(User, Item);
                    Room.GetRoomUserManager().UpdateUserStatusses();
                }

                if (User.GetClient().GetHabbo().Effects() != null)
                {
                    User.GetClient().GetHabbo().Effects().ApplyEffect(0);
                }
            }
        }
    }
}