using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing.Inventory.Achievements;
using StarBlue.Communication.Packets.Outgoing.Messenger;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System;
using System.Collections;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    class AddRewardPoints : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectAddRewardPoints;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public int Delay { get => _delay; set { _delay = value; TickCount = value + 1; } }
        public int TickCount { get; set; }
        public string ItemsData { get; set; }

        private Queue _queue;
        private int _delay = 0;

        public AddRewardPoints(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();

            _queue = new Queue();
            TickCount = Delay;
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int Score = Packet.PopInt();
            int times = Packet.PopInt();
            string Unknown2 = Packet.PopString();
            int Unknown3 = Packet.PopInt();
            int Delay = Packet.PopInt();

            this.Delay = Delay;
            StringData = Convert.ToString(Score + ";" + times);

            Habbo Owner = StarBlueServer.GetHabboById(Item.UserID);

            if (times > 3)
            {
                Owner.GetClient().SendWhisper("Você não pode dar o prêmio mais de três vezes.", 34);
                return;
            }

            if (Owner == null || Owner.Rank < 6)
            {
                StringData = Convert.ToString(0 + ";" + times);
                Owner.GetClient().SendWhisper("Eu não sei quem te deu isso, mas você não deveria estar brincando com brinquedos mais velhoss.", 34);
                StarBlueServer.GetGame().GetClientManager().StaffAlert1(new RoomInviteComposer(int.MinValue, Owner.Username + " você está usando um Wired Reward Points sem permissão."));
            }
            return;

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

                TeleportUser(Player);
            }

            TickCount = Delay;
            return true;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
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

        private void TeleportUser(Habbo Player)
        {
            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);
            if (User == null)
            {
                return;
            }

            Room Instance = Player.CurrentRoom;

            int RewardPoints = int.Parse(StringData.Split(';')[0]) * int.Parse(StringData.Split(';')[1]);

            User.GetClient().GetHabbo().GetStats().AchievementPoints += RewardPoints;
            User.GetClient().SendMessage(new AchievementScoreComposer(User.GetClient().GetHabbo().GetStats().AchievementPoints));
            User.GetClient().SendMessage(RoomNotificationComposer.SendBubble("RewardPoints", "Felicidades, acabas de ganar " + RewardPoints + " puntos de recompensa.", ""));
        }
    }
}
