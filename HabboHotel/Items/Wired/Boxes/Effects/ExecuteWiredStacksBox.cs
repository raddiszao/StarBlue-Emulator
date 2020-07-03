using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Conditions
{
    class ExecuteWiredStacksBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectExecuteWiredStacks;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }
        public int Delay { get => _delay; set { _delay = value; TickCount = value + 1; } }
        public int TickCount { get; set; }
        private int _delay = 0;
        private Queue _queue;

        public ExecuteWiredStacksBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
            TickCount = Delay;
            _queue = new Queue();
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

                ExecuteWiredStacks(Player);
            }

            TickCount = Delay;
            return true;
        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length != 1)
            {
                return false;
            }

            Habbo Player = (Habbo)Params[0];
            if (Player == null)
            {
                return false;
            }

            _queue.Enqueue(Player);
            return true;
        }

        private void ExecuteWiredStacks(Habbo Player)
        {
            foreach (Item Item in SetItems.Values.ToList())
            {
                if (Item == null || !Instance.GetRoomItemHandler().GetFloor.Contains(Item) || !Item.IsWired)
                {
                    continue;
                }

                if (Instance.GetWired().TryGet(Item.Id, out IWiredItem WiredItem))
                {
                    if (WiredItem.Type == WiredBoxType.EffectExecuteWiredStacks)
                    {
                        continue;
                    }
                    else
                    {
                        ICollection<IWiredItem> Effects = Instance.GetWired().GetEffects(WiredItem);
                        if (Effects.Count > 0)
                        {
                            foreach (IWiredItem EffectItem in Effects.ToList())
                            {
                                if (SetItems.ContainsKey(EffectItem.Item.Id) && EffectItem.Item.Id != Item.Id)
                                {
                                    continue;
                                }
                                else if (EffectItem.Type == WiredBoxType.EffectExecuteWiredStacks)
                                {
                                    continue;
                                }
                                else
                                {
                                    EffectItem.Execute(Player);
                                }
                            }
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
        }
    }
}