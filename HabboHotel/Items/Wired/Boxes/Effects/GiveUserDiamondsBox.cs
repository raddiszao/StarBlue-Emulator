using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    class GiveUserDiamondsBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type => WiredBoxType.EffectGiveUserDiamonds;

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public GiveUserDiamondsBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Diamonds = Packet.PopString();

            StringData = Diamonds;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
            {
                return false;
            }

            Habbo Owner = StarBlueServer.GetHabboById(Item.UserID);
            if (Owner == null || !Owner.GetPermissions().HasRight("room_item_wired_rewards"))
            {
                return false;
            }

            Habbo Player = (Habbo)Params[0];
            if (Player == null || Player.GetClient() == null)
            {
                return false;
            }

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
            {
                return false;
            }

            if (String.IsNullOrEmpty(StringData))
            {
                return false;
            }

            int Amount;
            Amount = Convert.ToInt32(StringData);
            if (Amount > 5000)
            {
                Player.GetClient().SendWhisper("A quantidade de diamantes ultrapassa os limites.");
                return false;
            }
            else
            {
                Player.GetClient().GetHabbo().Diamonds += Amount;
                Player.GetClient().SendMessage(new ActivityPointsComposer(Player.GetClient().GetHabbo().Duckets, Player.GetClient().GetHabbo().Diamonds, Player.GetClient().GetHabbo().GOTWPoints));
                Player.GetClient().SendMessage(RoomNotificationComposer.SendBubble("diamonds", "" + Player.GetClient().GetHabbo().Username + ", acaba de receber " + Amount + " diamante(s).", ""));

            }

            return true;
        }
    }
}