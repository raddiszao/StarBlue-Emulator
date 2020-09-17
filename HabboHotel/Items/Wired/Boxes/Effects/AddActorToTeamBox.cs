using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing.Rooms.Settings;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.Games.Teams;
using StarBlue.HabboHotel.Users;
using System;
using System.Collections;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class AddActorToTeamBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectAddActorToTeam;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public int Delay { get => _delay; set { _delay = value; TickCount = value + 1; } }
        public int TickCount { get; set; }
        private int _delay = 0;
        private Queue _queue;

        public AddActorToTeamBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            TickCount = Delay;
            _queue = new Queue();
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(MessageEvent Packet)
        {
            int Unknown = Packet.PopInt();
            int Team = Packet.PopInt();
            int Unknown3 = Packet.PopInt();
            string Unknown2 = Packet.PopString();
            Delay = Packet.PopInt();

            StringData = Team.ToString();
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

                AddActor(Player);
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

        public bool AddActor(Habbo Player)
        {
            if (Instance == null || string.IsNullOrEmpty(StringData))
            {
                return false;
            }

            if (Player == null)
            {
                return false;
            }

            RoomUser User = Instance.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);
            if (User == null)
            {
                return false;
            }

            TEAM ToJoin = (int.Parse(StringData) == 1 ? TEAM.RED : int.Parse(StringData) == 2 ? TEAM.GREEN : int.Parse(StringData) == 3 ? TEAM.BLUE : int.Parse(StringData) == 4 ? TEAM.YELLOW : TEAM.NONE);

            TeamManager Team = Instance.GetTeamManagerForFreeze();
            if (Team != null)
            {
                if (Team.CanEnterOnTeam(ToJoin))
                {
                    if (User.Team != TEAM.NONE)
                    {
                        Team.OnUserLeave(User);
                    }

                    User.Team = ToJoin;
                    Team.AddUser(User);

                    if (User.GetClient().GetHabbo().Effects().CurrentEffect != Convert.ToInt32(ToJoin + 39))
                    {
                        User.GetClient().GetHabbo().Effects().ApplyEffect(Convert.ToInt32(ToJoin + 39));
                    }

                    User.GetClient().SendMessage(new HideUserOnPlaying(true));
                }
            }
            return true;
        }
    }
}