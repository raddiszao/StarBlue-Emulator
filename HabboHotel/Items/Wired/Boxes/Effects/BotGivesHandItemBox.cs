using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System.Collections;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class BotGivesHandItemBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectBotGivesHanditemBox;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }
        public int Delay { get => _delay; set { _delay = value; TickCount = value + 1; } }
        public int TickCount { get; set; }
        private int _delay = 0;
        private Queue _queue;

        public BotGivesHandItemBox(Room Instance, Item Item)
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
            int DrinkID = Packet.PopInt();
            string BotName = Packet.PopString();
            int Unknown2 = Packet.PopInt();
            Delay = Packet.PopInt();

            if (SetItems.Count > 0)
            {
                SetItems.Clear();
            }

            StringData = BotName.ToString() + ";" + DrinkID.ToString();
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

                BotGiveHandItem(Player);
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

        public bool BotGiveHandItem(Habbo Player)
        {
            if (string.IsNullOrEmpty(StringData))
            {
                return false;
            }

            if (Player == null)
            {
                return false;
            }

            RoomUser Actor = Instance.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);

            if (Actor == null)
            {
                return false;
            }

            RoomUser User = Instance.GetRoomUserManager().GetBotByName(StringData.Split(';')[0]);

            if (User == null)
            {
                return false;
            }

            if (User.BotData.TargetUser == 0)
            {
                if (!Instance.GetGameMap().CanWalk(Actor.SquareBehind.X, Actor.SquareBehind.Y, false))
                {
                    Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "Não consigo alcançar, chegue mais perto.", 0, 31));
                }
                else
                {
                    string[] Data = StringData.Split(';');

                    if (!int.TryParse(Data[1], out int DrinkId))
                    {
                        return false;
                    }

                    Actor.CarryItem(DrinkId);
                    Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "Aqui está sua bebida, " + Player.GetClient().GetHabbo().Username + "!", 0, 31));
                    User.MoveTo(Actor.SquareBehind.X, Actor.SquareBehind.Y);
                }
            }
            return true;
        }
    }
}
