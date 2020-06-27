using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Interfaces;
using StarBlue.Communication.Packets.Outgoing;
using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Communication.Packets.Outgoing.Rooms.Avatar;
using StarBlue.Communication.Packets.Outgoing.Rooms.Camera;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Communication.Packets.Outgoing.Rooms.Poll;
using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using StarBlue.Core;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Groups;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Items.Data.Moodlight;
using StarBlue.HabboHotel.Items.Data.Toner;
using StarBlue.HabboHotel.Rooms.AI;
using StarBlue.HabboHotel.Rooms.AI.Speech;
using StarBlue.HabboHotel.Rooms.Games;
using StarBlue.HabboHotel.Rooms.Games.Banzai;
using StarBlue.HabboHotel.Rooms.Games.Football;
using StarBlue.HabboHotel.Rooms.Games.Freeze;
using StarBlue.HabboHotel.Rooms.Games.Teams;
using StarBlue.HabboHotel.Rooms.Instance;
using StarBlue.HabboHotel.Rooms.Trading;
using StarBlue.HabboHotel.Rooms.TraxMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBlue.HabboHotel.Rooms
{
    public class Room : RoomData
    {
        public bool isCrashed;
        public bool mDisposed;
        public bool RoomMuted;
        public bool DiscoMode;
        public bool muteSignalEnabled;
        public DateTime lastTimerReset;
        public DateTime lastRegeneration;
        public int BallSpeed;

        public delegate void FurnisLoaded();
        public event FurnisLoaded OnFurnisLoad;

        public Task ProcessTask;
        public ArrayList ActiveTrades;

        public TonerData TonerData;
        public MoodlightData MoodlightData;

        public Dictionary<int, double> Bans;
        public Dictionary<int, double> MutedUsers;

        internal bool ForSale;
        internal int SalePrice;
        private Dictionary<int, List<RoomUser>> Tents;

        internal string poolQuestion;

        public List<int> UsersWithRights;
        private GameManager _gameManager;
        private Freeze _freeze;
        private Soccer _soccer;
        private BattleBanzai _banzai;

        private Gamemap _gamemap;
        private GameItemHandler _gameItemHandler;

        private RoomData _roomData;
        public TeamManager teambanzai;
        public TeamManager teamfreeze;
        public RoomTraxManager _traxManager;

        private RoomUserManager _roomUserManager;
        private RoomItemHandling _roomItemHandling;

        private List<string> _wordFilterList;

        private FilterComponent _filterComponent = null;
        private WiredComponent _wiredComponent = null;

        internal List<int> yesPoolAnswers;
        internal List<int> noPoolAnswers;

        public int IsLagging { get; set; }
        public int IdleTime { get; set; }
        private bool _hideWired;
        public int ForSaleAmount;
        public bool RoomForSale;

        public Room(RoomData Data)
        {
            IsLagging = 0;
            IdleTime = 0;

            _roomData = Data;
            RoomMuted = false;
            mDisposed = false;
            muteSignalEnabled = false;

            Id = Data.Id;
            Name = Data.Name;
            Description = Data.Description;
            OwnerName = Data.OwnerName;
            OwnerId = Data.OwnerId;

            WiredScoreBordDay = Data.WiredScoreBordDay;
            WiredScoreBordWeek = Data.WiredScoreBordWeek;
            WiredScoreBordMonth = Data.WiredScoreBordMonth;
            WiredScoreFirstBordInformation = Data.WiredScoreFirstBordInformation;

            ForSale = false;
            SalePrice = 0;
            Category = Data.Category;
            Type = Data.Type;
            Access = Data.Access;
            UsersNow = 0;
            UsersMax = Data.UsersMax;
            ModelName = Data.ModelName;
            Score = Data.Score;
            Tags = new List<string>();
            foreach (string tag in Data.Tags)
            {
                Tags.Add(tag);
            }

            AllowPets = Data.AllowPets;
            AllowPetsEating = Data.AllowPetsEating;
            RoomBlockingEnabled = Data.RoomBlockingEnabled;
            Hidewall = Data.Hidewall;
            Group = Data.Group;

            Password = Data.Password;
            Wallpaper = Data.Wallpaper;
            Floor = Data.Floor;
            Landscape = Data.Landscape;

            WallThickness = Data.WallThickness;
            FloorThickness = Data.FloorThickness;

            chatMode = Data.chatMode;
            chatSize = Data.chatSize;
            chatSpeed = Data.chatSpeed;
            chatDistance = Data.chatDistance;
            extraFlood = Data.extraFlood;

            TradeSettings = Data.TradeSettings;

            WhoCanBan = Data.WhoCanBan;
            WhoCanKick = Data.WhoCanKick;
            WhoCanBan = Data.WhoCanBan;

            PushEnabled = Data.PushEnabled;
            PullEnabled = Data.PullEnabled;
            SPullEnabled = Data.SPullEnabled;
            SPushEnabled = Data.SPushEnabled;
            EnablesEnabled = Data.EnablesEnabled;
            HideWired = Data.HideWired;
            RespectNotificationsEnabled = Data.RespectNotificationsEnabled;
            PetMorphsAllowed = Data.PetMorphsAllowed;
            Shoot = Data.Shoot;

            poolQuestion = string.Empty;
            yesPoolAnswers = new List<int>();
            noPoolAnswers = new List<int>();

            ActiveTrades = new ArrayList();
            Bans = new Dictionary<int, double>();
            MutedUsers = new Dictionary<int, double>();
            Tents = new Dictionary<int, List<RoomUser>>();

            _gamemap = new Gamemap(this);
            if (_roomItemHandling == null)
            {
                _roomItemHandling = new RoomItemHandling(this);
            }

            _roomUserManager = new RoomUserManager(this);

            _filterComponent = new FilterComponent(this);
            _wiredComponent = new WiredComponent(this);
            if (_traxManager == null)
            {
                _traxManager = new RoomTraxManager(this);
            }

            GetRoomItemHandler().LoadFurniture();
            GetGameMap().GenerateMaps();

            LoadPromotions();
            LoadRights();
            LoadBans();
            LoadFilter();
            InitBots();
            InitPets();
        }

        public List<string> WordFilterList
        {
            get { return _wordFilterList; }
            set { _wordFilterList = value; }
        }

        #region Room Bans

        public bool UserIsBanned(int pId)
        {
            return Bans.ContainsKey(pId);
        }

        public void RemoveBan(int pId)
        {
            Bans.Remove(pId);
        }

        public void AddBan(int pId, long Time)
        {
            if (!Bans.ContainsKey(Convert.ToInt32(pId)))
            {
                Bans.Add(pId, StarBlueServer.GetUnixTimestamp() + Time);
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("REPLACE INTO `room_bans` VALUES (" + pId + ", " + Id + ", " + (StarBlueServer.GetUnixTimestamp() + Time) + ")");
            }
        }

        public List<int> BannedUsers()
        {
            var Bans = new List<int>();

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT user_id FROM room_bans WHERE expire > UNIX_TIMESTAMP() AND room_id=" + Id);
                DataTable Table = dbClient.GetTable();

                foreach (DataRow Row in Table.Rows)
                {
                    Bans.Add(Convert.ToInt32(Row[0]));
                }
            }

            return Bans;
        }

        public bool HasBanExpired(int pId)
        {
            if (!UserIsBanned(pId))
            {
                return true;
            }

            if (Bans[pId] < StarBlueServer.GetUnixTimestamp())
            {
                return true;
            }

            return false;
        }

        public void Unban(int UserId)
        {
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("DELETE FROM `room_bans` WHERE `user_id` = '" + UserId + "' AND `room_id` = '" + Id + "' LIMIT 1");
            }

            if (Bans.ContainsKey(UserId))
            {
                Bans.Remove(UserId);
            }
        }

        #endregion

        #region Trading

        public bool HasActiveTrade(RoomUser User)
        {
            if (User.IsBot)
            {
                return false;
            }

            return HasActiveTrade(User.GetClient().GetHabbo().Id);
        }

        public bool HasActiveTrade(int UserId)
        {
            if (ActiveTrades.Count == 0)
            {
                return false;
            }

            foreach (Trade Trade in ActiveTrades.ToArray())
            {
                if (Trade.ContainsUser(UserId))
                {
                    return true;
                }
            }
            return false;
        }

        public Trade GetUserTrade(int UserId)
        {
            foreach (Trade Trade in ActiveTrades.ToArray())
            {
                if (Trade.ContainsUser(UserId))
                {
                    return Trade;
                }
            }

            return null;
        }

        public void TryStartTrade(RoomUser UserOne, RoomUser UserTwo)
        {
            if (UserOne == null || UserTwo == null || UserOne.IsBot || UserTwo.IsBot || UserOne.IsTrading ||
                UserTwo.IsTrading || HasActiveTrade(UserOne) || HasActiveTrade(UserTwo))
            {
                return;
            }

            ActiveTrades.Add(new Trade(UserOne.GetClient().GetHabbo().Id, UserTwo.GetClient().GetHabbo().Id, RoomId));
        }

        public void TryStopTrade(int UserId)
        {
            Trade Trade = GetUserTrade(UserId);

            if (Trade == null)
            {
                return;
            }

            Trade.CloseTrade(UserId);
            ActiveTrades.Remove(Trade);
        }

        #endregion


        public int UserCount
        {
            get { return _roomUserManager.GetRoomUsers().Count; }
        }

        public int RoomId
        {
            get { return Id; }
        }

        public bool CanTradeInRoom
        {
            get { return true; }
        }

        public List<ServerPacket> HideWiredMessages(bool hideWired)
        {
            List<ServerPacket> list = new List<ServerPacket>();
            Item[] items = GetRoomItemHandler().GetFloor.ToArray();
            if (hideWired)
            {
                for (int i = 0; i < items.Count(); i++)
                {
                    Item item = items[i];
                    if (!item.IsWired)
                    {
                        continue;
                    }

                    list.Add(new ObjectRemoveComposer(item, 0));
                }
            }
            else
            {
                for (int i = 0; i < items.Count(); i++)
                {
                    Item item = items[i];
                    if (!item.IsWired)
                    {
                        continue;
                    }

                    list.Add(new ObjectAddComposer(item, this));
                }
            }
            return list;
        }

        public RoomData RoomData
        {
            get { return _roomData; }
        }

        public Gamemap GetGameMap()
        {
            return _gamemap;
        }

        public RoomItemHandling GetRoomItemHandler()
        {
            if (_roomItemHandling == null)
            {
                _roomItemHandling = new RoomItemHandling(this);
            }
            return _roomItemHandling;
        }

        public RoomUserManager GetRoomUserManager()
        {
            return _roomUserManager;
        }

        public Soccer GetSoccer()
        {
            if (_soccer == null)
            {
                _soccer = new Soccer(this);
            }

            return _soccer;
        }

        public TeamManager GetTeamManagerForBanzai()
        {
            if (teambanzai == null)
            {
                teambanzai = TeamManager.createTeamforGame("banzai");
            }

            return teambanzai;
        }

        public TeamManager GetTeamManagerForFreeze()
        {
            if (teamfreeze == null)
            {
                teamfreeze = TeamManager.createTeamforGame("freeze");
            }

            return teamfreeze;
        }

        public BattleBanzai GetBanzai()
        {
            if (_banzai == null)
            {
                _banzai = new BattleBanzai(this);
            }

            return _banzai;
        }

        public Freeze GetFreeze()
        {
            if (_freeze == null)
            {
                _freeze = new Freeze(this);
            }

            return _freeze;
        }

        public GameManager GetGameManager()
        {
            if (_gameManager == null)
            {
                _gameManager = new GameManager(this);
            }

            return _gameManager;
        }

        public GameItemHandler GetGameItemHandler()
        {
            if (_gameItemHandler == null)
            {
                _gameItemHandler = new GameItemHandler(this);
            }

            return _gameItemHandler;
        }

        public bool GotSoccer()
        {
            return (_soccer != null);
        }

        public bool GotBanzai()
        {
            return (_banzai != null);
        }

        public bool GotFreeze()
        {
            return (_freeze != null);
        }

        public void ClearTags()
        {
            Tags.Clear();
        }

        public void setPoolQuestion(String pool)
        {
            poolQuestion = pool;
        }

        public void clearPoolAnswers()
        {
            yesPoolAnswers.Clear();
            noPoolAnswers.Clear();
        }

        public void startQuestion(String question)
        {
            setPoolQuestion(question);
            clearPoolAnswers();

            SendMessage(new QuickPollMessageComposer(question));
        }
        public void endQuestion()
        {
            setPoolQuestion(string.Empty);
            SendMessage(new QuickPollResultsMessageComposer(yesPoolAnswers.Count, noPoolAnswers.Count));


            clearPoolAnswers();
        }

        public void AddTagRange(List<string> tags)
        {
            Tags.AddRange(tags);
        }

        public void InitBots()
        {
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`room_id`,`name`,`motto`,`look`,`x`,`y`,`z`,`rotation`,`gender`,`user_id`,`ai_type`,`walk_mode`,`automatic_chat`,`speaking_interval`,`mix_sentences`,`chat_bubble` FROM `bots` WHERE `room_id` = '" + RoomId + "' AND `ai_type` != 'pet'");
                DataTable Data = dbClient.GetTable();
                if (Data == null)
                {
                    return;
                }

                foreach (DataRow Bot in Data.Rows)
                {
                    dbClient.SetQuery("SELECT `text` FROM `bots_speech` WHERE `bot_id` = '" + Convert.ToInt32(Bot["id"]) + "'");
                    DataTable BotSpeech = dbClient.GetTable();

                    List<RandomSpeech> Speeches = new List<RandomSpeech>();

                    foreach (DataRow Speech in BotSpeech.Rows)
                    {
                        Speeches.Add(new RandomSpeech(Convert.ToString(Speech["text"]), Convert.ToInt32(Bot["id"])));
                    }

                    _roomUserManager.DeployBot(new RoomBot(Convert.ToInt32(Bot["id"]), Convert.ToInt32(Bot["room_id"]), Convert.ToString(Bot["ai_type"]), Convert.ToString(Bot["walk_mode"]), Convert.ToString(Bot["name"]), Convert.ToString(Bot["motto"]), Convert.ToString(Bot["look"]), int.Parse(Bot["x"].ToString()), int.Parse(Bot["y"].ToString()), int.Parse(Bot["z"].ToString()), int.Parse(Bot["rotation"].ToString()), 0, 0, 0, 0, ref Speeches, "M", 0, Convert.ToInt32(Bot["user_id"].ToString()), Convert.ToBoolean(Bot["automatic_chat"]), Convert.ToInt32(Bot["speaking_interval"]), StarBlueServer.EnumToBool(Bot["mix_sentences"].ToString()), Convert.ToInt32(Bot["chat_bubble"])), null);
                }
            }
        }

        public void InitPets()
        {
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`user_id`,`room_id`,`name`,`x`,`y`,`z` FROM `bots` WHERE `room_id` = '" + RoomId + "' AND `ai_type` = 'pet'");
                DataTable Data = dbClient.GetTable();

                if (Data == null)
                {
                    return;
                }

                foreach (DataRow Row in Data.Rows)
                {
                    dbClient.SetQuery("SELECT `type`,`race`,`color`,`experience`,`energy`,`nutrition`,`respect`,`createstamp`,`have_saddle`,`anyone_ride`,`hairdye`,`pethair`,`gnome_clothing` FROM `bots_petdata` WHERE `id` = '" + Row[0] + "' LIMIT 1");
                    DataRow mRow = dbClient.GetRow();
                    if (mRow == null)
                    {
                        continue;
                    }

                    Pet Pet = new Pet(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["user_id"]), Convert.ToInt32(Row["room_id"]), Convert.ToString(Row["name"]), Convert.ToInt32(mRow["type"]), Convert.ToString(mRow["race"]),
                        Convert.ToString(mRow["color"]), Convert.ToInt32(mRow["experience"]), Convert.ToInt32(mRow["energy"]), Convert.ToInt32(mRow["nutrition"]), Convert.ToInt32(mRow["respect"]), Convert.ToDouble(mRow["createstamp"]), Convert.ToInt32(Row["x"]), Convert.ToInt32(Row["y"]),
                        Convert.ToDouble(Row["z"]), Convert.ToInt32(mRow["have_saddle"]), Convert.ToInt32(mRow["anyone_ride"]), Convert.ToInt32(mRow["hairdye"]), Convert.ToInt32(mRow["pethair"]), Convert.ToString(mRow["gnome_clothing"]));

                    var RndSpeechList = new List<RandomSpeech>();

                    _roomUserManager.DeployBot(new RoomBot(Pet.PetId, RoomId, "pet", "freeroam", Pet.Name, "", Pet.Look, Pet.X, Pet.Y, Convert.ToInt32(Pet.Z), 0, 0, 0, 0, 0, ref RndSpeechList, "", 0, Pet.OwnerId, false, 0, false, 0), Pet);
                }
            }
        }

        public FilterComponent GetFilter()
        {
            return _filterComponent;
        }

        public WiredComponent GetWired()
        {
            return _wiredComponent;
        }

        public void LoadPromotions()
        {
            DataRow GetPromotion = null;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `room_promotions` WHERE `room_id` = " + Id + " LIMIT 1;");
                GetPromotion = dbClient.GetRow();

                if (GetPromotion != null)
                {
                    if (Convert.ToDouble(GetPromotion["timestamp_expire"]) > StarBlueServer.GetUnixTimestamp())
                    {
                        RoomData._promotion = new RoomPromotion(Convert.ToString(GetPromotion["title"]), Convert.ToString(GetPromotion["description"]), Convert.ToDouble(GetPromotion["timestamp_start"]), Convert.ToDouble(GetPromotion["timestamp_expire"]), Convert.ToInt32(GetPromotion["category_id"]));
                    }
                }
            }
        }

        public void LoadRights()
        {
            UsersWithRights = new List<int>();
            if (Group != null)
            {
                return;
            }

            DataTable Data = null;

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT room_rights.user_id FROM room_rights WHERE room_id = @roomid");
                dbClient.AddParameter("roomid", Id);
                Data = dbClient.GetTable();
            }

            if (Data != null)
            {
                foreach (DataRow Row in Data.Rows)
                {
                    UsersWithRights.Add(Convert.ToInt32(Row["user_id"]));
                }
            }
        }

        private void LoadFilter()
        {
            _wordFilterList = new List<string>();

            DataTable Data = null;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `room_filter` WHERE `room_id` = @roomid;");
                dbClient.AddParameter("roomid", Id);
                Data = dbClient.GetTable();
            }

            if (Data == null)
            {
                return;
            }

            foreach (DataRow Row in Data.Rows)
            {
                _wordFilterList.Add(Convert.ToString(Row["word"]));
            }
        }

        public void LoadBans()
        {
            this.Bans = new Dictionary<int, double>();

            DataTable Bans;

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT user_id, expire FROM room_bans WHERE room_id = " + Id);
                Bans = dbClient.GetTable();
            }

            if (Bans == null)
            {
                return;
            }

            foreach (DataRow ban in Bans.Rows)
            {
                this.Bans.Add(Convert.ToInt32(ban[0]), Convert.ToDouble(ban[1]));
            }
        }

        public bool CheckRights(GameClient Session)
        {
            return CheckRights(Session, false);
        }

        public bool CheckRights(GameClient Session, bool RequireOwnership, bool CheckForGroups = false)
        {
            try
            {
                if (Session == null || Session.GetHabbo() == null)
                {
                    return false;
                }

                if (Session.GetHabbo().Username == OwnerName && Type == "private")
                {
                    return true;
                }

                if (Session.GetHabbo().GetPermissions().HasRight("room_any_owner"))
                {
                    return true;
                }

                if (!RequireOwnership && Type == "private")
                {
                    if (Session.GetHabbo().GetPermissions().HasRight("room_any_rights"))
                    {
                        return true;
                    }

                    if (UsersWithRights.Contains(Session.GetHabbo().Id))
                    {
                        return true;
                    }
                }

                if (CheckForGroups && Type == "private")
                {
                    if (Group == null)
                    {
                        return false;
                    }

                    if (Group.IsAdmin(Session.GetHabbo().Id))
                    {
                        return true;
                    }

                    if (Group.AdminOnlyDeco == 0)
                    {
                        if (Group.IsAdmin(Session.GetHabbo().Id))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception e) { Logging.HandleException(e, "Room.CheckRights"); }
            return false;
        }

        public RoomTraxManager GetTraxManager()
        {
            if (_traxManager == null)
            {
                _traxManager = new RoomTraxManager(this);
            }
            return _traxManager;
        }

        public bool GotMusicManager()
        {
            return (_traxManager != null);
        }

        public void AssignNewOwner(Room ForSale, RoomUser Buyer, RoomUser Seller)
        {
            using (IQueryAdapter Adapter = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                Adapter.SetQuery("UPDATE rooms SET owner = @new WHERE id = @id");
                Adapter.AddParameter("new", Buyer.HabboId);
                Adapter.AddParameter("id", ForSale.RoomData.Id);
                Adapter.RunQuery();

                Adapter.SetQuery("UPDATE items SET user_id = @new WHERE id = @id");
                Adapter.AddParameter("new", Buyer.HabboId);
                Adapter.AddParameter("id", ForSale.RoomData.Id);
                Adapter.RunQuery();

                Adapter.SetQuery("DELETE FROM room_rights WHERE room_id = @id");
                Adapter.AddParameter("id", ForSale.RoomData.Id);

                if (ForSale.Group != null)
                {
                    Adapter.SetQuery("SELECT id FROM groups WHERE room_id = @id");
                    Adapter.AddParameter("id", ForSale.RoomData.Id);
                    int GroupID = Adapter.GetInteger();
                    if (GroupID > 0)
                    {
                        ForSale.Group.ClearRequests();
                        foreach (KeyValuePair<int, string> Data in ForSale.Group.GetAllMembers)
                        {
                            ForSale.Group.DeleteMember(Data.Key);
                            GameClient UserClient = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Data.Key);
                            if (UserClient == null)
                            {
                                continue;
                            }

                            if (UserClient.GetHabbo().GetStats().FavouriteGroupId == GroupID)
                            {
                                UserClient.GetHabbo().GetStats().FavouriteGroupId = 0;
                            }
                        }
                        Adapter.RunFastQuery("DELETE FROM `groups` WHERE `id` = @id = '" + ForSale.Group.Id + "'");
                        Adapter.RunFastQuery("DELETE FROM `group_memberships` WHERE group_id = '" + ForSale.Group.Id + "'");
                        Adapter.RunFastQuery("DELETE FROM `group_requests` WHERE group_id = '" + ForSale.Group.Id + "'");
                        Adapter.RunFastQuery("UPDATE `rooms` SET `group_id` = '0' WHERE `group_id` = '" + ForSale.Group.Id + "'");
                        Adapter.RunFastQuery("UPDATE `user_stats` SET `group_id` = '0' WHERE group_id = '" + ForSale.Group.Id + "'");
                        Adapter.RunFastQuery("DELETE FROM `items_groups` WHERE `group_id` = '" + ForSale.Group.Id + "'");
                    }
                    StarBlueServer.GetGame().GetGroupManager().DeleteGroup(ForSale.Group.Id);
                    ForSale.RoomData.Group = null;
                    ForSale.Group = null;
                }
            }
            ForSale.RoomData.OwnerId = Buyer.HabboId;
            ForSale.RoomData.OwnerName = Buyer.GetClient().GetHabbo().Username;

            foreach (Item Item in ForSale.GetRoomItemHandler().GetWallAndFloor)
            {
                Item.UserID = Buyer.HabboId;
                Item.Username = Buyer.GetClient().GetHabbo().Username;
            }

            Seller.GetClient().GetHabbo().Duckets += ForSale.ForSaleAmount;
            Seller.GetClient().SendMessage(new HabboActivityPointNotificationComposer(Buyer.GetClient().GetHabbo().Duckets, ForSale.ForSaleAmount));
            Buyer.GetClient().GetHabbo().Duckets -= ForSale.ForSaleAmount;
            Buyer.GetClient().SendMessage(new HabboActivityPointNotificationComposer(Buyer.GetClient().GetHabbo().Duckets, ForSale.ForSaleAmount));

            ForSale.RoomForSale = false;
            ForSale.ForSaleAmount = 0;

            Buyer.GetClient().GetHabbo().UsersRooms.Add(ForSale);
            Buyer.GetClient().GetHabbo().UsersRooms.Remove(ForSale);

            Room Room = null;
            List<RoomUser> UsersReturn = ForSale.GetRoomUserManager().GetRoomUsers().ToList();
            StarBlueServer.GetGame().GetNavigator().Init();
            StarBlueServer.GetGame().GetRoomManager().UnloadRoom(Room.Id, true);
            foreach (RoomUser User in UsersReturn)
            {
                if (User == null || User.GetClient() == null)
                {
                    continue;
                }

                User.GetClient().SendMessage(new RoomForwardComposer(ForSale.RoomId));
                User.GetClient().SendNotification("A sala onde você está acabou de ser comprada por " + Buyer.GetClient().GetHabbo().Username + " no total " + ForSale.ForSaleAmount + " duckets!");
            }
        }

        public bool OnUserShoot(RoomUser User, Item Ball)
        {
            Func<Item, bool> predicate = null;
            string Key = null;
            foreach (Item item in GetRoomItemHandler().GetFurniObjects(Ball.GetX, Ball.GetY).ToList())
            {
                if (item.GetBaseItem().ItemName.StartsWith("fball_goal_"))
                {
                    Key = item.GetBaseItem().ItemName.Split(new char[] { '_' })[2];
                    User.UnIdle();
                    User.DanceId = 0;
                    StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_FootballGoalScored", 1);
                    SendMessage(new ActionComposer(User.VirtualId, 1));
                }
            }
            if (Key != null)
            {
                if (predicate == null)
                {
                    predicate = p => p.GetBaseItem().ItemName == ("fball_score_" + Key);
                }
                foreach (Item item2 in GetRoomItemHandler().GetFloor.Where<Item>(predicate).ToList())
                {
                    if (item2.GetBaseItem().ItemName == ("fball_score_" + Key))
                    {
                        if (!String.IsNullOrEmpty(item2.ExtraData))
                        {
                            item2.ExtraData = (Convert.ToInt32(item2.ExtraData) + 1).ToString();
                        }
                        else
                        {
                            item2.ExtraData = "1";
                        }

                        item2.UpdateState();
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void OnUserShoot(GameClient client, Item Ball)
        {
            Func<Item, bool> predicate = null;
            string Key = null;

            if (client == null)
            {
                return;
            }

            RoomUser User = Ball.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(client.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            foreach (Item item in GetRoomItemHandler().GetFurniObjects(Ball.GetX, Ball.GetY).ToList())
            {
                if (item.GetBaseItem().ItemName.StartsWith("fball_goal_"))
                {
                    Key = item.GetBaseItem().ItemName.Split(new char[] { '_' })[2];
                    User.UnIdle();
                    User.DanceId = 0;

                    StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_FootballGoalScored", 1);

                    SendMessage(new ActionComposer(User.VirtualId, 1));
                }
            }

            if (Key != null)
            {
                if (predicate == null)
                {
                    predicate = p => p.GetBaseItem().ItemName == ("fball_score_" + Key);
                }

                foreach (Item item2 in GetRoomItemHandler().GetFloor.Where<Item>(predicate).ToList())
                {
                    if (item2.GetBaseItem().ItemName == ("fball_score_" + Key))
                    {
                        if (!String.IsNullOrEmpty(item2.ExtraData))
                        {
                            item2.ExtraData = (Convert.ToInt32(item2.ExtraData) + 1).ToString();
                        }
                        else
                        {
                            item2.ExtraData = "1";
                        }

                        item2.UpdateState();
                    }
                }
            }
        }

        public void ProcessRoom()
        {
            if (isCrashed || mDisposed)
            {
                return;
            }

            try
            {
                if (GetRoomUserManager().GetRoomUsers().Count == 0)
                {
                    IdleTime++;
                }
                else if (IdleTime > 0)
                {
                    IdleTime = 0;
                }

                if (RoomData.HasActivePromotion && RoomData.Promotion.HasExpired)
                {
                    RoomData.EndPromotion();
                }

                if (IdleTime >= 60 && !RoomData.HasActivePromotion)
                {
                    StarBlueServer.GetGame().GetRoomManager().UnloadRoom(Id);
                    return;
                }

                try { GetRoomItemHandler().OnCycle(); }
                catch (Exception)
                {
                    //Logging.LogException("Room ID [" + RoomId + "] está tendo problemas com ITEMS" + e.ToString());
                }

                try { GetRoomUserManager().OnCycle(); }
                catch (Exception)
                {
                    //Logging.LogException("Room ID [" + RoomId + "] está tendo problemas com ITEMS" + e.ToString());
                }

                try { _traxManager.OnCycle(); }
                catch (Exception)
                {
                    //ExceptionLogger.LogException(e);
                }

                #region Status Updates
                try
                {
                    GetRoomUserManager().SerializeStatusUpdates();
                }
                catch (Exception)
                {
                    //Logging.LogException("Room ID [" + RoomId + "] está tendo problemas com alguns PACOTES ou está usando TANJI." + e.ToString());
                }
                #endregion

                #region Game Item Cycle
                try
                {
                    if (_gameItemHandler != null)
                    {
                        _gameItemHandler.OnCycle();
                    }
                }
                catch (Exception)
                {
                    //Logging.LogException("Room ID [" + RoomId + "] está tendo problemas com ITEMS." + e.ToString());
                }
                #endregion

                try { GetWired().OnCycle(); }
                catch (Exception)
                {
                    //Logging.LogException("Room ID [" + RoomId + "] está tendo problemas com WIRED." + e.ToString());
                }

            }
            catch (Exception e)
            {
                Logging.WriteLine("Room ID [" + RoomId + "] travou!");
                Logging.LogException("Room ID [" + RoomId + "] travou!" + e.ToString());
                OnRoomCrash(e);
            }
        }

        private void OnRoomCrash(Exception e)
        {
            //Logging.LogThreadException(e.ToString(), "Erro com o quarto" + RoomId);

            try
            {
                foreach (RoomUser user in _roomUserManager.GetRoomUsers().ToList())
                {
                    if (user == null || user.GetClient() == null)
                    {
                        continue;
                    }

                    user.GetClient().SendNotification("O quarto travou, avise um Administrador.");

                    try
                    {
                        GetRoomUserManager().RemoveUserFromRoom(user.GetClient(), true, false);
                    }
                    catch (Exception e2) { Logging.LogException(e2.ToString()); }
                }
            }
            catch (Exception e3) { Logging.LogException(e3.ToString()); }

            isCrashed = true;
            StarBlueServer.GetGame().GetRoomManager().UnloadRoom(Id, true);
        }


        public bool CheckMute(GameClient Session)
        {
            if (MutedUsers.ContainsKey(Session.GetHabbo().Id))
            {
                if (MutedUsers[Session.GetHabbo().Id] < StarBlueServer.GetUnixTimestamp())
                {
                    MutedUsers.Remove(Session.GetHabbo().Id);
                }
                else
                {
                    return true;
                }
            }

            if (Session.GetHabbo().TimeMuted > 0 || (RoomMuted && Session.GetHabbo().Username != OwnerName))
            {
                return true;
            }

            return false;
        }

        public void AddChatlog(int Id, string Message)
        {
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `chatlogs` (user_id, room_id, message, timestamp) VALUES (@user, @room, @message, @time)");
                dbClient.AddParameter("user", Id);
                dbClient.AddParameter("room", RoomId);
                dbClient.AddParameter("message", Encoding.UTF8.GetString(Encoding.Default.GetBytes(Message)));
                dbClient.AddParameter("time", StarBlueServer.GetUnixTimestamp());
                dbClient.RunQuery();
            }
        }

        public void SendObjects(GameClient Session)
        {
            Room Room = Session.GetHabbo().CurrentRoom;

            Session.SendMessage(new HeightMapComposer(Room.GetGameMap().Model.Heightmap));
            Session.SendMessage(new FloorHeightMapComposer(Room.GetGameMap().Model.GetRelativeHeightmap(), Room.GetGameMap().StaticModel.WallHeight));

            foreach (RoomUser RoomUser in _roomUserManager.GetUserList().ToList())
            {
                if (RoomUser == null)
                {
                    continue;
                }

                Session.SendMessage(new UsersComposer(RoomUser));

                if (RoomUser.IsBot && RoomUser.BotData.DanceId > 0)
                {
                    Session.SendMessage(new DanceComposer(RoomUser, RoomUser.BotData.DanceId));
                }
                else if (!RoomUser.IsBot && !RoomUser.IsPet && RoomUser.IsDancing)
                {
                    Session.SendMessage(new DanceComposer(RoomUser, RoomUser.DanceId));
                }

                if (RoomUser.IsAsleep)
                {
                    Session.SendMessage(new SleepComposer(RoomUser, true));
                }

                if (RoomUser.CarryItemID > 0 && RoomUser.CarryTimer > 0)
                {
                    Session.SendMessage(new CarryObjectComposer(RoomUser.VirtualId, RoomUser.CarryItemID));
                }

              //  if (!RoomUser.IsBot && !RoomUser.IsPet && RoomUser.CurrentEffect > 0)
               // {
               //     Room.SendMessage(new AvatarEffectComposer(RoomUser.VirtualId, RoomUser.CurrentEffect));
               // }
            }

            Session.SendMessage(new UserUpdateComposer(_roomUserManager.GetUserList().ToList()));
            Session.SendMessage(new ObjectsComposer(Room.GetRoomItemHandler().GetFloor.ToArray(), this));
            Session.SendMessage(new ItemsComposer(Room.GetRoomItemHandler().GetWall.ToArray(), this));
            Session.SendMessage(new SetCameraPicturePriceMessageComposer(100, 10, 10));
        }

        #region Tents
        public void AddTent(int TentId)
        {
            if (Tents.ContainsKey(TentId))
            {
                Tents.Remove(TentId);
            }

            Tents.Add(TentId, new List<RoomUser>());
        }

        public void RemoveTent(int TentId, Item Item)
        {
            if (!Tents.ContainsKey(TentId))
            {
                return;
            }

            List<RoomUser> Users = Tents[TentId];
            foreach (RoomUser User in Users.ToList())
            {
                if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                {
                    continue;
                }

                User.GetClient().GetHabbo().TentId = 0;
            }

            if (Tents.ContainsKey(TentId))
            {
                Tents.Remove(TentId);
            }
        }

        public void AddUserToTent(int TentId, RoomUser User, Item Item)
        {
            if (User != null && User.GetClient() != null && User.GetClient().GetHabbo() != null)
            {
                if (!Tents.ContainsKey(TentId))
                {
                    Tents.Add(TentId, new List<RoomUser>());
                }

                if (!Tents[TentId].Contains(User))
                {
                    Tents[TentId].Add(User);
                }

                User.GetClient().GetHabbo().TentId = TentId;
            }
        }

        public void RemoveUserFromTent(int TentId, RoomUser User, Item Item)
        {
            if (User != null && User.GetClient() != null && User.GetClient().GetHabbo() != null)
            {
                if (!Tents.ContainsKey(TentId))
                {
                    Tents.Add(TentId, new List<RoomUser>());
                }

                if (Tents[TentId].Contains(User))
                {
                    Tents[TentId].Remove(User);
                }

                User.GetClient().GetHabbo().TentId = 0;
            }
        }

        public void SendToTent(int Id, int TentId, IServerPacket Packet)
        {
            if (!Tents.ContainsKey(TentId))
            {
                return;
            }

            foreach (RoomUser User in Tents[TentId].ToList())
            {
                if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null || User.GetClient().GetHabbo().MutedUsers.Contains(Id) || User.GetClient().GetHabbo().TentId != TentId)
                {
                    continue;
                }

                User.GetClient().SendMessage(Packet);
            }
        }
        #endregion

        #region Communication (Packets)
        public void SendMessage(IServerPacket Message, bool UsersWithRightsOnly = false)
        {
            if (Message == null)
            {
                return;
            }

            try
            {

                List<RoomUser> Users = _roomUserManager.GetUserList().ToList();

                if (this == null || _roomUserManager == null || Users == null)
                {
                    return;
                }

                foreach (RoomUser User in Users)
                {
                    if (User == null || User.IsBot)
                    {
                        continue;
                    }

                    if (User.GetClient() == null || User.GetClient().GetConnection() == null)
                    {
                        continue;
                    }

                    if (UsersWithRightsOnly && !CheckRights(User.GetClient()))
                    {
                        continue;
                    }

                    User.GetClient().SendMessage(Message);
                }
            }
            catch (Exception e)
            {
                Logging.HandleException(e, "Room.SendMessage");
            }
        }

        public void BroadcastPacket(byte[] Packet)
        {
            foreach (RoomUser User in _roomUserManager.GetUserList().ToList())
            {
                if (User == null || User.IsBot)
                {
                    continue;
                }

                if (User.GetClient() == null || User.GetClient().GetConnection() == null)
                {
                    continue;
                }

                User.GetClient().GetConnection().SendData(Packet);
            }
        }

        public void SendMessage(List<ServerPacket> Messages)
        {
            if (Messages.Count == 0)
            {
                return;
            }

            try
            {
                byte[] TotalBytes = new byte[0];
                int Current = 0;

                foreach (ServerPacket Packet in Messages.ToList())
                {
                    byte[] ToAdd = Packet.GetBytes();
                    int NewLen = TotalBytes.Length + ToAdd.Length;

                    Array.Resize(ref TotalBytes, NewLen);

                    for (int i = 0; i < ToAdd.Length; i++)
                    {
                        TotalBytes[Current] = ToAdd[i];
                        Current++;
                    }
                }

                BroadcastPacket(TotalBytes);
            }
            catch (Exception e)
            {
                Logging.HandleException(e, "Room.SendMessage List<ServerPacket>");
            }
        }
        #endregion

        private void SaveAI()
        {
            foreach (RoomUser User in GetRoomUserManager().GetRoomUsers().ToList())
            {
                if (User == null || !User.IsBot)
                {
                    continue;
                }

                if (User.IsBot)
                {
                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE bots SET x=@x, y=@y, z=@z, name=@name, look=@look, rotation=@rotation WHERE id=@id LIMIT 1;");
                        dbClient.AddParameter("name", User.BotData.Name);
                        dbClient.AddParameter("look", User.BotData.Look);
                        dbClient.AddParameter("rotation", User.BotData.Rot);
                        dbClient.AddParameter("x", User.X);
                        dbClient.AddParameter("y", User.Y);
                        dbClient.AddParameter("z", User.Z);
                        dbClient.AddParameter("id", User.BotData.BotId);
                        dbClient.RunQuery();
                    }
                }
            }
        }

        public void Dispose()
        {
            SendMessage(new CloseConnectionComposer());

            if (!mDisposed)
            {
                isCrashed = false;
                mDisposed = true;

                try
                {
                    if (ProcessTask != null && ProcessTask.IsCompleted)
                    {
                        ProcessTask.Dispose();
                    }
                }
                catch { }

                GetRoomItemHandler().SaveFurniture();

                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("UPDATE `rooms` SET `users_now` = '0' WHERE `id` = '" + Id + "' LIMIT 1");
                }

                if (_roomUserManager.PetCount > 0)
                {
                    _roomUserManager.UpdatePets();
                }

                SaveAI();

                UsersNow = 0;
                RoomData.UsersNow = 0;

                UsersWithRights.Clear();
                Bans.Clear();
                MutedUsers.Clear();
                Tents.Clear();

                if (ActiveTrades.Count > 0)
                {
                    ActiveTrades.Clear();
                }

                TonerData = null;
                MoodlightData = null;

                _filterComponent.Cleanup();
                _wiredComponent.Cleanup();

                if (_gameItemHandler != null)
                {
                    _gameItemHandler.Dispose();
                }

                if (_gameManager != null)
                {
                    _gameManager.Dispose();
                }

                if (_freeze != null)
                {
                    _freeze.Dispose();
                }

                if (_banzai != null)
                {
                    _banzai.Dispose();
                }

                if (_soccer != null)
                {
                    _soccer.Dispose();
                }

                if (_gamemap != null)
                {
                    _gamemap.Dispose();
                }

                if (_roomUserManager != null)
                {
                    _roomUserManager.Dispose();
                }

                if (_roomItemHandling != null)
                {
                    _roomItemHandling.Dispose();
                }

                if (ActiveTrades.Count > 0)
                {
                    ActiveTrades.Clear();
                }
            }
        }
    }
}
