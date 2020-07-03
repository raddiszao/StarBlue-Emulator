using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using System;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    class BotCommunicatesToAllBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectBotCommunicatesToAllBox;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public BotCommunicatesToAllBox(Room Instance, Item Item)
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

            RoomUser User = Instance.GetRoomUserManager().GetBotByName(BotName);
            if (User == null)
            {
                return false;
            }

            if (BoolData == true)
            {
                User.Shout(Chat, true, 31);
            }
            else
            {
                User.Chat(Chat, false, 31);
            }


            return true;
        }
    }
}