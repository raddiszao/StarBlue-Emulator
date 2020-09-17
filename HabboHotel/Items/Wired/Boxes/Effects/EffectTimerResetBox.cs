using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using System;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    class EffectTimerResetBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectTimerReset; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public int Delay { get; set; } = 0 * 500;
        public int TickCount { get; set; }
        public string ItemsData { get; set; }
        private bool Requested = false;
        private int counter = 0;
        private long _next;

        public EffectTimerResetBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(MessageEvent Packet)
        {
            int Unknown = Packet.PopInt();
            string Unknown3 = Packet.PopString();
            int Unknown4 = Packet.PopInt();
            int Delay = Packet.PopInt();
            this.counter = 0;
            this.TickCount = 0;
            this.Delay = Delay * 500;
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

                Instance.lastTimerReset = DateTime.Now;

                Requested = false;
                _next = 0;
                return true;
            }
            return false;
        }

        public bool Execute(params object[] Params)
        {
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
    }
}