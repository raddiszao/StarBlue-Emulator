using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System.Collections;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class RoomForwardBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type => WiredBoxType.EffectRoomForward;

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public int Delay { get => _delay; set { _delay = value; TickCount = value + 1; } }
        public int TickCount { get; set; }
        private int _delay = 0;
        private Queue _queue;

        public RoomForwardBox(Room Instance, Item Item)
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
            string Room = Packet.PopString();
            int Unknown2 = Packet.PopInt();
            Delay = Packet.PopInt();
            if (!string.IsNullOrEmpty(Room) && int.TryParse(Room, out int RoomId))
            {
                StringData = RoomId + "";
            }
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

                FowwardRoom(Player);
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

            TickCount = Delay;
            _queue.Enqueue(Player);
            return true;
        }

        public bool FowwardRoom(Habbo Player)
        {
            if (Player == null || Player.GetClient() == null)
            {
                return false;
            }

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(StringData))
            {
                return false;
            }

            User.GetClient().SendMessage(new CloseConnectionComposer());
            User.GetClient().SendMessage(new RoomForwardComposer(int.Parse(StringData)));

            return true;
        }
    }
}