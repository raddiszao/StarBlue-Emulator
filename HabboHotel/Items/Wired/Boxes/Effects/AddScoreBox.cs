using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class AddScoreBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectAddScore;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public int Delay { get => _delay; set { _delay = value; TickCount = value + 1; } }
        public int TickCount { get; set; }
        public string ItemsData { get; set; }

        private Queue _queue;
        private int _delay = 0;

        public AddScoreBox(Room Instance, Item Item)
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
            int score = Packet.PopInt();
            int times = Packet.PopInt();
            string Unknown2 = Packet.PopString();
            int Unknown3 = Packet.PopInt();
            int Delay = Packet.PopInt();

            this.Delay = Delay;
            StringData = Convert.ToString(score + ";" + times);

            // this.Delay = Packet.PopInt();
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

                AddScore(Player);
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

            TickCount = Delay;
            _queue.Enqueue(Player);
            return true;
        }

        private void AddScore(Habbo Player)
        {
            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);
            if (User == null || (!int.TryParse(StringData.Split(';')[0], out int m1) || !int.TryParse(StringData.Split(';')[1], out int m2)))
            {
                return;
            }

            Room Instance = Player.CurrentRoom;

            int currentscore = 0;
            int mScore = m1 * m2;
            KeyValuePair<int, string> newkey;
            KeyValuePair<int, string> item;

            if ((Instance == null || User == null ? false : !User.IsBot))
            {
                Instance.GetRoomItemHandler().usedwiredscorebord = true;

                if (Instance.RoomData.WiredScoreFirstBordInformation.Count == 3)
                {
                    Instance.GetRoomItemHandler().ScorebordChangeCheck();
                }

                if ((Instance.RoomData.WiredScoreBordDay == null || Instance.RoomData.WiredScoreBordMonth == null ? false : Instance.RoomData.WiredScoreBordWeek != null))
                {
                    string username = User.GetClient().GetHabbo().Username;

                    lock (Instance.RoomData.WiredScoreBordDay)
                    {
                        if (!Instance.RoomData.WiredScoreBordDay.ContainsKey(User.UserId))
                        {
                            Instance.RoomData.WiredScoreBordDay.Add(User.UserId, new KeyValuePair<int, string>(mScore, username));
                        }
                        else
                        {
                            item = Instance.RoomData.WiredScoreBordDay[User.UserId];
                            currentscore = (item.Key + mScore);

                            newkey = new KeyValuePair<int, string>(currentscore, username);
                            Instance.RoomData.WiredScoreBordDay[User.UserId] = newkey;
                        }
                    }

                    lock (Instance.RoomData.WiredScoreBordWeek)
                    {
                        if (!Instance.RoomData.WiredScoreBordWeek.ContainsKey(User.UserId))
                        {
                            Instance.RoomData.WiredScoreBordWeek.Add(User.UserId, new KeyValuePair<int, string>(mScore, username));
                        }
                        else
                        {
                            item = Instance.RoomData.WiredScoreBordWeek[User.UserId];
                            currentscore = (item.Key + mScore);

                            newkey = new KeyValuePair<int, string>(currentscore, username);
                            Instance.RoomData.WiredScoreBordWeek[User.UserId] = newkey;
                        }
                    }

                    lock (Instance.RoomData.WiredScoreBordMonth)
                    {
                        if (!Instance.RoomData.WiredScoreBordMonth.ContainsKey(User.UserId))
                        {
                            Instance.RoomData.WiredScoreBordMonth.Add(User.UserId, new KeyValuePair<int, string>(mScore, username));
                        }
                        else
                        {
                            item = Instance.RoomData.WiredScoreBordMonth[User.UserId];
                            currentscore = (item.Key + mScore);
                            newkey = new KeyValuePair<int, string>(currentscore, username);
                            Instance.RoomData.WiredScoreBordMonth[User.UserId] = newkey;
                        }
                    }
                    //Instance.GetWired().ExecuteWired(WiredItemType.TriggerScoreAchieved, User, currentscore);
                }

                Instance.GetRoomItemHandler().UpdateWiredScoreBord();

                //if (Player.Effects() != null)
                //    Player.Effects().ApplyEffect(0);
            }
        }
    }
}