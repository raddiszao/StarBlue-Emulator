using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System.Collections;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class SendCustomMessageBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.SendCustomMessageBox;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }
        public int Delay { get => _delay; set { _delay = value; TickCount = value + 1; } }
        public int TickCount { get; set; }
        private int _delay = 0;
        private Queue _queue;

        public SendCustomMessageBox(Room Instance, Item Item)
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
            string Message = Packet.PopString();
            int Unknown2 = Packet.PopInt();
            Delay = Packet.PopInt();

            StringData = Message;
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

                ShowMessage(Player);
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

        public bool ShowMessage(Habbo Player)
        {
            if (Player == null || Player.GetClient() == null || string.IsNullOrWhiteSpace(StringData))
            {
                return false;
            }

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
            {
                return false;
            }

            string[] Message = StringData.Split('_');

            if (Message[0] == "1")
            {
                Player.GetClient().SendMessage(new RoomNotificationComposer(Message[1],
                Message[2], "gifts_event", Message[3], "event:" + Message[4]));
            }

            if (Message[0] == "2")
            {
                Player.GetClient().SendMessage(new RoomCustomizedAlertComposer(Message[1]));
            }

            if (Message[0] == "3")
            {
                Player.GetClient().SendMessage(new MassEventComposer(Message[1]));
            }

            if (Message[0] == "4")
            {
                Player.GetClient().SendMessage(RoomNotificationComposer.SendBubble(Message[1], Message[2], Message[3]));
            }

            if (Message[0] == "5")
            {
                Player.GetClient().SendMessage(new WiredSmartAlertComposer(Message[1]));
            }

            //if (Message[0] == "6")
            //{
            //    string eventillo = "<img src=\"icon_256.png\" alt=\"xD\" height=\"15\" width=\"15\"></img>";
            //    Player.GetClient().SendMessage(new UserNameChangeComposer(Instance.Id, User.VirtualId, eventillo));
            //    Player.GetClient().SendChat("Hay un nuevo evento, haz <a href='event:navigator/goto/3'><b>click aquí</b></a> para ir al evento.        ", 33);
            //}
            return true;
        }
    }
}