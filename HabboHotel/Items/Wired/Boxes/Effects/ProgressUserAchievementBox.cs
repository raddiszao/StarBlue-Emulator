using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing.Messenger;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System.Collections;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class ProgressUserAchievementBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectProgressUserAchievement;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public int Delay { get => _delay; set { _delay = value; TickCount = value + 1; } }
        public int TickCount { get; set; }
        private int _delay = 0;
        private Queue _queue;

        public ProgressUserAchievementBox(Room Instance, Item Item)
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
            string Message = Packet.PopString();
            int Unknown2 = Packet.PopInt();
            Delay = Packet.PopInt();

            StringData = Message;


            Habbo Owner = StarBlueServer.GetHabboById(Item.UserID);
            if (Owner == null || Owner.Rank < 6)
            {
                StringData = "";
                Owner.GetClient().SendWhisper("Eu não sei quem te deu isso, mas você não deveria estar brincando com brinquedos mais velhos.", 34);
                StarBlueServer.GetGame().GetClientManager().StaffAlert1(new RoomInviteComposer(int.MinValue, Owner.Username + " você está usando um Wired Reward Points sem permissão."));
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

                ProgressUserAch(Player);
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

        public bool ProgressUserAch(Habbo Player)
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

            string[] Message = StringData.Split('-');
            StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_" + Message[0], int.Parse(Message[1]));
            return true;
        }
    }
}