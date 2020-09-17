using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Concurrent;
namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class SetRollerSpeedBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectSetRollerSpeed;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public int Delay { get; set; } = 0 * 500;

        public int TickCount { get; set; }

        private long _next;
        private int counter = 0;
        private bool Requested = false;

        public SetRollerSpeedBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();

            if (SetItems.Count > 0)
            {
                SetItems.Clear();
            }
        }

        public void HandleSave(MessageEvent Packet)
        {
            if (SetItems.Count > 0)
            {
                SetItems.Clear();
            }

            int Unknown = Packet.PopInt();
            string Message = Packet.PopString();
            int Unknown2 = Packet.PopInt();
            Delay = Packet.PopInt() * 500;
            counter = 0;
            TickCount = 0;

            StringData = Message;

            if (!int.TryParse(StringData, out int Speed))
            {
                StringData = "";
            }
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
                if (int.TryParse(StringData, out int Speed))
                {
                    Instance.GetRoomItemHandler().SetSpeed(Speed);
                }

                Requested = false;
                _next = 0;
                return true;
            }
            return false;
        }
    }
}