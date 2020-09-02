using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Concurrent;
using System.Linq;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class ToggleNegativeFurniBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectToggleNegativeFurniState;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public int TickCount { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public int Delay { get; set; } = 0 * 500;
        public string ItemsData { get; set; }

        private long _next;

        private int counter = 0;

        private bool Requested = false;

        public ToggleNegativeFurniBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            SetItems.Clear();
            int Unknown = Packet.PopInt();
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

            Delay = Packet.PopInt() * 500;
            counter = 0;
            TickCount = 0;
        }

        public bool Execute(params object[] Params)
        {
            if (_next == 0 || _next < StarBlueServer.Now())
            {
                _next = StarBlueServer.Now() + Delay;
            }

            Requested = true;
            TickCount = 0;
            return true;
        }

        public bool OnCycle()
        {
            if (SetItems.Count == 0 || !Requested)
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
                        SetItems.TryRemove(Item.Id, out Item n);
                        continue;
                    }

                    int test = int.Parse(Item.ExtraData);
                    test--;
                    Item.ExtraData = test + "";

                }

                Requested = false;

                _next = 0;

            }
            return true;
        }
    }
}
