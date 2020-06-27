using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing.Messenger;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    class ProgressUserAchievementBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectProgressUserAchievement; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public ProgressUserAchievementBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Message = Packet.PopString();

            StringData = Message;


            Habbo Owner = StarBlueServer.GetHabboById(Item.UserID);
            if (Owner == null || Owner.Rank < 6)
            {
                StringData = "";
                Owner.GetClient().SendWhisper("Eu não sei quem te deu isso, mas você não deveria estar brincando com brinquedos mais velhos.", 34);
                StarBlueServer.GetGame().GetClientManager().StaffAlert1(new RoomInviteComposer(int.MinValue, Owner.Username + " você está usando um Wired Reward Points sem permissão."));
            }
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

            var Message = StringData.Split('-');
            StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_" + Message[0], int.Parse(Message[1]));
            return true;
        }
    }
}