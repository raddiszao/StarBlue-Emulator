using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System.Collections;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class KickUserBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectKickUser;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public int Delay { get => _delay; set { _delay = value; TickCount = value + 1; } }
        public string StringData { get; set; }
        public bool BoolData { get; set; }

        public int TickCount { get; set; }
        public string ItemsData { get; set; }

        private int _delay = 0;

        private Queue _toKick;

        public KickUserBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
            TickCount = Delay;
            _toKick = new Queue();
        }

        public void HandleSave(ClientPacket Packet)
        {
            if (SetItems.Count > 0)
            {
                SetItems.Clear();
            }

            int Unknown = Packet.PopInt();
            string Message = Packet.PopString();
            int Unknown2 = Packet.PopInt();
            Delay = Packet.PopInt();
            StringData = Message;
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

            if (!string.IsNullOrEmpty(StringData))
            {
                Player.GetClient().SendWhisper(StringData, 34);
            }

            TickCount = Delay;
            _toKick.Enqueue(Player);
            return true;
        }

        public bool OnCycle()
        {
            if (Instance == null)
            {
                return false;
            }

            if (_toKick.Count == 0)
            {
                _toKick.Clear();
                TickCount = Delay;
                return true;
            }

            while (_toKick.Count > 0)
            {
                Habbo Player = (Habbo)_toKick.Dequeue();
                if (Player == null || !Player.InRoom || Player.CurrentRoom != Instance)
                {
                    continue;
                }

                if (Player.Rank >= 7 || Instance.RoomData.OwnerId == Player.Id)
                {
                    Player.GetClient().SendWhisper("Wired Kick: Impossível expulsar este jogador.", 34);
                    return true;
                }

                Player.GetClient().GetHabbo().Effects().ApplyEffect(0);
                Instance.GetRoomUserManager().RemoveUserFromRoom(Player.GetClient(), true, false);
            }

            TickCount = Delay;
            return true;
        }
    }
}