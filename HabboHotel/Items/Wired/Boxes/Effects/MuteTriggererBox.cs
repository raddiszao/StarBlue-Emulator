using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System.Collections;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class MuteTriggererBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectMuteTriggerer;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public int Delay { get => _delay; set { _delay = value; TickCount = value + 1; } }
        public int TickCount { get; set; }
        private int _delay = 0;
        private Queue _queue;

        public MuteTriggererBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
            TickCount = Delay;
            _queue = new Queue();
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
            int Time = Packet.PopInt();
            string Message = Packet.PopString();
            int Unknown2 = Packet.PopInt();
            Delay = Packet.PopInt();

            StringData = Time + ";" + Message;
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

                MuteUser(Player);
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

        public bool MuteUser(Habbo Player)
        {
            if (Player == null)
            {
                return false;
            }

            RoomUser User = Instance.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);
            if (User == null)
            {
                return false;
            }

            if (Player.GetPermissions().HasRight("mod_tool") || Instance.RoomData.OwnerId == Player.Id)
            {
                Player.GetClient().SendWhisper("Wired Mute: Usuário impossível de mutar.", 34);
                return false;
            }

            int Time = (StringData != null ? int.Parse(StringData.Split(';')[0]) : 0);
            string Message = (StringData != null ? (StringData.Split(';')[1]) : "No message!");

            if (Time > 0)
            {
                Player.GetClient().SendWhisper("Wired Mute: Você foi mutado por " + Time + "! Razao: " + Message, 34);
                if (!Instance.MutedUsers.ContainsKey(Player.Id))
                {
                    Instance.MutedUsers.Add(Player.Id, (StarBlueServer.GetUnixTimestamp() + (Time * 60)));
                }
                else
                {
                    Instance.MutedUsers.Remove(Player.Id);
                    Instance.MutedUsers.Add(Player.Id, (StarBlueServer.GetUnixTimestamp() + (Time * 60)));
                }
            }

            return true;
        }
    }
}