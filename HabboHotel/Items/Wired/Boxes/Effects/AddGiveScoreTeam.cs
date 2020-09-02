using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.Games.Teams;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    class AddGiveScoreBoxTeam : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectGiveScoreTeam; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public int Delay { get; set; } = 0 * 500;
        public int TickCount { get; set; }
        public string ItemsData { get; set; }
        private bool Requested = false;
        private int counter = 0;
        private long _next;

        public AddGiveScoreBoxTeam(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int score = Packet.PopInt();
            int times = Packet.PopInt();
            int team = Packet.PopInt();
            string Unknown3 = Packet.PopString();
            int Unknown4 = Packet.PopInt();
            int Delay = Packet.PopInt();
            this.counter = 0;
            this.TickCount = 0;
            this.Delay = Delay * 500;
            this.StringData = Convert.ToString(score + ";" + times + ";" + team);
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
                if (Instance.GotFreeze())
                    AddScoreToTeam(Instance.GetTeamManagerForFreeze());

                if (Instance.GotBanzai())
                    AddScoreToTeam(Instance.GetTeamManagerForBanzai());

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

        private void AddScoreToTeam(TeamManager t)
        {
            int mScore = int.Parse(StringData.Split(';')[0]) * int.Parse(StringData.Split(';')[1]);
            int mTeam = int.Parse(StringData.Split(';')[2]);
            bool bTeam = false;
            List<RoomUser> cTeam = new List<RoomUser>();

            switch (mTeam)
            {
                case 1:
                    bTeam = t.RedTeam.Any();
                    cTeam = t.RedTeam;
                    break;
                case 2:
                    bTeam = t.GreenTeam.Any();
                    cTeam = t.GreenTeam;
                    break;
                case 3:
                    bTeam = t.BlueTeam.Any();
                    cTeam = t.BlueTeam;
                    break;
                case 4:
                    bTeam = t.YellowTeam.Any();
                    cTeam = t.YellowTeam;
                    break;
                default:
                    bTeam = true;
                    break;
            }

            if (Instance != null)
            {
                if (bTeam)
                {
                    if ((Instance.RoomData.WiredScoreBordDay == null || Instance.RoomData.WiredScoreBordMonth == null ? false : Instance.RoomData.WiredScoreBordWeek != null))
                    {
                        KeyValuePair<int, string> newkey;
                        KeyValuePair<int, string> item;
                        int currentscore = 0;
                        foreach (Item Item in Instance.GetRoomItemHandler().GetFloor.ToList())
                        {
                            if (Item.GetBaseItem().InteractionType == InteractionType.wired_score_board)
                            {
                                foreach (RoomUser _cteam in cTeam)
                                {
                                    string username = _cteam.GetUsername();
                                    int UserId = _cteam.GetClient().GetRoomUser().UserId;
                                    lock (Instance.RoomData.WiredScoreBordDay)
                                    {
                                        if (!Instance.RoomData.WiredScoreBordDay.ContainsKey(UserId))
                                        {
                                            Instance.RoomData.WiredScoreBordDay.Add(UserId, new KeyValuePair<int, string>(mScore, username));
                                        }
                                        else
                                        {
                                            item = Instance.RoomData.WiredScoreBordDay[UserId];
                                            currentscore = (item.Key + mScore);

                                            newkey = new KeyValuePair<int, string>(currentscore, username);
                                            Instance.RoomData.WiredScoreBordDay[UserId] = newkey;
                                        }
                                    }

                                    lock (Instance.RoomData.WiredScoreBordWeek)
                                    {
                                        if (!Instance.RoomData.WiredScoreBordWeek.ContainsKey(UserId))
                                        {
                                            Instance.RoomData.WiredScoreBordWeek.Add(UserId, new KeyValuePair<int, string>(mScore, username));
                                        }
                                        else
                                        {
                                            item = Instance.RoomData.WiredScoreBordWeek[UserId];
                                            currentscore = (item.Key + mScore);

                                            newkey = new KeyValuePair<int, string>(currentscore, username);
                                            Instance.RoomData.WiredScoreBordWeek[UserId] = newkey;
                                        }
                                    }

                                    lock (Instance.RoomData.WiredScoreBordMonth)
                                    {
                                        if (!Instance.RoomData.WiredScoreBordMonth.ContainsKey(UserId))
                                        {
                                            Instance.RoomData.WiredScoreBordMonth.Add(UserId, new KeyValuePair<int, string>(mScore, username));
                                        }
                                        else
                                        {
                                            item = Instance.RoomData.WiredScoreBordMonth[UserId];
                                            currentscore = (item.Key + mScore);
                                            newkey = new KeyValuePair<int, string>(currentscore, username);
                                            Instance.RoomData.WiredScoreBordMonth[UserId] = newkey;
                                        }
                                    }
                                }
                            }
                        }

                        Instance.GetRoomItemHandler().UpdateWiredScoreBord();
                    }

                }
            }
            else
                return;
        }
    }
}