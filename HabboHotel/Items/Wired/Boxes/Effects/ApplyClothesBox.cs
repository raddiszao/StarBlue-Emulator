using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    class ApplyClothesBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectApplyClothes;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public ApplyClothesBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string BotConfiguration = Packet.PopString();

            if (SetItems.Count > 0)
            {
                SetItems.Clear();
            }

            StringData = BotConfiguration;
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

            string[] Stuff = StringData.Split('\t');
            if (Stuff.Length != 2)
            {
                return false;//This is important, incase a cunt scripts.
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
            //string Username = Stuff[0];

            //RoomUser User = this.Instance.GetRoomUserManager().GetBotByName(Username);
            //if (User == null)
            //    return false;

            string Figure = Stuff[1];

            User.GetClient().GetHabbo().Look = Figure;

            ServerPacket UserChangeComposer = new ServerPacket(ServerPacketHeader.UserChangeMessageComposer);
            UserChangeComposer.WriteInteger(User.VirtualId);
            UserChangeComposer.WriteString(Figure);
            UserChangeComposer.WriteString("M");
            UserChangeComposer.WriteString(User.GetClient().GetHabbo().Motto);
            UserChangeComposer.WriteInteger(0);
            Instance.SendMessage(UserChangeComposer);

            User.GetClient().SendWhisper("Olá!", 1);
            User.GetClient().SendMessage(new AvatarAspectUpdateMessageComposer(User.GetClient().GetHabbo().Look, User.GetClient().GetHabbo().Gender));

            //User.BotData.Look = Figure;
            //User.BotData.Gender = "M";

            //using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            //{
            //    dbClient.SetQuery("UPDATE `bots` SET `look` = @look, `gender` = '" + User.BotData.Gender + "' WHERE `id` = '" + User.BotData.Id + "' LIMIT 1");
            //    dbClient.AddParameter("look", User.BotData.Look);
            //    dbClient.RunQuery();
            //}

            return true;
        }
    }
}