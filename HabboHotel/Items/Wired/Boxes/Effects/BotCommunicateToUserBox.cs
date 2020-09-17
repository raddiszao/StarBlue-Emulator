using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System.Collections;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class BotCommunicateToUserBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectBotCommunicatesToUserBox;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public int Delay { get => _delay; set { _delay = value; TickCount = value + 1; } }
        public int TickCount { get; set; }
        private int _delay = 0;
        private Queue _queue;

        public BotCommunicateToUserBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
            TickCount = Delay;
            _queue = new Queue();
        }

        public void HandleSave(MessageEvent Packet)
        {
            int Unknown = Packet.PopInt();
            int ChatMode = Packet.PopInt();
            string ChatConfig = Packet.PopString();
            int Unknown2 = Packet.PopInt();
            Delay = Packet.PopInt();

            StringData = ChatConfig;
            if (ChatMode == 1)
            {
                BoolData = true;
            }
            else
            {
                BoolData = false;
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

                BotCommunicateToUser(Player);
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

        public bool BotCommunicateToUser(Habbo Player)
        {
            if (string.IsNullOrEmpty(StringData) || Player == null || Player.GetClient() == null)
            {
                return false;
            }

            string BotName = StringData.Split('	')[0];
            string Chat = string.Empty;
            try
            {
                Chat = StringData.Split('	')[1];
            }
            catch
            {
                return false;
            }

            RoomUser User = Instance.GetRoomUserManager().GetBotByName(BotName);
            if (User == null)
            {
                return false;
            }

            if (Chat.Contains("%username%"))
            {
                Chat = Chat.Replace("%username%", Player.Username);
            }

            if (BoolData)
            {
                Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, Chat, 0, 31));
            }
            else
            {
                Player.GetClient().SendMessage(new ChatComposer(User.VirtualId, Chat, 0, 31));
            }

            return true;
        }
    }
}