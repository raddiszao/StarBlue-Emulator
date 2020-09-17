using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using StarBlue.HabboHotel.Users.Messenger;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class ApplyClothesBox : IWiredItem
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

        public void HandleSave(MessageEvent Packet)
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

            if (string.IsNullOrEmpty(StringData))
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
            Instance.SendMessage(new UserChangeComposer(User, false));

            User.GetClient().SendWhisper("Olá!", 1);
            User.GetClient().SendMessage(new AvatarAspectUpdateMessageComposer(User.GetClient().GetHabbo().Look, User.GetClient().GetHabbo().Gender));

            foreach (HabboHotel.Users.Messenger.MessengerBuddy buddy in Player.GetMessenger().GetFriends())
            {
                if (buddy.client == null)
                {
                    continue;
                }

                Habbo _habbo = StarBlueServer.GetHabboById(buddy.UserId);
                if (_habbo != null && _habbo.GetMessenger() != null)
                {
                    if (_habbo.GetMessenger().GetFriendsIds().TryGetValue(Player.Id, out MessengerBuddy value))
                    {
                        value.mLook = Figure;
                        _habbo.GetMessenger().UpdateFriend(Player.Id, Player.GetClient(), true);
                    }
                }
            }

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