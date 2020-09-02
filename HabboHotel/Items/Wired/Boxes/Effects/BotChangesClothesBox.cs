using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class BotChangesClothesBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectBotChangesClothesBox;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public int Delay { get; set; } = 0 * 500;

        public int TickCount { get; set; }

        private long _next;
        private int counter = 0;
        private bool Requested = false;

        public BotChangesClothesBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string BotConfiguration = Packet.PopString();
            int Unknown2 = Packet.PopInt();
            Delay = Packet.PopInt() * 500;
            counter = 0;
            TickCount = 0;

            if (SetItems.Count > 0)
            {
                SetItems.Clear();
            }

            StringData = BotConfiguration;
        }

        public bool Execute(params object[] Params)
        {
            if (_next == 0 || _next < StarBlueServer.Now())
            {
                _next = StarBlueServer.Now() + Delay;
            }

            if (!Requested)
            {
                counter = 0;
                Requested = true;
            }

            return true;
        }

        public bool OnCycle()
        {
            if (string.IsNullOrEmpty(StringData) || !Requested || _next == 0)
            {
                return false;
            }

            counter += 500;
            if (counter > Delay)
            {
                counter = 0;

                string[] Stuff = StringData.Split('\t');
                if (Stuff.Length != 2)
                {
                    return false;//This is important, incase a cunt scripts.
                }

                string Username = Stuff[0];

                RoomUser User = Instance.GetRoomUserManager().GetBotByName(Username);
                if (User == null)
                {
                    return false;
                }

                string Figure = Stuff[1];

                ServerPacket UserChangeComposer = new ServerPacket(ServerPacketHeader.UserChangeMessageComposer);
                UserChangeComposer.WriteInteger(User.VirtualId);
                UserChangeComposer.WriteString(Figure);
                UserChangeComposer.WriteString("M");
                UserChangeComposer.WriteString(User.BotData.Motto);
                UserChangeComposer.WriteInteger(0);
                Instance.SendMessage(UserChangeComposer);

                User.BotData.Look = Figure;
                User.BotData.Gender = "M";

                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `bots` SET `look` = @look, `gender` = '" + User.BotData.Gender + "' WHERE `id` = '" + User.BotData.Id + "' LIMIT 1");
                    dbClient.AddParameter("look", User.BotData.Look);
                    dbClient.RunQuery();
                }

                Requested = false;
                _next = 0;
                return true;
            }

            return false;
        }
    }
}