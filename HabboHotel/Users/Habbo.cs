using StarBlue.Communication.Packets.Outgoing.Handshake;
using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Communication.Packets.Outgoing.Navigator;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using StarBlue.Communication.WebSocket;
using StarBlue.Core;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Achievements;
using StarBlue.HabboHotel.Catalog;
using StarBlue.HabboHotel.Club;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Groups;
using StarBlue.HabboHotel.Helpers;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.Chat.Commands;
using StarBlue.HabboHotel.Users.Badges;
using StarBlue.HabboHotel.Users.Clothing;
using StarBlue.HabboHotel.Users.Effects;
using StarBlue.HabboHotel.Users.Ignores;
using StarBlue.HabboHotel.Users.Inventory;
using StarBlue.HabboHotel.Users.Messenger;
using StarBlue.HabboHotel.Users.Messenger.FriendBar;
using StarBlue.HabboHotel.Users.Navigator.SavedSearches;
using StarBlue.HabboHotel.Users.Permissions;
using StarBlue.HabboHotel.Users.Polls;
using StarBlue.HabboHotel.Users.Process;
using StarBlue.HabboHotel.Users.Relationships;
using StarBlue.HabboHotel.Users.UserDataManagement;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace StarBlue.HabboHotel.Users
{
    public class Habbo
    {
        public string _tag;
        public string _tagcolor;
        public string _nameColor;

        //Generic player values.
        private int _id;
        private string _username;
        private int _rank;
        private string _motto;
        private string _look;
        private string _gender;
        public int _bet;
        private string _footballLook;
        private string _footballGender;
        private string _backupLook;
        private bool _lastMovFGate;
        private string _backupGender;
        private int _credits;
        private int _duckets;
        private int _diamonds;
        private string _pinClient;
        private int _gotwPoints;
        private int _userpoints;
        private int _homeRoom;
        private double _lastOnline;
        private double _accountCreated;
        private List<int> _clientVolume;
        private double _lastNameChange;
        private string _machineId;
        private bool _chatPreference;
        private bool _focusPreference;
        private bool _isExpert;
        private int _vipRank;
        private int _CurrentTalentLevel;
        private int _BonusPoints;
        public int IceSkatingCounter = 0;
        public bool _playingChess = false;
        public string _eColor = "";
        public bool FindFurniMode = false;

        // Leaderboards
        public int _leaderPoints;
        public int _leaderRecord;

        // Bools Custom Settings
        public bool _isControlling = false;

        //Abilitys triggered by generic events.
        public string _alerttype = "2";
        public string _eventtype = "2";
        public int _eventsopened;
        private bool _appearOffline;
        private bool _allowTradingRequests;
        private bool _allowUserFollowing;
        private bool _allowFriendRequests;
        private bool _allowMessengerInvites;
        private bool _allowPetSpeech;
        private bool _allowBotSpeech;
        private bool _allowPublicRoomStatus;
        private bool _allowConsoleMessages;
        private bool _allowGifts;
        private bool _allowMimic;
        private bool _receiveWhispers;
        private bool _ignorePublicWhispers;
        private bool _playingFastFood;
        private FriendBarState _friendbarState;
        private int _christmasDay;
        private int _wantsToRideHorse;
        private int _timeAFK;
        private int _lastUserID;
        private bool _disableForcedEffects = false;

        public long _lastTimeUsedHelpCommand;

        //Player saving.
        private bool _disconnected;
        private bool _habboSaved;
        private bool _changingName;
        public byte _changename;
        public bool _disabledEvents;
        public bool _disabledMentions;

        public Dictionary<string, int> WiredRewards;

        //Counters
        private double _floodTime;
        private int _friendCount;
        private double _timeMuted;
        private double _tradingLockExpiry;

        private int _bannedPhraseCount;
        private double _sessionStart;
        private int _messengerSpamCount;
        private double _messengerSpamTime;
        private int _creditsTickUpdate;
        private int _bonusTickUpdate;
        private int _diamantesTickUpdate;
        private int _hofTickUpdate;
        public byte _guidelevel;
        public byte _builder;
        public byte _croupier;
        public bool _isFirstThrow;
        public bool _hisTurn = false;
        public byte _TargetedBuy;

        public bool casinoEnabled;
        public int casinoCount;

        //Room related
        private int _tentId;
        private int _hopperId;
        private bool _isHopping;
        private int _teleportId;
        private bool _isTeleporting;
        private int _teleportingRoomId;
        private bool _roomAuthOk;
        private int _currentRoomId;
        public bool Spectating = false;
        public string _Opponent;

        //Advertising reporting system.
        private bool _hasSpoken;
        private bool _advertisingReported;
        private double _lastAdvertiseReport;
        private bool _advertisingReportBlocked;

        //Values generated within the game.
        private bool _wiredInteraction;
        private int _questLastCompleted;

        private int _lastMessageCount;
        private string _lastMessage;

        private bool _inventoryAlert;
        private bool _ignoreBobbaFilter;
        private bool _wiredTeleporting;
        private int _customBubbleId;
        private int _tempInt;
        private bool _onHelperDuty;
        public string chatHTMLColour;

        public bool isPasting = false;
        public bool isDeveloping = false;
        public int lastX;
        public int lastY;
        public int lastX2;
        public int lastY2;

        public int multiWhisperID;

        public UserData userData;

        //alfas
        internal bool onDuty;
        internal bool onService;
        internal uint userHelping;
        internal bool requestHelp;
        internal bool requestTour;
        internal bool reportsOfHarassment;
        public bool _SecureTradeEnabled = false;
        public bool _SecurityQuestion = false;
        public bool _IsBeingAsked = false;

        // Camera

        public string lastPhotoPreview;

        //Fastfood
        private int _fastfoodScore;


        //Just random fun stuff.
        private int _petId;
        private string _sexWith;

        //Anti-script placeholders.
        private DateTime _lastGiftPurchaseTime;
        private DateTime _lastMottoUpdateTime;
        private DateTime _lastClothingUpdateTime;

        public int LastSqlQuery = 0;
        private DateTime _lastForumMessageUpdateTime;

        private int _giftPurchasingWarnings;
        private int _mottoUpdateWarnings;
        private int _clothingUpdateWarnings;

        public bool _sellingroom = false;

        public bool StaffOk = false;
        public int LastCraftingMachine = 0;
        public int LastEffect = 0;
        public int EventType = 1;
        public bool _NUX;
        public bool _NuxRoom;
        public bool PassedNuxNavigator = false, PassedNuxDuckets = false, PassedNuxItems = false, PassedNuxChat = false, PassedNuxCatalog = false, PassedNuxMMenu = false, PassedNuxCredits = false;
        private bool _sessionGiftBlocked;
        private bool _sessionMottoBlocked;
        private bool _sessionClothingBlocked;

        private bool _rigDice;
        private int _diceNumber;

        public List<int> RatedRooms;
        public List<RoomUser> MultiWhispers;
        public List<RoomData> UsersRooms;
        public List<Item> TradeItems;
        public bool _isBettingDice = false;
        public int[] PurchasingItem;

        private GameClient _client;
        private HabboStats _habboStats;
        private HabboMessenger Messenger;
        private ClubManager ClubManager;
        private IgnoresComponent _ignores;
        private ProcessComponent _process;
        public ArrayList FavoriteRooms;
        public ArrayList Tags;
        public ArrayList MysticKeys;
        public ArrayList MysticBoxes;
        public Dictionary<int, int> quests;
        public Dictionary<int, CatalogItem> LastPurchasesItems;
        private BadgeComponent BadgeComponent;
        private InventoryComponent InventoryComponent;
        public Dictionary<int, Relationship> Relationships;
        public ConcurrentDictionary<string, UserAchievement> Achievements;
        private PollsComponent _polls;

        private DateTime _timeCached;

        private SearchesComponent _navigatorSearches;
        private EffectsComponent _fx;
        private ClothingComponent _clothing;
        private PermissionComponent _permissions;

        private IChatCommand _iChatCommand;
        public int chatHTMLSize;
        private Dictionary<int, UserTalent> _Talents;
        public bool PassedQuiz;

        public bool _multiWhisper;
        public bool IsCitizen => CurrentTalentLevel > 4;
        internal List<int> _HabboQuizQuestions;

        internal string chatColour;
        public bool[] calendarGift;
        public double StackHeight = 0;
        public int FurniRotation = -1;
        public int FurniState = -1;
        public bool BuilderTool;
        public long emojiTime;

        public Habbo(int Id, string Username, int Rank, string Motto, string Look, string Gender, int Credits, int ActivityPoints, int HomeRoom,
         bool HasFriendRequestsDisabled, int LastOnline, bool AppearOffline, bool HideInRoom, double CreateDate, int Diamonds,
         string machineID, string clientVolume, bool ChatPreference, bool FocusPreference, bool PetsMuted, bool BotsMuted, bool AdvertisingReportBlocked, double LastNameChange,
         int GOTWPoints, int UserPoints, bool IgnoreInvites, double TimeMuted, double TradingLock, bool AllowGifts, int FriendBarState, bool DisableForcedEffects, bool AllowMimic, int VIPRank,
         byte guidelevel, byte builder, byte croupier, bool Nux, bool NuxRoom, byte TargetedBuy, string NameColor, string Chatcolour, string Tag, string TagColor, int BubbleId, byte NameChange, string PinClient, bool DisabledEvents, bool DisabledMentions)
        {
            _id = Id;
            _username = Username;
            _rank = Rank;
            _motto = Motto;
            _look = Look;
            _gender = Gender.ToLower();
            _footballLook = StarBlueServer.FilterFigure(Look.ToLower());
            _footballGender = Gender.ToLower();
            _credits = Credits;
            _duckets = ActivityPoints;
            _diamonds = Diamonds;
            _gotwPoints = GOTWPoints;
            _pinClient = PinClient;
            _disabledEvents = DisabledEvents;
            _disabledMentions = DisabledMentions;
            _NUX = Nux;
            _NuxRoom = NuxRoom;
            _userpoints = UserPoints;
            _homeRoom = HomeRoom;
            _lastOnline = LastOnline;
            _guidelevel = guidelevel;
            _builder = builder;
            _croupier = croupier;
            _TargetedBuy = TargetedBuy;
            _accountCreated = CreateDate;
            _clientVolume = new List<int>();
            _Talents = new Dictionary<int, UserTalent>();
            _nameColor = NameColor;
            _tag = Tag;
            _tagcolor = TagColor;
            _changename = NameChange;

            foreach (string Str in clientVolume.Split(','))
            {
                if (int.TryParse(Str, out int Val))
                {
                    _clientVolume.Add(int.Parse(Str));
                }
                else
                {
                    _clientVolume.Add(100);
                }
            }

            _lastNameChange = LastNameChange;
            _machineId = machineID;
            _chatPreference = ChatPreference;
            _focusPreference = FocusPreference;
            _isExpert = IsExpert == true;

            _appearOffline = AppearOffline;
            _allowTradingRequests = true;//TODO
            _allowUserFollowing = true;//TODO
            _allowFriendRequests = HasFriendRequestsDisabled;//TODO
            _allowMessengerInvites = IgnoreInvites;
            _allowPetSpeech = PetsMuted;
            _allowBotSpeech = BotsMuted;
            _allowPublicRoomStatus = HideInRoom;
            _allowConsoleMessages = true;
            _allowGifts = AllowGifts;
            _allowMimic = AllowMimic;
            _receiveWhispers = true;
            _ignorePublicWhispers = false;
            _playingFastFood = false;
            _friendbarState = FriendBarStateUtility.GetEnum(FriendBarState);
            _christmasDay = ChristmasDay;
            _wantsToRideHorse = 0;
            _timeAFK = 0;
            _disableForcedEffects = DisableForcedEffects;
            _vipRank = VIPRank;
            _bet = 0;

            onDuty = false;
            requestHelp = false;
            requestTour = false;
            userHelping = 0;
            reportsOfHarassment = false;
            onService = false;

            _disconnected = false;
            _habboSaved = false;
            _changingName = false;

            _floodTime = 0;
            _friendCount = 0;
            _timeMuted = TimeMuted;
            _timeCached = DateTime.Now;

            _sellingroom = false;

            //this._CurrentTalentLevel = GetCurrentTalentLevel();

            _tradingLockExpiry = TradingLock;
            if (_tradingLockExpiry > 0 && StarBlueServer.GetUnixTimestamp() > TradingLockExpiry)
            {
                _tradingLockExpiry = 0;
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("UPDATE `user_info` SET `trading_locked` = '0' WHERE `user_id` = '" + Id + "' LIMIT 1");
                }
            }

            _bannedPhraseCount = 0;
            _sessionStart = StarBlueServer.GetUnixTimestamp();
            _messengerSpamCount = 0;
            _messengerSpamTime = 0;
            _creditsTickUpdate = Convert.ToInt32(StarBlueServer.GetConfig().data["amount.time.minutes"]);

            casinoCount = 0;
            casinoEnabled = false;
            chatHTMLSize = 12;

            _tentId = 0;
            _hopperId = 0;
            _isHopping = false;
            _teleportId = 0;
            _isTeleporting = false;
            _teleportingRoomId = 0;
            _roomAuthOk = false;
            _currentRoomId = 0;

            _hasSpoken = false;
            _lastAdvertiseReport = 0;
            _advertisingReported = false;
            _advertisingReportBlocked = AdvertisingReportBlocked;

            _multiWhisper = false;
            _wiredInteraction = false;
            _questLastCompleted = 0;
            _inventoryAlert = false;
            _ignoreBobbaFilter = false;
            _wiredTeleporting = false;
            _customBubbleId = 0;
            _onHelperDuty = false;
            _fastfoodScore = 0;
            _petId = 0;
            _tempInt = 0;

            _lastGiftPurchaseTime = DateTime.Now;
            _lastMottoUpdateTime = DateTime.Now;
            _lastClothingUpdateTime = DateTime.Now;
            _lastForumMessageUpdateTime = DateTime.Now;

            _giftPurchasingWarnings = 0;
            _mottoUpdateWarnings = 0;
            _clothingUpdateWarnings = 0;

            _sessionGiftBlocked = false;
            _sessionMottoBlocked = false;
            _sessionClothingBlocked = false;
            _isFirstThrow = true;

            FavoriteRooms = new ArrayList();
            MultiWhispers = new List<RoomUser>();
            Achievements = new ConcurrentDictionary<string, UserAchievement>();
            Relationships = new Dictionary<int, Relationship>();
            RatedRooms = new List<int>();
            UsersRooms = new List<RoomData>();
            TradeItems = new List<Item>();
            LastPurchasesItems = new Dictionary<int, CatalogItem>();

            //TODO: Nope.
            InitPermissions();

            #region Stats
            DataRow StatRow = null;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`roomvisits`,`onlinetime`,`respect`,`respectgiven`,`giftsgiven`,`giftsreceived`,`dailyrespectpoints`,`dailypetrespectpoints`,`achievementscore`,`quest_id`,`quest_progress`,`groupid`,`tickets_answered`,`respectstimestamp`,`forum_posts`, `PurchaseUsersConcurrent`, `vip_gifts` FROM `user_stats` WHERE `id` = @user_id LIMIT 1");
                dbClient.AddParameter("user_id", Id);
                StatRow = dbClient.GetRow();

                if (StatRow == null)//No row, add it yo
                {
                    dbClient.RunFastQuery("INSERT INTO `user_stats` (`id`) VALUES ('" + Id + "')");
                    dbClient.SetQuery("SELECT `id`,`roomvisits`,`onlinetime`,`respect`,`respectgiven`,`giftsgiven`,`giftsreceived`,`dailyrespectpoints`,`dailypetrespectpoints`,`achievementscore`,`quest_id`,`quest_progress`,`groupid`,`tickets_answered`,`respectstimestamp`,`forum_posts`, `PurchaseUsersConcurrent`, `vip_gifts` FROM `user_stats` WHERE `id` = @user_id LIMIT 1");
                    dbClient.AddParameter("user_id", Id);
                    StatRow = dbClient.GetRow();
                }

                try
                {
                    _habboStats = new HabboStats(Convert.ToInt32(StatRow["roomvisits"]), Convert.ToDouble(StatRow["onlineTime"]), Convert.ToInt32(StatRow["respect"]), Convert.ToInt32(StatRow["respectGiven"]), Convert.ToInt32(StatRow["giftsGiven"]),
                        Convert.ToInt32(StatRow["giftsReceived"]), Convert.ToInt32(StatRow["dailyRespectPoints"]), Convert.ToInt32(StatRow["dailyPetRespectPoints"]), Convert.ToInt32(StatRow["AchievementScore"]),
                        Convert.ToInt32(StatRow["quest_id"]), Convert.ToInt32(StatRow["quest_progress"]), Convert.ToInt32(StatRow["groupid"]), Convert.ToString(StatRow["respectsTimestamp"]), Convert.ToInt32(StatRow["forum_posts"]), Convert.ToBoolean(StatRow["PurchaseUsersConcurrent"]), Convert.ToInt32(StatRow["vip_gifts"]));

                    if (Convert.ToString(StatRow["respectsTimestamp"]) != DateTime.Today.ToString("MM/dd"))
                    {
                        _habboStats.RespectsTimestamp = DateTime.Today.ToString("MM/dd");

                        int DailyRespects = 5;
                        _habboStats.DailyRespectPoints = DailyRespects;
                        _habboStats.DailyPetRespectPoints = DailyRespects;

                        dbClient.RunFastQuery("UPDATE `user_stats` SET `dailyRespectPoints` = '" + DailyRespects + "', `dailyPetRespectPoints` = '" + DailyRespects + "', `respectsTimestamp` = '" + DateTime.Today.ToString("MM/dd") + "' WHERE `id` = '" + Id + "' LIMIT 1");
                    }
                }
                catch (Exception e)
                {
                    Logging.LogException(e.ToString());
                }
            }

            if (!StarBlueServer.GetGame().GetGroupManager().TryGetGroup(_habboStats.FavouriteGroupId, out Group G))
            {
                _habboStats.FavouriteGroupId = 0;
            }
            #endregion
        }

        internal ClubManager GetClubManager()
        {
            return ClubManager;
        }

        public IgnoresComponent GetIgnores()
        {
            return _ignores;
        }

        public string PrefixName
        {
            get => _tag;
            set => _tag = value;
        }

        public string eColor
        {
            get => _eColor;
            set => _eColor = value;
        }

        public string PrefixColor
        {
            get => _tagcolor;
            set => _tagcolor = value;
        }

        public string NameColor
        {
            get => _nameColor;
            set => _nameColor = value;
        }

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public int lastUserId
        {
            get => _lastUserID;
            set => _lastUserID = value;
        }

        public string Username
        {
            get => _username;
            set => _username = value;
        }

        public int Rank
        {
            get => _rank;
            set => _rank = value;
        }

        public string Motto
        {
            get => _motto;
            set => _motto = value;
        }

        public string Look
        {
            get => _look;
            set => _look = value;
        }

        public string Gender
        {
            get => _gender;
            set => _gender = value;
        }

        public string FootballLook
        {
            get => _footballLook;
            set => _footballLook = value;
        }

        private bool InitPolls()
        {
            _polls = new PollsComponent();

            return _polls.Init(this);
        }

        public PollsComponent GetPolls()
        {
            return _polls;
        }

        public string FootballGender
        {
            get => _footballGender;
            set => _footballGender = value;
        }

        public bool LastMovFGate
        {
            get => _lastMovFGate;
            set => _lastMovFGate = value;
        }

        // Dice System

        public bool FirstThrow
        {
            get => _isFirstThrow;
            set => _isFirstThrow = value;
        }

        public bool isControlling
        {
            get => _isControlling;
            set => _isControlling = value;
        }

        public bool HisTurn
        {
            get => _hisTurn;
            set => _hisTurn = value;
        }

        public string Opponent
        {
            get => _Opponent;
            set => _Opponent = value;
        }

        public bool MultiWhisper
        {
            get => _multiWhisper;
            set => _multiWhisper = value;
        }

        public string BackupLook
        {
            get => _backupLook;
            set => _backupLook = value;
        }

        public string BackupGender
        {
            get => _backupGender;
            set => _backupGender = value;
        }

        public int Credits
        {
            get => _credits;
            set => _credits = value;
        }

        public int Duckets
        {
            get => _duckets;
            set => _duckets = value;
        }

        public int Diamonds
        {
            get => _diamonds;
            set => _diamonds = value;
        }

        public bool RigDice
        {
            get => _rigDice;
            set => _rigDice = value;
        }

        public int DiceNumber
        {
            get => _diceNumber;
            set => _diceNumber = value;
        }

        public string PinClient
        {
            get => _pinClient;
            set => _pinClient = value;
        }

        public bool DisabledNotificationEvents
        {
            get => _disabledEvents;
            set => _disabledEvents = value;
        }

        public bool DisabledMentions
        {
            get => _disabledMentions;
            set => _disabledMentions = value;
        }

        public int GOTWPoints
        {
            get => _gotwPoints;
            set => _gotwPoints = value;
        }

        public int BonusPoints
        {
            get => _BonusPoints;
            set => _BonusPoints = value;
        }

        public int UserPoints
        {
            get => _userpoints;
            set => _userpoints = value;
        }

        public int HomeRoom
        {
            get => _homeRoom;
            set => _homeRoom = value;
        }

        public double LastOnline
        {
            get => _lastOnline;
            set => _lastOnline = value;
        }

        public double AccountCreated
        {
            get => _accountCreated;
            set => _accountCreated = value;
        }

        public List<int> ClientVolume
        {
            get => _clientVolume;
            set => _clientVolume = value;
        }

        public double LastNameChange
        {
            get => _lastNameChange;
            set => _lastNameChange = value;
        }

        public string MachineId
        {
            get => _machineId;
            set => _machineId = value;
        }

        public bool ChatPreference
        {
            get => _chatPreference;
            set => _chatPreference = value;
        }
        public bool FocusPreference
        {
            get => _focusPreference;
            set => _focusPreference = value;
        }

        public bool IsExpert
        {
            get => _isExpert;
            set => _isExpert = value;
        }

        public bool AppearOffline
        {
            get => _appearOffline;
            set => _appearOffline = value;
        }

        public int VIPRank
        {
            get => _vipRank;
            set => _vipRank = value;
        }

        public int TempInt
        {
            get => _tempInt;
            set => _tempInt = value;
        }

        public bool AllowTradingRequests
        {
            get => _allowTradingRequests;
            set => _allowTradingRequests = value;
        }

        public bool AllowUserFollowing
        {
            get => _allowUserFollowing;
            set => _allowUserFollowing = value;
        }

        public bool AllowFriendRequests
        {
            get => _allowFriendRequests;
            set => _allowFriendRequests = value;
        }

        public bool AllowMessengerInvites
        {
            get => _allowMessengerInvites;
            set => _allowMessengerInvites = value;
        }

        public bool AllowPetSpeech
        {
            get => _allowPetSpeech;
            set => _allowPetSpeech = value;
        }

        public bool AllowBotSpeech
        {
            get => _allowBotSpeech;
            set => _allowBotSpeech = value;
        }

        public bool AllowPublicRoomStatus
        {
            get => _allowPublicRoomStatus;
            set => _allowPublicRoomStatus = value;
        }

        public bool AllowConsoleMessages
        {
            get => _allowConsoleMessages;
            set => _allowConsoleMessages = value;
        }

        public bool AllowGifts
        {
            get => _allowGifts;
            set => _allowGifts = value;
        }

        // CHESS SYSTEM
        public bool PlayingChess
        {
            get => _playingChess;
            set => _playingChess = value;
        }

        public bool AllowMimic
        {
            get => _allowMimic;
            set => _allowMimic = value;
        }

        public bool ReceiveWhispers
        {
            get => _receiveWhispers;
            set => _receiveWhispers = value;
        }

        public bool IgnorePublicWhispers
        {
            get => _ignorePublicWhispers;
            set => _ignorePublicWhispers = value;
        }

        public bool PlayingFastFood
        {
            get => _playingFastFood;
            set => _playingFastFood = value;
        }

        public FriendBarState FriendbarState
        {
            get => _friendbarState;
            set => _friendbarState = value;
        }

        public int ChristmasDay
        {
            get => _christmasDay;
            set => _christmasDay = value;
        }

        public int WantsToRideHorse
        {
            get => _wantsToRideHorse;
            set => _wantsToRideHorse = value;
        }

        public int TimeAFK
        {
            get => _timeAFK;
            set => _timeAFK = value;
        }

        public string LastMessage
        {
            get => _lastMessage;
            set => _lastMessage = value;
        }

        public int LastMessageCount
        {
            get => _lastMessageCount;
            set => _lastMessageCount = value;
        }

        public bool DisableForcedEffects
        {
            get => _disableForcedEffects;
            set => _disableForcedEffects = value;
        }

        public bool ChangingName
        {
            get => _changingName;
            set => _changingName = value;
        }

        public int FriendCount
        {
            get => _friendCount;
            set => _friendCount = value;
        }

        public double FloodTime
        {
            get => _floodTime;
            set => _floodTime = value;
        }

        public int BannedPhraseCount
        {
            get => _bannedPhraseCount;
            set => _bannedPhraseCount = value;
        }

        public bool RoomAuthOk
        {
            get => _roomAuthOk;
            set => _roomAuthOk = value;
        }

        public int CurrentRoomId
        {
            get => _currentRoomId;
            set => _currentRoomId = value;
        }

        public int QuestLastCompleted
        {
            get => _questLastCompleted;
            set => _questLastCompleted = value;
        }

        public int MessengerSpamCount
        {
            get => _messengerSpamCount;
            set => _messengerSpamCount = value;
        }

        public double MessengerSpamTime
        {
            get => _messengerSpamTime;
            set => _messengerSpamTime = value;
        }

        public double TimeMuted
        {
            get => _timeMuted;
            set => _timeMuted = value;
        }

        public double TradingLockExpiry
        {
            get => _tradingLockExpiry;
            set => _tradingLockExpiry = value;
        }

        public double SessionStart
        {
            get => _sessionStart;
            set => _sessionStart = value;
        }

        public int TentId
        {
            get => _tentId;
            set => _tentId = value;
        }

        public int HopperId
        {
            get => _hopperId;
            set => _hopperId = value;
        }

        public bool IsHopping
        {
            get => _isHopping;
            set => _isHopping = value;
        }

        public int TeleporterId
        {
            get => _teleportId;
            set => _teleportId = value;
        }

        public bool IsTeleporting
        {
            get => _isTeleporting;
            set => _isTeleporting = value;
        }

        public int TeleportingRoomID
        {
            get => _teleportingRoomId;
            set => _teleportingRoomId = value;
        }

        public bool HasSpoken
        {
            get => _hasSpoken;
            set => _hasSpoken = value;
        }

        public double LastAdvertiseReport
        {
            get => _lastAdvertiseReport;
            set => _lastAdvertiseReport = value;
        }

        public bool AdvertisingReported
        {
            get => _advertisingReported;
            set => _advertisingReported = value;
        }

        public bool AdvertisingReportedBlocked
        {
            get => _advertisingReportBlocked;
            set => _advertisingReportBlocked = value;
        }

        public bool WiredInteraction
        {
            get => _wiredInteraction;
            set => _wiredInteraction = value;
        }

        public bool InventoryAlert
        {
            get => _inventoryAlert;
            set => _inventoryAlert = value;
        }

        public bool IgnoreBobbaFilter
        {
            get => _ignoreBobbaFilter;
            set => _ignoreBobbaFilter = value;
        }

        public bool WiredTeleporting
        {
            get => _wiredTeleporting;
            set => _wiredTeleporting = value;
        }

        public int CustomBubbleId
        {
            get => _customBubbleId;
            set => _customBubbleId = value;
        }

        public bool OnHelperDuty
        {
            get => _onHelperDuty;
            set => _onHelperDuty = value;
        }

        public int FastfoodScore
        {
            get => _fastfoodScore;
            set => _fastfoodScore = value;
        }

        public int PetId
        {
            get => _petId;
            set => _petId = value;
        }

        public int CreditsUpdateTick
        {
            get => _creditsTickUpdate;
            set => _creditsTickUpdate = value;
        }

        public int BonusUpdateTick
        {
            get => _bonusTickUpdate;
            set => _bonusTickUpdate = value;
        }

        public int DiamantesUpdateTick
        {
            get => _diamantesTickUpdate;
            set => _diamantesTickUpdate = value;
        }

        public int HofUpdateTick
        {
            get => _hofTickUpdate;
            set => _hofTickUpdate = value;
        }

        public IChatCommand IChatCommand
        {
            get => _iChatCommand;
            set => _iChatCommand = value;
        }

        public DateTime LastGiftPurchaseTime
        {
            get => _lastGiftPurchaseTime;
            set => _lastGiftPurchaseTime = value;
        }

        public DateTime LastMottoUpdateTime
        {
            get => _lastMottoUpdateTime;
            set => _lastMottoUpdateTime = value;
        }

        public DateTime LastClothingUpdateTime
        {
            get => _lastClothingUpdateTime;
            set => _lastClothingUpdateTime = value;
        }

        public DateTime LastForumMessageUpdateTime
        {
            get => _lastForumMessageUpdateTime;
            set => _lastForumMessageUpdateTime = value;
        }

        public int GiftPurchasingWarnings
        {
            get => _giftPurchasingWarnings;
            set => _giftPurchasingWarnings = value;
        }

        public int MottoUpdateWarnings
        {
            get => _mottoUpdateWarnings;
            set => _mottoUpdateWarnings = value;
        }

        public int ClothingUpdateWarnings
        {
            get => _clothingUpdateWarnings;
            set => _clothingUpdateWarnings = value;
        }

        public Dictionary<int, UserTalent> Talents
        {
            get => _Talents;
            set => _Talents = value;
        }

        public int CurrentTalentLevel
        {
            get => _CurrentTalentLevel;
            set => _CurrentTalentLevel = value;
        }

        public bool SessionGiftBlocked
        {
            get => _sessionGiftBlocked;
            set => _sessionGiftBlocked = value;
        }

        public bool SecureTradeEnabled
        {
            get => _SecureTradeEnabled;
            set => _SecureTradeEnabled = value;
        }

        public bool SecurityQuestion
        {
            get => _SecurityQuestion;
            set => _SecurityQuestion = value;
        }

        public bool PlayingDice
        {
            get => _isBettingDice;
            set => _isBettingDice = value;
        }

        public bool IsBeingAsked
        {
            get => _IsBeingAsked;
            set => _IsBeingAsked = value;
        }

        public bool SessionMottoBlocked
        {
            get => _sessionMottoBlocked;
            set => _sessionMottoBlocked = value;
        }

        public bool SessionClothingBlocked
        {
            get => _sessionClothingBlocked;
            set => _sessionClothingBlocked = value;
        }

        public HabboStats GetStats()
        {
            return _habboStats;
        }

        public bool InRoom => CurrentRoomId >= 1 && CurrentRoom != null;

        public Room CurrentRoom
        {
            get
            {
                if (CurrentRoomId <= 0)
                {
                    return null;
                }

                if (StarBlueServer.GetGame().GetRoomManager().TryGetRoom(CurrentRoomId, out Room _room))
                {
                    return _room;
                }

                return null;
            }
        }

        public bool CacheExpired()
        {
            TimeSpan Span = DateTime.Now - _timeCached;
            return (Span.TotalMinutes >= 30);
        }

        public string sexWith
        {
            get => _sexWith;
            set => _sexWith = value;
        }

        public string GetQueryString
        {
            get
            {
                _habboSaved = true;
                return "UPDATE `users` SET `online` = '0', `auth_ticket` = '', `last_online` = '" + StarBlueServer.GetUnixTimestamp() + "', `activity_points` = '" + Duckets + "', `credits` = '" + Credits + "', `vip_points` = '" + Diamonds + "', `disabledevents` = '" + StarBlueServer.BoolToEnum(DisabledNotificationEvents) + "', `disabledmentions` = '" + StarBlueServer.BoolToEnum(DisabledMentions) + "', `bonus_points` = '" + _BonusPoints + "', `home_room` = '" + HomeRoom + "',  `gotw_points` = '" + GOTWPoints + "', `puntos_eventos` = '" + UserPoints + "', `guia` = '" + _guidelevel + "', `builder` = '" + _builder + "', `croupier` = '" + _croupier + "', `time_muted` = '" + TimeMuted + "',`friend_bar_state` = '" + FriendBarStateUtility.GetInt(_friendbarState) + "' WHERE id = '" + Id + "' LIMIT 1;UPDATE `user_stats` SET `roomvisits` = '" + _habboStats.RoomVisits + "', `onlineTime` = '" + (StarBlueServer.GetUnixTimestamp() - SessionStart + _habboStats.OnlineTime) + "', `respect` = '" + _habboStats.Respect + "', `respectGiven` = '" + _habboStats.RespectGiven + "', `giftsGiven` = '" + _habboStats.GiftsGiven + "', `giftsReceived` = '" + _habboStats.GiftsReceived + "', `dailyRespectPoints` = '" + _habboStats.DailyRespectPoints + "', `dailyPetRespectPoints` = '" + _habboStats.DailyPetRespectPoints + "', `AchievementScore` = '" + _habboStats.AchievementPoints + "', `quest_id` = '" + _habboStats.QuestID + "', `quest_progress` = '" + _habboStats.QuestProgress + "', `groupid` = '" + _habboStats.FavouriteGroupId + "',`forum_posts` = '" + _habboStats.ForumPosts + "',`PurchaseUsersConcurrent` = '" + _habboStats.PurchaseUsersConcurrent + "', `vip_gifts` = '" + _habboStats.vipGifts + "' WHERE `id` = '" + Id + "' LIMIT 1;";
            }
        }

        public string GetUpdateString
        {
            get
            {
                return "UPDATE `users` SET `activity_points` = '" + Duckets + "', `credits` = '" + Credits + "', `vip_points` = '" + Diamonds + "', `disabledevents` = '" + StarBlueServer.BoolToEnum(DisabledNotificationEvents) + "', `disabledmentions` = '" + StarBlueServer.BoolToEnum(DisabledMentions) + "', `bonus_points` = '" + _BonusPoints + "', `home_room` = '" + HomeRoom + "',  `gotw_points` = '" + GOTWPoints + "', `puntos_eventos` = '" + UserPoints + "', `guia` = '" + _guidelevel + "', `builder` = '" + _builder + "', `croupier` = '" + _croupier + "', `time_muted` = '" + TimeMuted + "',`friend_bar_state` = '" + FriendBarStateUtility.GetInt(_friendbarState) + "' WHERE id = '" + Id + "' LIMIT 1;UPDATE `user_stats` SET `roomvisits` = '" + _habboStats.RoomVisits + "', `onlineTime` = '" + (StarBlueServer.GetUnixTimestamp() - SessionStart + _habboStats.OnlineTime) + "', `respect` = '" + _habboStats.Respect + "', `respectGiven` = '" + _habboStats.RespectGiven + "', `giftsGiven` = '" + _habboStats.GiftsGiven + "', `giftsReceived` = '" + _habboStats.GiftsReceived + "', `dailyRespectPoints` = '" + _habboStats.DailyRespectPoints + "', `dailyPetRespectPoints` = '" + _habboStats.DailyPetRespectPoints + "', `AchievementScore` = '" + _habboStats.AchievementPoints + "', `quest_id` = '" + _habboStats.QuestID + "', `quest_progress` = '" + _habboStats.QuestProgress + "', `groupid` = '" + _habboStats.FavouriteGroupId + "',`forum_posts` = '" + _habboStats.ForumPosts + "',`PurchaseUsersConcurrent` = '" + _habboStats.PurchaseUsersConcurrent + "', `vip_gifts` = '" + _habboStats.vipGifts + "' WHERE `id` = '" + Id + "' LIMIT 1;";
            }
        }
        public bool InitProcess()
        {
            _process = new ProcessComponent();
            if (_process.Init(this))
            {
                return true;
            }

            return false;
        }

        public bool InitSearches()
        {
            _navigatorSearches = new SearchesComponent();
            if (_navigatorSearches.Init(this))
            {
                return true;
            }

            return false;
        }

        public bool InitFX()
        {
            _fx = new EffectsComponent();
            if (_fx.Init(this))
            {
                return true;
            }

            return false;
        }

        public bool InitClothing()
        {
            _clothing = new ClothingComponent();
            if (_clothing.Init(this))
            {
                return true;
            }

            return false;
        }

        private bool InitPermissions()
        {
            _permissions = new PermissionComponent();
            if (_permissions.Init(this))
            {
                return true;
            }

            return false;
        }

        public void LoadTalents(Dictionary<int, UserTalent> talents)
        {
            _Talents = talents;
        }

        public UserTalent GetTalentData(int t)
        {
            _Talents.TryGetValue(t, out UserTalent result);

            return result;
        }

        public int GetCurrentTalentLevel()
        {
            int level = _Talents.Values.Select(current => StarBlueServer.GetGame().GetTalentManager().GetTalent(current.TalentId).Level).Concat(new[] { 1 }).Max();
            return level;
        }

        public void InitInformation(UserData data)
        {
            BadgeComponent = new BadgeComponent(this, data);
            Relationships = data.Relations;
        }

        public void Init(GameClient client, UserData data)
        {
            userData = data;
            Achievements = data.achievements;

            FavoriteRooms = new ArrayList();
            foreach (int id in data.favouritedRooms)
            {
                FavoriteRooms.Add(id);
            }

            Tags = new ArrayList();
            foreach (string name in data.tags)
            {
                Tags.Add(name);
            }

            MysticKeys = new ArrayList();
            foreach (string key in data.MysticKeys)
            {
                MysticKeys.Add(key);
            }

            MysticBoxes = new ArrayList();
            foreach (string box in data.MysticBoxes)
            {
                MysticBoxes.Add(box);
            }

            _client = client;
            BadgeComponent = new BadgeComponent(this, data);
            InventoryComponent = new InventoryComponent(Id, client);

            quests = data.quests;

            Messenger = new HabboMessenger(Id);
            Messenger.Init(data.friends, data.requests);
            _friendCount = Convert.ToInt32(data.friends.Count);
            _disconnected = false;
            UsersRooms = data.rooms;
            Relationships = data.Relations;

            InitSearches();
            InitFX();
            InitClothing();
            LoadTalents(data.Talents);
            ClubManager = new ClubManager(Id, data);
            InitCalendar();
            InitPolls();
            InitIgnores();
        }


        public PermissionComponent GetPermissions()
        {
            return _permissions;
        }

        public void OnDisconnect()
        {
            if (_disconnected)
            {
                return;
            }

            try
            {
                if (_process != null)
                {
                    _process.Dispose();
                }
            }
            catch { }

            _disconnected = true;

            if (ClubManager != null)
            {
                ClubManager.Clear();
                ClubManager = null;
            }

            if (_onHelperDuty)
            {
                GameClient Session = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Id);
                HelperToolsManager.RemoveHelper(Session);
            }

            StarBlueServer.GetGame().GetClientManager().UnregisterClient(Id, Username);

            if (!_habboSaved) // GUARDADO DE USER
            {
                _habboSaved = true;
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("UPDATE `users` SET `online` = '0', `last_online` = '" + StarBlueServer.GetUnixTimestamp() + "', `activity_points` = '" + Duckets + "', `disabledevents` = '" + StarBlueServer.BoolToEnum(DisabledNotificationEvents) + "', `disabledmentions` = '" + StarBlueServer.BoolToEnum(DisabledMentions) + "', `credits` = '" + Credits + "',  `vip_points` = '" + Diamonds + "' ,  `bonus_points` = '" + _BonusPoints + "', `home_room` = '" + HomeRoom + "', `gotw_points` = '" + GOTWPoints + "', `puntos_eventos` = '" + UserPoints + "', `time_muted` = '" + TimeMuted + "',`friend_bar_state` = '" + FriendBarStateUtility.GetInt(_friendbarState) + "' WHERE id = '" + Id + "' LIMIT 1;UPDATE `user_stats` SET `roomvisits` = '" + _habboStats.RoomVisits + "', `onlineTime` = '" + (StarBlueServer.GetUnixTimestamp() - SessionStart + _habboStats.OnlineTime) + "', `respect` = '" + _habboStats.Respect + "', `respectGiven` = '" + _habboStats.RespectGiven + "', `giftsGiven` = '" + _habboStats.GiftsGiven + "', `giftsReceived` = '" + _habboStats.GiftsReceived + "', `dailyRespectPoints` = '" + _habboStats.DailyRespectPoints + "', `dailyPetRespectPoints` = '" + _habboStats.DailyPetRespectPoints + "', `AchievementScore` = '" + _habboStats.AchievementPoints + "', `quest_id` = '" + _habboStats.QuestID + "', `quest_progress` = '" + _habboStats.QuestProgress + "', `groupid` = '" + _habboStats.FavouriteGroupId + "',`forum_posts` = '" + _habboStats.ForumPosts + "',`PurchaseUsersConcurrent` = '" + _habboStats.PurchaseUsersConcurrent + "' WHERE `id` = '" + Id + "' LIMIT 1;");

                    if (GetPermissions().HasRight("mod_tickets"))
                    {
                        dbClient.RunFastQuery("UPDATE `moderation_tickets` SET `status` = 'open', `moderator_id` = '0' WHERE `status` ='picked' AND `moderator_id` = '" + Id + "'");
                    }
                }
            }

            Dispose();

            _client = null;

        }

        public void Dispose()
        {
            if (InventoryComponent != null)
            {
                InventoryComponent.SetIdleState();
            }

            if (UsersRooms != null)
            {
                UsersRooms.Clear();
            }

            if (MultiWhispers != null)
            {
                MultiWhispers.Clear();
            }

            if (InRoom && CurrentRoom != null)
            {
                CurrentRoom.GetRoomUserManager().RemoveUserFromRoom(_client, false, false);
            }

            if (Messenger != null)
            {
                Messenger.AppearOffline = true;
                Messenger.Destroy();
            }

            if (_fx != null)
            {
                _fx.Dispose();
            }

            if (_clothing != null)
            {
                _clothing.Dispose();
            }

            if (_permissions != null)
            {
                _permissions.Dispose();
            }

            if (_ignores != null)
            {
                _ignores.Dispose();
            }
        }

        public void CheckCreditsTimer()
        {
            try
            {
                _creditsTickUpdate--;

                if (_creditsTickUpdate <= 0)
                {
                    int CreditUpdate = Convert.ToInt32(StarBlueServer.GetConfig().data["user.credits.update"]);
                    int DucketUpdate = Convert.ToInt32(StarBlueServer.GetConfig().data["user.duckets.update"]);
                    int DiamondUpdate = Convert.ToInt32(StarBlueServer.GetConfig().data["user.diamonds.update"]);
                    int GOTWUpdate = Convert.ToInt32(StarBlueServer.GetConfig().data["user.gotw.update"]);

                    //VIP
                    int CreditVipUpdate = Convert.ToInt32(StarBlueServer.GetConfig().data["vip.credits.update"]);
                    int DucketVipUpdate = Convert.ToInt32(StarBlueServer.GetConfig().data["vip.duckets.update"]);
                    int DiamondVipUpdate = Convert.ToInt32(StarBlueServer.GetConfig().data["vip.diamonds.update"]);
                    int GOTWVipUpdate = Convert.ToInt32(StarBlueServer.GetConfig().data["vip.gotw.update"]);

                    if (this.Rank > 1)
                    {
                        _duckets += DucketVipUpdate;
                        _credits += CreditVipUpdate;
                        _diamonds += DiamondVipUpdate;
                        _gotwPoints += GOTWVipUpdate;
                        _client.SendMessage(new CreditBalanceComposer(_credits));
                        _client.SendMessage(new HabboActivityPointNotificationComposer(_duckets, DucketVipUpdate));
                        _client.SendMessage(new HabboActivityPointNotificationComposer(_diamonds, DiamondVipUpdate, 5));
                        _client.SendMessage(new HabboActivityPointNotificationComposer(_gotwPoints, GOTWVipUpdate, 103));
                        GetClient().SendMessage(RoomNotificationComposer.SendBubble("newuser", "Você recebeu " + DucketVipUpdate + " duckets, " + CreditVipUpdate + " créditos, " + DiamondVipUpdate + " diamantes e " + GOTWVipUpdate + " " + Convert.ToString(StarBlueServer.GetConfig().data["seasonal.currency.name"]) + " por estar conectado no hotel.", ""));
                    }
                    else
                    {
                        _duckets += DucketUpdate;
                        _credits += CreditUpdate;
                        _diamonds += DiamondUpdate;
                        _gotwPoints += GOTWUpdate;
                        _client.SendMessage(new CreditBalanceComposer(_credits));
                        _client.SendMessage(new HabboActivityPointNotificationComposer(_duckets, DucketUpdate));
                        _client.SendMessage(new HabboActivityPointNotificationComposer(_diamonds, DiamondUpdate, 5));
                        _client.SendMessage(new HabboActivityPointNotificationComposer(_gotwPoints, GOTWUpdate, 103));
                        GetClient().SendMessage(RoomNotificationComposer.SendBubble("newuser", "Você recebeu " + DucketUpdate + " duckets, " + CreditUpdate + " créditos, " + DiamondUpdate + " diamantes e " + GOTWUpdate + " " + Convert.ToString(StarBlueServer.GetConfig().data["seasonal.currency.name"]) + " por estar conectado no hotel.", ""));
                    }


                    CreditsUpdateTick = Convert.ToInt32(StarBlueServer.GetConfig().data["amount.time.minutes"]);
                }
            }
            catch { }
        }

        public GameClient GetClient()
        {
            if (_client != null)
            {
                return _client;
            }

            return StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Id);
        }

        public HabboMessenger GetMessenger()
        {
            return Messenger;
        }

        public BadgeComponent GetBadgeComponent()
        {
            return BadgeComponent;
        }

        public InventoryComponent GetInventoryComponent()
        {
            return InventoryComponent;
        }

        public SearchesComponent GetNavigatorSearches()
        {
            return _navigatorSearches;
        }

        public EffectsComponent Effects()
        {
            return _fx;
        }

        public ClothingComponent GetClothing()
        {
            return _clothing;
        }

        public int GetQuestProgress(int p)
        {
            quests.TryGetValue(p, out int progress);
            return progress;
        }

        public UserAchievement GetAchievementData(string p)
        {
            Achievements.TryGetValue(p, out UserAchievement achievement);
            return achievement;
        }

        public void ChangeName(string Username)
        {
            LastNameChange = StarBlueServer.GetUnixTimestamp();
            this.Username = Username;

            SaveKey("username", Username);
            SaveKey("last_change", LastNameChange.ToString());
        }

        public void casinoEvent(string diceRoll)
        {
            if (casinoEnabled)
            {
                casinoCount = casinoCount + int.Parse(diceRoll);
                if (casinoCount > 21)
                {
                    CurrentRoom.SendMessage(RoomNotificationComposer.SendBubble("volada", "O usuário " + Username + " jogue os dados e pegue " + casinoCount + ".", ""));
                    casinoCount = 0;
                    casinoEnabled = false;
                    GetClient().SendWhisper("Modo cassino desativado.", 34);
                }
                else if (casinoCount == 21)
                {
                    CurrentRoom.SendMessage(RoomNotificationComposer.SendBubble("ganador", "El usuario " + Username + " ha sacado " + casinoCount + " en los dados (PL Automatico)", ""));
                    Effects().ApplyEffect(165);
                    casinoCount = 0;
                    casinoEnabled = false;
                    GetClient().SendWhisper("Modo cassino desativado.", 34);
                }
                else
                {
                    CurrentRoom.SendMessage(RoomNotificationComposer.SendBubble("sumando", "El usuario " + Username + " tira los dados y lleva " + casinoCount + ".", ""));
                }

            }
        }

        public void SaveKey(string Key, string Value)
        {
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET " + Key + " = @value WHERE `id` = '" + Id + "' LIMIT 1;");
                dbClient.AddParameter("value", Value);
                dbClient.RunQuery();
            }
        }

        public bool SendWebPacket(WebComposer Message)
        {
            StarBlue.HabboHotel.WebClient.WebClient ClientWeb = StarBlueServer.GetGame().GetWebClientManager().GetClientByUserID(this.Id);
            if (ClientWeb != null)
            {
                ClientWeb.SendPacket(Message);
                return true;
            }

            return false;
        }

        public void PrepareRoom(int Id, string Password)
        {
            GameClient Session = this.GetClient();

            if (Session == null || this == null)
            {
                return;
            }

            if (this.InRoom && this.CurrentRoom != null)
            {
                this.CurrentRoom.GetRoomUserManager().RemoveUserFromRoom(Session, false, false);
            }

            Room Room = StarBlueServer.GetGame().GetRoomManager().LoadRoom(Id);
            if (Room == null || Room.isCrashed || (this.IsTeleporting && this.TeleportingRoomID != Id))
            {
                Session.SendMessage(new CloseConnectionComposer(Session));
                return;
            }

            this.CurrentRoomId = Room.Id;

            if (!this.GetPermissions().HasRight("room_ban_override") && Room.UserIsBanned(this.Id))
            {
                if (Room.HasBanExpired(this.Id))
                {
                    Room.RemoveBan(this.Id);
                }
                else
                {
                    this.RoomAuthOk = false;
                    Session.SendMessage(new CantConnectComposer(4));
                    Session.SendMessage(new CloseConnectionComposer(Session));
                    return;
                }
            }

            if (Room.RoomData.UsersNow >= Room.RoomData.UsersMax && !this.GetPermissions().HasRight("room_enter_full") && this.Id != Room.RoomData.OwnerId)
            {
                Session.SendMessage(new CantConnectComposer(1));
                Session.SendMessage(new CloseConnectionComposer(Session));
                return;
            }

            Session.SendMessage(new OpenConnectionComposer());

            if (!Room.CheckRights(Session, false, true) && !this.IsTeleporting && !this.IsHopping)
            {
                if (Room.RoomData.Access == RoomAccess.DOORBELL && !this.GetPermissions().HasRight("room_enter_locked"))
                {
                    if (Room.UserCount > 0)
                    {
                        Session.SendMessage(new DoorbellComposer(""));
                        Room.SendMessage(new DoorbellComposer(this.Username), true);
                        return;
                    }
                    else
                    {
                        Session.SendMessage(new FlatAccessDeniedComposer(""));
                        Session.SendMessage(new CloseConnectionComposer(Session));
                        return;
                    }
                }
                else if (Room.RoomData.Access == RoomAccess.PASSWORD && !this.GetPermissions().HasRight("room_enter_locked"))
                {
                    if (Password.ToLower() != Room.RoomData.Password.ToLower() || string.IsNullOrWhiteSpace(Password))
                    {
                        Session.SendMessage(new GenericErrorComposer(-100002));
                        Session.SendMessage(new CloseConnectionComposer(Session));
                        return;
                    }
                }
            }

            Session.SendQueue(new RoomReadyComposer(Room.Id, Room.RoomData.ModelName));

            if (Room.RoomData.Wallpaper != "0.0")
            {
                Session.SendQueue(new RoomPropertyComposer("wallpaper", Room.RoomData.Wallpaper));
            }

            if (Room.RoomData.Floor != "0.0")
            {
                Session.SendQueue(new RoomPropertyComposer("floor", Room.RoomData.Floor));
            }

            Session.SendQueue(new RoomPropertyComposer("landscape", Room.RoomData.Landscape));
            Session.SendQueue(new RoomRatingComposer(Room.RoomData.Score, !(this.RatedRooms.Contains(Room.Id) || Room.RoomData.OwnerId == this.Id)));
            Session.Flush();
        }

        public void InitCalendar()
        {
            if (!StarBlueServer.GetGame().GetCalendarManager().CampaignEnable())
            {
                return;
            }

            calendarGift = new bool[StarBlueServer.GetGame().GetCalendarManager().GetTotalDays()];
            for (int i = 0; i < calendarGift.Length; i++)
            {
                calendarGift[i] = false;
            }

            DataTable dTable = null;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT day FROM user_campaign_gifts WHERE user_id = '" + Id + "' AND campaign_name = @name");
                dbClient.AddParameter("name", StarBlueServer.GetGame().GetCalendarManager().GetCampaignName());
                dTable = dbClient.GetTable();
            }

            if (dTable != null)
            {
                foreach (DataRow dRow in dTable.Rows)
                {
                    int Day = (int)dRow["day"];
                    calendarGift[Day - 1] = true;
                }
            }
        }

        public bool InitIgnores()
        {
            _ignores = new IgnoresComponent();

            return _ignores.Init(this);
        }

        public bool EnterRoom(Room Room)
        {
            GameClient Session = this.GetClient();
            if (Room == null || Session == null)
            {
                return false;
            }

            Session.SendQueue(new RoomReadyComposer(Room.Id, Room.RoomData.ModelName));

            if (Room.RoomData.Wallpaper != "0.0")
            {
                Session.SendQueue(new RoomPropertyComposer("wallpaper", Room.RoomData.Wallpaper));
            }

            if (Room.RoomData.Floor != "0.0")
            {
                Session.SendQueue(new RoomPropertyComposer("floor", Room.RoomData.Floor));
            }

            Session.SendQueue(new RoomPropertyComposer("landscape", Room.RoomData.Landscape));
            Session.SendQueue(new RoomRatingComposer(Room.RoomData.Score, !(this.RatedRooms.Contains(Room.Id) || Room.RoomData.OwnerId == this.Id)));
            Session.Flush();

            return true;
        }
    }
}