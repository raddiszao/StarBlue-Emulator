using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System.Collections.Concurrent;
using System.Text;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    class ShowMessageNux : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type => WiredBoxType.EffectShowMessageNux;

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public ShowMessageNux(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Message = Encoding.UTF8.GetString(Encoding.Default.GetBytes(Packet.PopString()));

            StringData = Message;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
            {
                return false;
            }

            Habbo Player = (Habbo)Params[0];
            if (Player == null || Player.GetClient() == null || string.IsNullOrWhiteSpace(StringData))
            {
                return false;
            }

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
            {
                return false;
            }

            string Message = StringData;

            ServerPacket notif = new ServerPacket(ServerPacketHeader.NuxAlertMessageComposer);
            notif.WriteString("habbopages/salas/tutoriales/" + Message);
            Player.GetClient().SendMessage(notif);
            return true;
        }
    }
}