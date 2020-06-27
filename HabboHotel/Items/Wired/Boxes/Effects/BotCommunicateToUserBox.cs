using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    class BotCommunicateToUserBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectBotCommunicatesToUserBox; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public BotCommunicateToUserBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int ChatMode = Packet.PopInt();
            string ChatConfig = Packet.PopString();

            StringData = ChatConfig;
            if (ChatMode == 1)
            {
                BoolData = true;
            }
            else
            {
                BoolData = false;
            }

        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
            {
                return false;
            }

            if (String.IsNullOrEmpty(StringData))
            {
                return false;
            }

            StringData.Split(' ');
            string BotName = StringData.Split('	')[0];
            string Chat = StringData.Split('	')[1];

            string Message = StringData.Split('	')[1];
            string MessageFiltered = StringData.Split('	')[1];

            RoomUser User = Instance.GetRoomUserManager().GetBotByName(BotName);
            if (User == null)
            {
                return false;
            }

            Habbo Player = (Habbo)Params[0];
            if (BoolData)
            {
                Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, Chat, 0, 31));
            }
            else
            {
                User.Chat(Player.GetClient().GetHabbo().Username + ": " + Chat, false, User.LastBubble);
            }

            return true;
        }
    }
}