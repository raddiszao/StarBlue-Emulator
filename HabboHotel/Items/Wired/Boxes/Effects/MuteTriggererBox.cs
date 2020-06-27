using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System.Collections.Concurrent;
using System.Text;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    class MuteTriggererBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectMuteTriggerer; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public MuteTriggererBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();

            if (SetItems.Count > 0)
            {
                SetItems.Clear();
            }
        }

        public void HandleSave(ClientPacket Packet)
        {
            if (SetItems.Count > 0)
            {
                SetItems.Clear();
            }

            int Unknown = Packet.PopInt();
            int Time = Packet.PopInt();
            string Message = Encoding.UTF8.GetString(Encoding.Default.GetBytes(Packet.PopString()));

            StringData = Time + ";" + Message;
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

            RoomUser User = Instance.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);
            if (User == null)
            {
                return false;
            }

            if (Player.GetPermissions().HasRight("mod_tool") || Instance.OwnerId == Player.Id)
            {
                Player.GetClient().SendWhisper("Wired Mute: Usuário impossível de mutar.", 34);
                return false;
            }

            int Time = (StringData != null ? int.Parse(StringData.Split(';')[0]) : 0);
            string Message = (StringData != null ? (StringData.Split(';')[1]) : "No message!");

            if (Time > 0)
            {
                Player.GetClient().SendWhisper("Wired Mute: Você foi mutado por " + Time + "! Razao: " + Message, 34);
                if (!Instance.MutedUsers.ContainsKey(Player.Id))
                {
                    Instance.MutedUsers.Add(Player.Id, (StarBlueServer.GetUnixTimestamp() + (Time * 60)));
                }
                else
                {
                    Instance.MutedUsers.Remove(Player.Id);
                    Instance.MutedUsers.Add(Player.Id, (StarBlueServer.GetUnixTimestamp() + (Time * 60)));
                }
            }

            return true;
        }
    }
}