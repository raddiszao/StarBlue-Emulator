
using log4net;
using StarBlue.Communication.Packets;
using StarBlue.Communication.Packets.Incoming.LandingView;
using StarBlue.Core;
using StarBlue.HabboHotel.Achievements;
using StarBlue.HabboHotel.Badges;
using StarBlue.HabboHotel.Bots;
using StarBlue.HabboHotel.Cache;
using StarBlue.HabboHotel.Calendar;
using StarBlue.HabboHotel.Catalog;
using StarBlue.HabboHotel.Catalog.FurniMatic;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Games;
using StarBlue.HabboHotel.Groups;
using StarBlue.HabboHotel.Groups.Forums;
using StarBlue.HabboHotel.Helpers;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Items.Crafting;
using StarBlue.HabboHotel.Items.RentableSpaces;
using StarBlue.HabboHotel.Items.Televisions;
using StarBlue.HabboHotel.LandingView;
using StarBlue.HabboHotel.LandingView.CommunityGoal;
using StarBlue.HabboHotel.Moderation;
using StarBlue.HabboHotel.Navigator;
using StarBlue.HabboHotel.Permissions;
using StarBlue.HabboHotel.Quests;
using StarBlue.HabboHotel.Rewards;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.Chat;
using StarBlue.HabboHotel.Rooms.Polls;
using StarBlue.HabboHotel.Rooms.TraxMachine;
using StarBlue.HabboHotel.Subscriptions;
using StarBlue.HabboHotel.Talents;
using StarBlue.WebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StarBlue.HabboHotel
{
    public class Game
    {
        private static readonly ILog log = LogManager.GetLogger("StarBlue.HabboHotel.Game");

        public GroupForumManager GetGroupForumManager()
        {
            return _forumManager;
        }

        private GroupForumManager _forumManager;
        private readonly PacketManager _packetManager;
        private readonly GameClientManager _clientManager;
        private readonly ModerationManager _modManager;
        private readonly FrontpageManager _frontpageManager;
        private readonly ItemDataManager _itemDataManager;
        private readonly CatalogManager _catalogManager;
        private readonly TelevisionManager _televisionManager;//TODO: Initialize from the item manager.
        private readonly NavigatorManager _navigatorManager;
        private readonly RoomManager _roomManager;
        private readonly ChatManager _chatManager;
        private readonly GroupManager _groupManager;
        private readonly QuestManager _questManager;
        private readonly AchievementManager _achievementManager;
        private readonly TalentTrackManager _talentTrackManager;
        private readonly LandingViewManager _landingViewManager;//TODO: Rename class
        private readonly GameDataManager _gameDataManager;
        private readonly CraftingManager _craftingManager;
        private readonly ServerStatusUpdater _globalUpdater;
        private readonly BotManager _botManager;
        private readonly CacheManager _cacheManager;
        private readonly RewardManager _rewardManager;
        private readonly BadgeManager _badgeManager;
        private readonly PermissionManager _permissionManager;
        private readonly SubscriptionManager _subscriptionManager;
        private readonly TargetedOffersManager _targetedoffersManager;
        private readonly TalentManager _talentManager;
        private readonly CrackableManager _crackableManager;
        private readonly FurniMaticRewardsManager _furniMaticRewardsManager;
        private readonly PollManager _pollManager;
        private readonly CommunityGoalVS _communityGoalVS;
        private readonly CalendarManager _calendarManager;
        private RentableSpaceManager _rentableSpaceManager;
        private readonly LeaderBoardDataManager _leaderBoardDataManager;
        private readonly WebEventManager _webEventManager;

        private bool _cycleEnded;
        private bool _cycleActive;
        private Task _gameCycle;
        private int _cycleSleepTime = 25;

        public Game()
        {
            _packetManager = new PacketManager();
            _rentableSpaceManager = new RentableSpaceManager();
            _clientManager = new GameClientManager();
            _modManager = new ModerationManager();

            _itemDataManager = new ItemDataManager();
            _itemDataManager.Init();

            _catalogManager = new CatalogManager();
            _catalogManager.Init(_itemDataManager);

            _frontpageManager = new FrontpageManager();

            _televisionManager = new TelevisionManager();
            _crackableManager = new CrackableManager();
            _crackableManager.Initialize(StarBlueServer.GetDatabaseManager().GetQueryReactor());
            _furniMaticRewardsManager = new FurniMaticRewardsManager();
            _furniMaticRewardsManager.Initialize(StarBlueServer.GetDatabaseManager().GetQueryReactor());

            _craftingManager = new CraftingManager();
            _craftingManager.Init();

            _navigatorManager = new NavigatorManager();
            _roomManager = new RoomManager();
            _chatManager = new ChatManager();
            _groupManager = new GroupManager();
            _questManager = new QuestManager();
            _achievementManager = new AchievementManager();

            _talentManager = new TalentManager();
            _talentManager.Initialize();
            _talentTrackManager = new TalentTrackManager();

            _landingViewManager = new LandingViewManager();
            _gameDataManager = new GameDataManager();

            _globalUpdater = new ServerStatusUpdater();
            _globalUpdater.Init();

            _botManager = new BotManager();

            _cacheManager = new CacheManager();
            _rewardManager = new RewardManager();

            _badgeManager = new BadgeManager();
            _badgeManager.Init();

            _forumManager = new GroupForumManager();

            _communityGoalVS = new CommunityGoalVS();
            _communityGoalVS.LoadCommunityGoalVS();

            _permissionManager = new PermissionManager();
            _permissionManager.Init();

            _subscriptionManager = new SubscriptionManager();
            _subscriptionManager.Init();

            _calendarManager = new CalendarManager();
            _calendarManager.Init();

            _leaderBoardDataManager = new LeaderBoardDataManager();

            _targetedoffersManager = new TargetedOffersManager();
            _targetedoffersManager.Initialize(StarBlueServer.GetDatabaseManager().GetQueryReactor());

            _pollManager = new PollManager();
            _pollManager.Init();

            HelperToolsManager.Init();
            TraxSoundManager.Init();
            GetHallOfFame.Load();

            _webEventManager = new WebEventManager();
            _webEventManager.Init();

        }

        public void StartGameLoop()
        {
            _gameCycle = new Task(GameCycle);
            _gameCycle.Start();

            _cycleActive = true;
        }

        private void GameCycle()
        {
            while (_cycleActive)
            {
                _cycleEnded = false;

                StarBlueServer.GetGame().GetRoomManager().OnCycle();
                StarBlueServer.GetGame().GetClientManager().OnCycle();
                //AlphaManager.getInstance().onCycle();
                _cycleEnded = true;
                Thread.Sleep(_cycleSleepTime);
            }
        }

        public void StopGameLoop()
        {
            _cycleActive = false;

            while (!_cycleEnded)
            {
                Thread.Sleep(_cycleSleepTime);
            }
        }

        public PacketManager GetPacketManager()
        {
            return _packetManager;
        }

        public GameClientManager GetClientManager()
        {
            return _clientManager;
        }

        public CatalogManager GetCatalog()
        {
            return _catalogManager;
        }

        public NavigatorManager GetNavigator()
        {
            return _navigatorManager;
        }

        public CalendarManager GetCalendarManager()
        {
            return _calendarManager;
        }

        public FrontpageManager GetCatalogFrontPageManager()
        {
            return _frontpageManager;
        }

        public ItemDataManager GetItemManager()
        {
            return _itemDataManager;
        }

        public RoomManager GetRoomManager()
        {
            return _roomManager;
        }

        internal TargetedOffersManager GetTargetedOffersManager()
        {
            return _targetedoffersManager;
        }

        public AchievementManager GetAchievementManager()
        {
            return _achievementManager;
        }

        public TalentTrackManager GetTalentTrackManager()
        {
            return _talentTrackManager;
        }

        public TalentManager GetTalentManager()
        {
            return _talentManager;

        }

        public ModerationManager GetModerationManager()
        {
            return _modManager;
        }

        public PermissionManager GetPermissionManager()
        {
            return _permissionManager;
        }

        public SubscriptionManager GetSubscriptionManager()
        {
            return _subscriptionManager;
        }

        public QuestManager GetQuestManager()
        {
            return _questManager;
        }

        public RentableSpaceManager GetRentableSpaceManager()
        {
            return _rentableSpaceManager;
        }

        public GroupManager GetGroupManager()
        {
            return _groupManager;
        }

        public LandingViewManager GetLandingManager()
        {
            return _landingViewManager;
        }

        public WebEventManager GetWebEventManager()
        {
            return _webEventManager;
        }

        public TelevisionManager GetTelevisionManager()
        {
            return _televisionManager;
        }

        public ChatManager GetChatManager()
        {
            return _chatManager;
        }

        internal CrackableManager GetPinataManager()
        {
            return _crackableManager;
        }

        public CraftingManager GetCraftingManager()
        {
            return _craftingManager;
        }

        public FurniMaticRewardsManager GetFurniMaticRewardsMnager()
        {
            return _furniMaticRewardsManager;
        }

        public GameDataManager GetGameDataManager()
        {
            return _gameDataManager;
        }

        public BotManager GetBotManager()
        {
            return _botManager;
        }

        public CacheManager GetCacheManager()
        {
            return _cacheManager;
        }

        public RewardManager GetRewardManager()
        {
            return _rewardManager;
        }


        internal LeaderBoardDataManager GetLeaderBoardDataManager()
        {
            return _leaderBoardDataManager;
        }


        public BadgeManager GetBadgeManager()
        {
            return _badgeManager;
        }


        internal object GetFilterComponent()
        {
            throw new NotImplementedException();
        }

        public PollManager GetPollManager()
        {
            return _pollManager;
        }

        public CommunityGoalVS GetCommunityGoalVS()
        {
            return _communityGoalVS;
        }
    }
}