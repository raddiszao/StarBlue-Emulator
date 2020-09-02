using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using System;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class RegenerateMapsBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }

        public WiredBoxType Type => WiredBoxType.EffectRegenerateMaps;

        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public int Delay { get; set; } = 0 * 500;

        public int TickCount { get; set; }

        private long _next;
        private int counter = 0;
        private bool Requested = false;

        public RegenerateMapsBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            StringData = "";
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Unknown2 = Packet.PopString();
            int Unknown3 = Packet.PopInt();
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
                TimeSpan TimeSinceRegen = DateTime.Now - Instance.lastRegeneration;

                if (TimeSinceRegen.TotalMinutes > 1)
                {
                    Instance.GetGameMap().GenerateMaps();
                    return true;
                }

                Requested = false;
                _next = 0;
            }

            return false;
        }
    }
}
