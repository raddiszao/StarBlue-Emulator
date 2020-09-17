using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing.Rooms.Settings;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.Games.Teams;
using StarBlue.HabboHotel.Users;
using System.Collections;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class RemoveActorFromTeamBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectRemoveActorFromTeam;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public int Delay { get => _delay; set { _delay = value; TickCount = value + 1; } }
        public int TickCount { get; set; }
        private int _delay = 0;
        private Queue _queue;

        public RemoveActorFromTeamBox(Room Instance, Item Item)
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
            string Unknown2 = Packet.PopString();
            int Unknown3 = Packet.PopInt();
            Delay = Packet.PopInt();
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

                RemoveActor(Player);
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

        public bool RemoveActor(Habbo Player)
        {
            if (Player == null || Instance == null)
            {
                return false;
            }

            RoomUser User = Instance.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);
            if (User == null)
            {
                return false;
            }

            if (User.Team != TEAM.NONE)
            {
                TeamManager Team = Instance.GetTeamManagerForFreeze();
                if (Team != null)
                {
                    Team.OnUserLeave(User);

                    User.Team = TEAM.NONE;

                    if (User.GetClient().GetHabbo().Effects().CurrentEffect != 0)
                    {
                        User.GetClient().GetHabbo().Effects().ApplyEffect(0);
                    }

                    User.GetClient().SendMessage(new HideUserOnPlaying(false));
                }
            }
            return true;
        }
    }
}