using log4net;
using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Incoming.Avatar;
using StarBlue.Communication.Packets.Incoming.Calendar;
using StarBlue.Communication.Packets.Incoming.Catalog;
using StarBlue.Communication.Packets.Incoming.GameCenter;
using StarBlue.Communication.Packets.Incoming.Groups;
using StarBlue.Communication.Packets.Incoming.Groups.Forums;
using StarBlue.Communication.Packets.Incoming.Handshake;
using StarBlue.Communication.Packets.Incoming.Help;
using StarBlue.Communication.Packets.Incoming.Help.Helpers;
using StarBlue.Communication.Packets.Incoming.Inventory.Achievements;
using StarBlue.Communication.Packets.Incoming.Inventory.AvatarEffects;
using StarBlue.Communication.Packets.Incoming.Inventory.Badges;
using StarBlue.Communication.Packets.Incoming.Inventory.Bots;
using StarBlue.Communication.Packets.Incoming.Inventory.Furni;
using StarBlue.Communication.Packets.Incoming.Inventory.Pets;
using StarBlue.Communication.Packets.Incoming.Inventory.Purse;
using StarBlue.Communication.Packets.Incoming.Inventory.Trading;
using StarBlue.Communication.Packets.Incoming.LandingView;
using StarBlue.Communication.Packets.Incoming.Marketplace;
using StarBlue.Communication.Packets.Incoming.Messenger;
using StarBlue.Communication.Packets.Incoming.Misc;
using StarBlue.Communication.Packets.Incoming.Moderation;
using StarBlue.Communication.Packets.Incoming.Navigator;
using StarBlue.Communication.Packets.Incoming.Nuxs;
using StarBlue.Communication.Packets.Incoming.Quests;
using StarBlue.Communication.Packets.Incoming.QuickPolls;
using StarBlue.Communication.Packets.Incoming.Rooms.Action;
using StarBlue.Communication.Packets.Incoming.Rooms.AI.Bots;
using StarBlue.Communication.Packets.Incoming.Rooms.AI.Pets;
using StarBlue.Communication.Packets.Incoming.Rooms.AI.Pets.Horse;
using StarBlue.Communication.Packets.Incoming.Rooms.Avatar;
using StarBlue.Communication.Packets.Incoming.Rooms.Camera;
using StarBlue.Communication.Packets.Incoming.Rooms.Chat;
using StarBlue.Communication.Packets.Incoming.Rooms.Connection;
using StarBlue.Communication.Packets.Incoming.Rooms.Engine;
using StarBlue.Communication.Packets.Incoming.Rooms.FloorPlan;
using StarBlue.Communication.Packets.Incoming.Rooms.Furni;
using StarBlue.Communication.Packets.Incoming.Rooms.Furni.LoveLocks;
using StarBlue.Communication.Packets.Incoming.Rooms.Furni.Moodlight;
using StarBlue.Communication.Packets.Incoming.Rooms.Furni.RentableSpaces;
using StarBlue.Communication.Packets.Incoming.Rooms.Furni.Stickys;
using StarBlue.Communication.Packets.Incoming.Rooms.Furni.Wired;
using StarBlue.Communication.Packets.Incoming.Rooms.Furni.YouTubeTelevisions;
using StarBlue.Communication.Packets.Incoming.Rooms.Nux;
using StarBlue.Communication.Packets.Incoming.Rooms.Polls;
using StarBlue.Communication.Packets.Incoming.Rooms.Settings;
using StarBlue.Communication.Packets.Incoming.SMS;
using StarBlue.Communication.Packets.Incoming.Sound;
using StarBlue.Communication.Packets.Incoming.Talents;
using StarBlue.Communication.Packets.Incoming.Users;
using StarBlue.Communication.Packets.Incoming.WebSocket;
using StarBlue.Communication.WebSocket;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms.Instance;
using StarBlue.HabboHotel.WebClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StarBlue.Communication.Packets
{
    public sealed class PacketManager
    {
        private static readonly ILog log = LogManager.GetLogger("StarBlue.Communication.Packets");

        private readonly bool IgnoreTasks = true;

        private readonly int MaximumRunTimeInSec = 300; // 5 minutes

        private readonly bool ThrowUserErrors = false;

        private readonly TaskFactory _eventDispatcher;

        private readonly Dictionary<int, IPacketEvent> _incomingPackets;

        private readonly Dictionary<int, IPacketWebEvent> _incomingWebPackets;

        private readonly ConcurrentDictionary<int, Task> _runningTasks;

        public PacketManager()
        {
            _incomingWebPackets = new Dictionary<int, IPacketWebEvent>();
            _incomingPackets = new Dictionary<int, IPacketEvent>();

            _eventDispatcher = new TaskFactory(TaskCreationOptions.PreferFairness, TaskContinuationOptions.None);
            _runningTasks = new ConcurrentDictionary<int, Task>();

            RegisterHandshake();
            RegisterAdventCalendar();
            RegisterLandingView();
            RegisterCatalog();
            RegisterMarketplace();
            RegisterNavigator();
            RegisterNewNavigator();
            RegisterRoomAction();
            RegisterHelperTool();
            RegisterQuests();
            RegisterRoomConnection();
            RegisterRoomChat();
            RegisterRoomEngine();
            RegisterFurni();
            RegisterUsers();
            RegisterSound();
            RegisterMisc();
            RegisterInventory();
            RegisterTalents();
            RegisterForums();
            RegisterPurse();
            RegisterRoomAvatar();
            RegisterAvatar();
            RegisterMessenger();
            RegisterGroups();
            RegisterRoomSettings();
            RegisterPets();
            RegisterBots();
            RegisterHelp();
            FloorPlanEditor();
            RegisterPolls();
            RegisterRoomPolls();
            RegisterModeration();
            RegisterGameCenter();
            RegisterRoomCamera();
            RegisterNux();
            RegisterSMS();
            RegisterWebEvents();
        }

        public void TryExecutePacket(GameClient Session, MessageEvent Packet)
        {
            if (Session == null)
                return;

            if (!_incomingPackets.TryGetValue(Packet.Id, out IPacketEvent OutPacket))
            {
                if (Convert.ToBoolean(StarBlueServer.GetConfig().data["debug"]))
                {
                    log.Debug("Unhandled Packet: " + Packet.Id);
                }

                return;
            }

            if (Convert.ToBoolean(StarBlueServer.GetConfig().data["debug"]))
            {
                if (_incomingPackets.ContainsKey(Packet.Id))
                {
                    log.Debug("Handled Packet: [" + Packet.Id + "] " + _incomingPackets[Packet.Id].GetType().Name);
                }
                else
                {
                    log.Debug("Handled Packet: [" + Packet.Id + "] UnnamedPacketEvent");
                }
            }

            if (!IgnoreTasks)
            {
                ExecutePacketAsync(Session, Packet, OutPacket);
            }
            else
            {
                OutPacket.Parse(Session, Packet);
            }
        }

        public void TryExecuteWebPacket(WebClient Session, MessageWebEvent Packet)
        {
            if (!_incomingWebPackets.TryGetValue(Packet.Id, out IPacketWebEvent Pak))
            {
                return;
            }

            Pak.Parse(Session, Packet);
        }

        private void ExecutePacketAsync(GameClient Session, MessageEvent Packet, IPacketEvent Pak)
        {
            DateTime Start = DateTime.Now;

            CancellationTokenSource CancelSource = new CancellationTokenSource();
            CancellationToken Token = CancelSource.Token;

            Task t = _eventDispatcher.StartNew(() =>
            {
                Pak.Parse(Session, Packet);
                Token.ThrowIfCancellationRequested();
            }, Token);

            _runningTasks.TryAdd(t.Id, t);

            try
            {
                if (!t.Wait(MaximumRunTimeInSec * 1000, Token))
                {
                    CancelSource.Cancel();
                }
            }
            catch (AggregateException ex)
            {
                foreach (Exception e in ex.Flatten().InnerExceptions)
                {
                    if (ThrowUserErrors)
                    {
                        throw e;
                    }
                    else
                    {
                        //log.Fatal("Unhandled Error: " + e.Message + " - " + e.StackTrace);
                        Session.Disconnect();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Session.Disconnect();
            }
            finally
            {
                _runningTasks.TryRemove(t.Id, out Task RemovedTask);

                CancelSource.Dispose();

                log.Warn("Event took " + (DateTime.Now - Start).Milliseconds + "ms to complete.");
            }
        }

        public void WaitForAllToComplete()
        {
            foreach (Task t in _runningTasks.Values.ToList())
            {
                t.Wait();
            }
        }

        public void UnregisterAll()
        {
            _incomingPackets.Clear();
        }

        private void RegisterWebEvents()
        {
            _incomingWebPackets.Add(1, new SSOWebTicketEvent());
            _incomingWebPackets.Add(2, new BuilderToolEvent());
            _incomingWebPackets.Add(3, new EnterRoomEvent());
            _incomingWebPackets.Add(4, new EmojiEvent());
            _incomingWebPackets.Add(12, new PinVerifyEvent());
        }

        private void RegisterHandshake()
        {
            _incomingPackets.Add(Events.GetClientVersionMessageEvent, new GetClientVersionEvent());
            _incomingPackets.Add(Events.InitCryptoMessageEvent, new InitCryptoEvent());
            _incomingPackets.Add(Events.GenerateSecretKeyMessageEvent, new GenerateSecretKeyEvent());
            _incomingPackets.Add(Events.UniqueIDMessageEvent, new UniqueIDEvent());
            _incomingPackets.Add(Events.SSOTicketMessageEvent, new SSOTicketEvent());
            _incomingPackets.Add(Events.InfoRetrieveMessageEvent, new InfoRetrieveEvent());
            _incomingPackets.Add(Events.PingMessageEvent, new PingEvent());

        }

        private void RegisterLandingView()
        {
            _incomingPackets.Add(Events.RefreshCampaignMessageEvent, new RefreshCampaignEvent());
            _incomingPackets.Add(Events.GetPromoArticlesMessageEvent, new GetPromoArticlesEvent());
            _incomingPackets.Add(Events.GiveConcurrentUsersRewardEvent, new GiveConcurrentUsersReward());
            _incomingPackets.Add(Events.VoteCommunityGoalVS, new VoteCommunityGoalVS());
            _incomingPackets.Add(Events.CommunityGoalEvent, new CommunityGoalEvent());
        }

        private void RegisterRoomCamera()
        {
            _incomingPackets.Add(Events.SetRoomThumbnailMessageEvent, new SetRoomThumbnailMessageEvent());
            _incomingPackets.Add(Events.HabboCameraPictureDataMessageEvent, new HabboCameraPictureDataMessageEvent());
            _incomingPackets.Add(Events.PurchaseCameraPictureMessageEvent, new PurchaseCameraPictureMessageEvent());
            _incomingPackets.Add(Events.PublishCameraPictureMessageEvent, new PublishCameraPictureMessageEvent());
            _incomingPackets.Add(Events.ParticipatePictureCameraCompetitionMessageEvent, new ParticipatePictureCameraCompetitionMessageEvent());

        }

        private void RegisterNux()
        {

            _incomingPackets.Add(Events.RoomNuxAlert, new RoomNuxAlert());
        }

        private void RegisterSMS()
        {
            _incomingPackets.Add(Events.SmsVerification, new SMSVerification());
        }

        private void RegisterCatalog()
        {
            _incomingPackets.Add(Events.GetCatalogModeMessageEvent, new GetCatalogModeEvent());
            _incomingPackets.Add(Events.BuyTargettedOfferMessageEvent, new BuyTargettedOfferMessage());
            _incomingPackets.Add(Events.GetCatalogIndexMessageEvent, new GetCatalogIndexEvent());
            _incomingPackets.Add(Events.GetCatalogPageMessageEvent, new GetCatalogPageEvent());
            _incomingPackets.Add(Events.GetCatalogOfferMessageEvent, new GetCatalogOfferEvent());
            _incomingPackets.Add(Events.PurchaseFromCatalogMessageEvent, new PurchaseFromCatalogEvent());
            _incomingPackets.Add(Events.PurchaseFromCatalogAsGiftMessageEvent, new PurchaseFromCatalogAsGiftEvent());
            _incomingPackets.Add(Events.PurchaseRoomPromotionMessageEvent, new PurchaseRoomPromotionEvent());
            _incomingPackets.Add(Events.GetGiftWrappingConfigurationMessageEvent, new GetGiftWrappingConfigurationEvent());
            _incomingPackets.Add(Events.GetMarketplaceConfigurationMessageEvent, new GetMarketplaceConfigurationEvent());
            _incomingPackets.Add(Events.CheckPetNameMessageEvent, new CheckPetNameEvent());
            _incomingPackets.Add(Events.RedeemVoucherMessageEvent, new RedeemVoucherEvent());
            _incomingPackets.Add(Events.GetSellablePetBreedsMessageEvent, new GetSellablePetBreedsEvent());
            _incomingPackets.Add(Events.GetPromotableRoomsMessageEvent, new GetPromotableRoomsEvent());
            _incomingPackets.Add(Events.GetCatalogRoomPromotionMessageEvent, new GetCatalogRoomPromotionEvent());
            _incomingPackets.Add(Events.GetNuxPresentEvent, new GetNuxPresentEvent());
            _incomingPackets.Add(Events.GetGroupFurniConfigMessageEvent, new GetGroupFurniConfigEvent());
            _incomingPackets.Add(Events.CheckGnomeNameMessageEvent, new CheckGnomeNameEvent());
            //this._incomingPackets.Add(ClientPacketHeader.GetClubGiftsMessageEvent, new GetClubGiftsEvent());
            _incomingPackets.Add(Events.LTDCountdownEvent, new LTDCountdownEvent());
            _incomingPackets.Add(Events.RedeemHCGiftEvent, new RedeemHCGiftEvent());
            _incomingPackets.Add(Events.FurniMaticPageEvent, new FurniMaticPageEvent());
            _incomingPackets.Add(Events.FurniMaticRecycleEvent, new FurniMaticRecycleEvent());
            _incomingPackets.Add(Events.FurniMaticRewardsEvent, new FurniMaticRewardsEvent());

        }

        private void RegisterMarketplace()
        {
            _incomingPackets.Add(Events.GetOffersMessageEvent, new GetOffersEvent());
            _incomingPackets.Add(Events.GetOwnOffersMessageEvent, new GetOwnOffersEvent());
            _incomingPackets.Add(Events.GetMarketplaceCanMakeOfferMessageEvent, new GetMarketplaceCanMakeOfferEvent());
            _incomingPackets.Add(Events.GetMarketplaceItemStatsMessageEvent, new GetMarketplaceItemStatsEvent());
            _incomingPackets.Add(Events.MakeOfferMessageEvent, new MakeOfferEvent());
            _incomingPackets.Add(Events.CancelOfferMessageEvent, new CancelOfferEvent());
            _incomingPackets.Add(Events.BuyOfferMessageEvent, new BuyOfferEvent());
            _incomingPackets.Add(Events.RedeemOfferCreditsMessageEvent, new RedeemOfferCreditsEvent());
        }

        private void RegisterNavigator()
        {
            _incomingPackets.Add(Events.AddFavouriteRoomMessageEvent, new AddFavouriteRoomEvent());
            _incomingPackets.Add(Events.GetUserFlatCatsMessageEvent, new GetUserFlatCatsEvent());
            _incomingPackets.Add(Events.DeleteFavouriteRoomMessageEvent, new RemoveFavouriteRoomEvent());
            _incomingPackets.Add(Events.GoToHotelViewMessageEvent, new GoToHotelViewEvent());
            _incomingPackets.Add(Events.UpdateNavigatorSettingsMessageEvent, new UpdateNavigatorSettingsEvent());
            _incomingPackets.Add(Events.CanCreateRoomMessageEvent, new CanCreateRoomEvent());
            _incomingPackets.Add(Events.CreateFlatMessageEvent, new CreateFlatEvent());
            _incomingPackets.Add(Events.GetGuestRoomMessageEvent, new GetGuestRoomEvent());
            _incomingPackets.Add(Events.EditRoomPromotionMessageEvent, new EditRoomEventEvent());
            _incomingPackets.Add(Events.GetEventCategoriesMessageEvent, new GetNavigatorFlatsEvent());
            _incomingPackets.Add(Events.StaffPickRoomEvent, new StaffPickRoomEvent());
        }

        public void RegisterNewNavigator()
        {
            _incomingPackets.Add(Events.InitializeNewNavigatorMessageEvent, new InitializeNewNavigatorEvent());
            _incomingPackets.Add(Events.NewNavigatorSearchMessageEvent, new NewNavigatorSearchEvent());
            _incomingPackets.Add(Events.FindRandomFriendingRoomMessageEvent, new FindRandomFriendingRoomEvent());
        }

        private void RegisterQuests()
        {
            _incomingPackets.Add(Events.GetQuestListMessageEvent, new GetQuestListEvent());
            _incomingPackets.Add(Events.StartQuestMessageEvent, new StartQuestEvent());
            _incomingPackets.Add(Events.CancelQuestMessageEvent, new CancelQuestEvent());
            _incomingPackets.Add(Events.GetCurrentQuestMessageEvent, new GetCurrentQuestEvent());
            _incomingPackets.Add(Events.GetDailyQuestMessageEvent, new GetDailyQuestEvent());
            //this._incomingPackets.Add(ClientPacketHeader.GetCommunityGoalHallOfFameMessageEvent, new GetCommunityGoalHallOfFameEvent());
        }

        private void RegisterHelp()
        {
            _incomingPackets.Add(Events.OnBullyClickMessageEvent, new OnBullyClickEvent());
            _incomingPackets.Add(Events.SendBullyReportMessageEvent, new SendBullyReportEvent());
            _incomingPackets.Add(Events.SubmitBullyReportMessageEvent, new SubmitBullyReportEvent());
            _incomingPackets.Add(Events.GetSanctionStatusMessageEvent, new GetSanctionStatusEvent());
        }

        private void RegisterRoomAction()
        {
            _incomingPackets.Add(Events.LetUserInMessageEvent, new LetUserInEvent());
            _incomingPackets.Add(Events.BanUserMessageEvent, new BanUserEvent());
            _incomingPackets.Add(Events.KickUserMessageEvent, new KickUserEvent());
            _incomingPackets.Add(Events.AssignRightsMessageEvent, new AssignRightsEvent());
            _incomingPackets.Add(Events.RemoveRightsMessageEvent, new RemoveRightsEvent());
            _incomingPackets.Add(Events.RemoveAllRightsMessageEvent, new RemoveAllRightsEvent());
            _incomingPackets.Add(Events.MuteUserMessageEvent, new MuteUserEvent());
            _incomingPackets.Add(Events.AmbassadorWarningMessageEvent, new AmbassadorWarningMessageEvent());
            _incomingPackets.Add(Events.GiveHandItemMessageEvent, new GiveHandItemEvent());
            _incomingPackets.Add(Events.RemoveMyRightsMessageEvent, new RemoveMyRightsEvent());
        }

        private void RegisterHelperTool()
        {
            _incomingPackets.Add(Events.HandleHelperToolMessageEvent, new HandleHelperToolEvent());
            _incomingPackets.Add(Events.CallForHelperMessageEvent, new CallForHelperEvent());
            _incomingPackets.Add(Events.AcceptHelperSessionMessageEvent, new AcceptHelperSessionEvent());
            _incomingPackets.Add(Events.CancelCallForHelperMessageEvent, new CancelCallForHelperEvent());
            _incomingPackets.Add(Events.FinishHelperSessionMessageEvent, new FinishHelperSessionEvent());
            _incomingPackets.Add(Events.CloseHelperChatSessionMessageEvent, new CloseHelperChatSessionEvent());
            _incomingPackets.Add(Events.HelperSessioChatTypingMessageEvent, new HelperSessionChatTypingEvent());
            _incomingPackets.Add(Events.HelperSessioChatSendMessageMessageEvent, new HelperSessionChatSendMessageEvent());
            _incomingPackets.Add(Events.InvinteHelperUserSessionMessageEvent, new InvinteHelperUserSessionEvent());
            _incomingPackets.Add(Events.VisitHelperUserSessionMessageEvent, new VisitHelperUserSessionEvent());
            _incomingPackets.Add(Events.ReportBullyUserMessageEvent, new ReportBullyUserEvent());
            _incomingPackets.Add(Events.AcceptJoinJudgeChatMessageEvent, new AcceptJoinJudgeChatEvent());
        }

        private void RegisterAvatar()
        {
            _incomingPackets.Add(Events.GetWardrobeMessageEvent, new GetWardrobeEvent());
            _incomingPackets.Add(Events.SaveWardrobeOutfitMessageEvent, new SaveWardrobeOutfitEvent());
        }

        private void RegisterRoomAvatar()
        {
            _incomingPackets.Add(Events.ActionMessageEvent, new ActionEvent());
            _incomingPackets.Add(Events.ApplySignMessageEvent, new ApplySignEvent());
            _incomingPackets.Add(Events.DanceMessageEvent, new DanceEvent());
            _incomingPackets.Add(Events.SitMessageEvent, new SitEvent());
            _incomingPackets.Add(Events.ChangeMottoMessageEvent, new ChangeMottoEvent());
            _incomingPackets.Add(Events.LookToMessageEvent, new LookToEvent());
            _incomingPackets.Add(Events.DropHandItemMessageEvent, new DropHandItemEvent());
            _incomingPackets.Add(Events.GiveRoomScoreMessageEvent, new GiveRoomScoreEvent());
            _incomingPackets.Add(Events.IgnoreUserMessageEvent, new IgnoreUserEvent());
            _incomingPackets.Add(Events.UnIgnoreUserMessageEvent, new UnIgnoreUserEvent());
        }

        private void RegisterRoomConnection()
        {
            _incomingPackets.Add(Events.OpenFlatConnectionMessageEvent, new OpenFlatConnectionEvent());
            _incomingPackets.Add(Events.GoToFlatMessageEvent, new GoToFlatEvent());
        }

        private void RegisterRoomChat()
        {
            _incomingPackets.Add(Events.ChatMessageEvent, new ChatEvent());
            _incomingPackets.Add(Events.ShoutMessageEvent, new ShoutEvent());
            _incomingPackets.Add(Events.WhisperMessageEvent, new WhisperEvent());
            _incomingPackets.Add(Events.StartTypingMessageEvent, new StartTypingEvent());
            _incomingPackets.Add(Events.CancelTypingMessageEvent, new CancelTypingEvent());
        }

        private void RegisterRoomEngine()
        {
            _incomingPackets.Add(Events.GetRoomEntryDataMessageEvent, new GetRoomEntryDataEvent());
            _incomingPackets.Add(Events.GoToFlatAsSpectatorEvent, new GoToFlatAsSpectatorEvent());
            _incomingPackets.Add(Events.GetFurnitureAliasesMessageEvent, new GetFurnitureAliasesEvent());
            _incomingPackets.Add(Events.MoveAvatarMessageEvent, new MoveAvatarEvent());
            _incomingPackets.Add(Events.MoveObjectMessageEvent, new MoveObjectEvent());
            _incomingPackets.Add(Events.PickupObjectMessageEvent, new PickupObjectEvent());
            _incomingPackets.Add(Events.MoveWallItemMessageEvent, new MoveWallItemEvent());
            _incomingPackets.Add(Events.ApplyDecorationMessageEvent, new ApplyDecorationEvent());
            _incomingPackets.Add(Events.PlaceObjectMessageEvent, new PlaceObjectEvent());
            _incomingPackets.Add(Events.UseFurnitureMessageEvent, new UseFurnitureEvent());
            _incomingPackets.Add(Events.UseWallItemMessageEvent, new UseWallItemEvent());
            //_incomingPackets.Add(ClientPacketHeader.PlaceBuilderItemMessageEvent, new PlaceBuilderObjectEvent());
            _incomingPackets.Add(Events.EventTrackerMessageEvent, new EventTrackerEvent());
        }

        private void RegisterInventory()
        {
            _incomingPackets.Add(Events.InitTradeMessageEvent, new InitTradeEvent());
            _incomingPackets.Add(Events.TradingOfferItemMessageEvent, new TradingOfferItemEvent());
            _incomingPackets.Add(Events.TradingOfferItemsMessageEvent, new TradingOfferItemsEvent());
            _incomingPackets.Add(Events.TradingRemoveItemMessageEvent, new TradingRemoveItemEvent());
            _incomingPackets.Add(Events.TradingAcceptMessageEvent, new TradingAcceptEvent());
            _incomingPackets.Add(Events.TradingCancelMessageEvent, new TradingCancelEvent());
            _incomingPackets.Add(Events.TradingConfirmMessageEvent, new TradingConfirmEvent());
            _incomingPackets.Add(Events.TradingModifyMessageEvent, new TradingModifyEvent());
            _incomingPackets.Add(Events.TradingCancelConfirmMessageEvent, new TradingCancelConfirmEvent());
            _incomingPackets.Add(Events.RequestFurniInventoryMessageEvent, new RequestFurniInventoryEvent());
            _incomingPackets.Add(Events.GetBadgesMessageEvent, new GetBadgesEvent());
            _incomingPackets.Add(Events.GetAchievementsMessageEvent, new GetAchievementsEvent());
            _incomingPackets.Add(Events.SetActivatedBadgesMessageEvent, new SetActivatedBadgesEvent());
            _incomingPackets.Add(Events.GetBotInventoryMessageEvent, new GetBotInventoryEvent());
            _incomingPackets.Add(Events.GetPetInventoryMessageEvent, new GetPetInventoryEvent());
            _incomingPackets.Add(Events.AvatarEffectActivatedMessageEvent, new AvatarEffectActivatedEvent());
            _incomingPackets.Add(Events.AvatarEffectSelectedMessageEvent, new AvatarEffectSelectedEvent());
        }

        private void RegisterTalents()
        {
            _incomingPackets.Add(Events.GetTalentTrackMessageEvent, new GetTalentTrackEvent());
            _incomingPackets.Add(Events.CheckQuizTypeEvent, new CheckQuizType());
            _incomingPackets.Add(Events.PostQuizAnswersMessageEvent, new PostQuizAnswersMessage());

        }

        private void RegisterPolls()
        {
            _incomingPackets.Add(Events.SubmitPollAnswerMessageEvent, new SubmitPollAnswerMessageEvent());

        }

        private void RegisterPurse()
        {
            _incomingPackets.Add(Events.GetCreditsInfoMessageEvent, new GetCreditsInfoEvent());
            _incomingPackets.Add(Events.GetHabboClubWindowMessageEvent, new GetHabboClubWindowEvent());
            _incomingPackets.Add(Events.GetHabboClubCenterInfoMessageEvent, new GetHabboClubCenterInfoMessageEvent());
            _incomingPackets.Add(Events.GetCameraPriceEvent, new GetCameraPriceEvent());
        }

        private void RegisterUsers()
        {
            _incomingPackets.Add(Events.ScrGetUserInfoMessageEvent, new ScrGetUserInfoEvent());
            _incomingPackets.Add(Events.SetChatPreferenceMessageEvent, new SetChatPreferenceEvent());
            _incomingPackets.Add(Events.SetUserFocusPreferenceEvent, new SetUserFocusPreferenceEvent());
            _incomingPackets.Add(Events.SetMessengerInviteStatusMessageEvent, new SetMessengerInviteStatusEvent());
            _incomingPackets.Add(Events.RespectUserMessageEvent, new RespectUserEvent());
            _incomingPackets.Add(Events.UpdateFigureDataMessageEvent, new UpdateFigureDataEvent());
            _incomingPackets.Add(Events.OpenPlayerProfileMessageEvent, new OpenPlayerProfileEvent());
            _incomingPackets.Add(Events.GetSelectedBadgesMessageEvent, new GetSelectedBadgesEvent());
            _incomingPackets.Add(Events.GetRelationshipsMessageEvent, new GetRelationshipsEvent());
            _incomingPackets.Add(Events.SetRelationshipMessageEvent, new SetRelationshipEvent());
            _incomingPackets.Add(Events.CheckValidNameMessageEvent, new CheckValidNameEvent());
            _incomingPackets.Add(Events.ChangeNameMessageEvent, new ChangeNameEvent());
            _incomingPackets.Add(Events.SetUsernameMessageEvent, new SetUsernameEvent());
            _incomingPackets.Add(Events.GetHabboGroupBadgesMessageEvent, new GetHabboGroupBadgesEvent());
            _incomingPackets.Add(Events.GetUserTagsMessageEvent, new GetUserTagsEvent());
            _incomingPackets.Add(Events.GetIgnoredUsersMessageEvent, new GetIgnoredUsersEvent());
        }

        private void RegisterSound()
        {
            _incomingPackets.Add(Events.SetSoundSettingsMessageEvent, new SetSoundSettingsEvent());
            _incomingPackets.Add(Events.GetSongInfoMessageEvent, new GetSongInfoEvent());
            _incomingPackets.Add(Events.GetJukeboxPlaylistMessageEvent, new GetJukeboxPlayListEvent());
            _incomingPackets.Add(Events.LoadJukeboxDiscsMessageEvent, new LoadJukeboxDiscsEvent());
            _incomingPackets.Add(Events.GetJukeboxDiscsDataMessageEvent, new GetJukeboxDiscsDataEvent());
            _incomingPackets.Add(Events.AddDiscToPlayListMessageEvent, new AddDiscToPlayListEvent());
            _incomingPackets.Add(Events.RemoveDiscFromPlayListMessageEvent, new RemoveDiscFromPlayListEvent());
        }


        private void RegisterMisc()
        {
            _incomingPackets.Add(Events.UnknownQuestMessageEvent, new GetQuestListEvent());
            _incomingPackets.Add(Events.ClientVariablesMessageEvent, new ClientVariablesEvent());
            _incomingPackets.Add(Events.DisconnectionMessageEvent, new DisconnectEvent());
            _incomingPackets.Add(Events.LatencyTestMessageEvent, new LatencyTestEvent());
            _incomingPackets.Add(Events.MemoryPerformanceMessageEvent, new MemoryPerformanceEvent());
            _incomingPackets.Add(Events.SetFriendBarStateMessageEvent, new SetFriendBarStateEvent());
            _incomingPackets.Add(Events.GetAdsOfferEvent, new GetAdsOfferEvent());
        }


        private void RegisterMessenger()
        {
            _incomingPackets.Add(Events.MessengerInitMessageEvent, new MessengerInitEvent());
            _incomingPackets.Add(Events.GetBuddyRequestsMessageEvent, new GetBuddyRequestsEvent());
            _incomingPackets.Add(Events.FollowFriendMessageEvent, new FollowFriendEvent());
            _incomingPackets.Add(Events.FindNewFriendsMessageEvent, new FindNewFriendsEvent());
            _incomingPackets.Add(Events.FriendListUpdateMessageEvent, new FriendListUpdateEvent());
            _incomingPackets.Add(Events.RemoveBuddyMessageEvent, new RemoveBuddyEvent());
            _incomingPackets.Add(Events.RequestBuddyMessageEvent, new RequestBuddyEvent());
            _incomingPackets.Add(Events.SendMsgMessageEvent, new SendMsgEvent());
            _incomingPackets.Add(Events.SendRoomInviteMessageEvent, new SendRoomInviteEvent());
            _incomingPackets.Add(Events.HabboSearchMessageEvent, new HabboSearchEvent());
            _incomingPackets.Add(Events.AcceptBuddyMessageEvent, new AcceptBuddyEvent());
            _incomingPackets.Add(Events.DeclineBuddyMessageEvent, new DeclineBuddyEvent());
        }

        public void RegisterAdventCalendar()
        {
            _incomingPackets.Add(Events.OpenCalendarBoxMessageEvent, new OpenCalendarBoxEvent());
        }

        private void RegisterGroups()
        {
            _incomingPackets.Add(Events.JoinGroupMessageEvent, new JoinGroupEvent());
            _incomingPackets.Add(Events.RemoveGroupFavouriteMessageEvent, new RemoveGroupFavouriteEvent());
            _incomingPackets.Add(Events.SetGroupFavouriteMessageEvent, new SetGroupFavouriteEvent());
            _incomingPackets.Add(Events.GetGroupInfoMessageEvent, new GetGroupInfoEvent());
            _incomingPackets.Add(Events.GetGroupMembersMessageEvent, new GetGroupMembersEvent());
            _incomingPackets.Add(Events.GetGroupCreationWindowMessageEvent, new GetGroupCreationWindowEvent());
            _incomingPackets.Add(Events.GetBadgeEditorPartsMessageEvent, new GetBadgeEditorPartsEvent());
            _incomingPackets.Add(Events.PurchaseGroupMessageEvent, new PurchaseGroupEvent());
            _incomingPackets.Add(Events.UpdateGroupIdentityMessageEvent, new UpdateGroupIdentityEvent());
            _incomingPackets.Add(Events.UpdateGroupBadgeMessageEvent, new UpdateGroupBadgeEvent());
            _incomingPackets.Add(Events.UpdateGroupColoursMessageEvent, new UpdateGroupColoursEvent());
            _incomingPackets.Add(Events.UpdateGroupSettingsMessageEvent, new UpdateGroupSettingsEvent());
            _incomingPackets.Add(Events.ManageGroupMessageEvent, new ManageGroupEvent());
            _incomingPackets.Add(Events.GiveAdminRightsMessageEvent, new GiveAdminRightsEvent());
            _incomingPackets.Add(Events.TakeAdminRightsMessageEvent, new TakeAdminRightsEvent());
            _incomingPackets.Add(Events.RemoveGroupMemberMessageEvent, new RemoveGroupMemberEvent());
            _incomingPackets.Add(Events.AcceptGroupMembershipMessageEvent, new AcceptGroupMembershipEvent());
            _incomingPackets.Add(Events.DeclineGroupMembershipMessageEvent, new DeclineGroupMembershipEvent());
            _incomingPackets.Add(Events.DeleteGroupMessageEvent, new DeleteGroupEvent());
        }

        private void RegisterForums()
        {
            _incomingPackets.Add(Events.GetForumsListDataMessageEvent, new GetForumsListDataEvent());
            _incomingPackets.Add(Events.GetForumStatsMessageEvent, new GetForumStatsEvent());
            _incomingPackets.Add(Events.GetThreadsListDataMessageEvent, new GetThreadsListDataEvent());
            _incomingPackets.Add(Events.GetThreadDataMessageEvent, new GetThreadDataEvent());
            _incomingPackets.Add(Events.PostGroupContentMessageEvent, new PostGroupContentEvent());
            _incomingPackets.Add(Events.DeleteGroupThreadMessageEvent, new DeleteGroupThreadEvent());
            _incomingPackets.Add(Events.UpdateForumSettingsMessageEvent, new UpdateForumSettingsEvent());
            _incomingPackets.Add(Events.UpdateThreadMessageEvent, new UpdateForumThreadStatusEvent());
            _incomingPackets.Add(Events.DeleteGroupPostMessageEvent, new DeleteGroupPostEvent());
            _incomingPackets.Add(Events.GetGroupForumsMessageEvent, new GetForumsListDataEvent());
            _incomingPackets.Add(Events.GetForumUserProfileMessageEvent, new GetForumUserProfileEvent());
        }

        private void RegisterRoomSettings()
        {
            _incomingPackets.Add(Events.GetRoomSettingsMessageEvent, new GetRoomSettingsEvent());
            _incomingPackets.Add(Events.SaveRoomSettingsMessageEvent, new SaveRoomSettingsEvent());
            _incomingPackets.Add(Events.DeleteRoomMessageEvent, new DeleteRoomEvent());
            _incomingPackets.Add(Events.ToggleMuteToolMessageEvent, new ToggleMuteToolEvent());
            _incomingPackets.Add(Events.GetRoomFilterListMessageEvent, new GetRoomFilterListEvent());
            _incomingPackets.Add(Events.ModifyRoomFilterListMessageEvent, new ModifyRoomFilterListEvent());
            _incomingPackets.Add(Events.GetRoomRightsMessageEvent, new GetRoomRightsEvent());
            _incomingPackets.Add(Events.GetRoomBannedUsersMessageEvent, new GetRoomBannedUsersEvent());
            _incomingPackets.Add(Events.UnbanUserFromRoomMessageEvent, new UnbanUserFromRoomEvent());
            _incomingPackets.Add(Events.SaveEnforcedCategorySettingsMessageEvent, new SaveEnforcedCategorySettingsEvent());
        }

        private void RegisterPets()
        {
            _incomingPackets.Add(Events.RespectPetMessageEvent, new RespectPetEvent());
            _incomingPackets.Add(Events.GetPetInformationMessageEvent, new GetPetInformationEvent());
            _incomingPackets.Add(Events.PickUpPetMessageEvent, new PickUpPetEvent());
            _incomingPackets.Add(Events.PlacePetMessageEvent, new PlacePetEvent());
            _incomingPackets.Add(Events.RideHorseMessageEvent, new RideHorseEvent());
            _incomingPackets.Add(Events.ApplyHorseEffectMessageEvent, new ApplyHorseEffectEvent());
            _incomingPackets.Add(Events.RemoveSaddleFromHorseMessageEvent, new RemoveSaddleFromHorseEvent());
            _incomingPackets.Add(Events.ModifyWhoCanRideHorseMessageEvent, new ModifyWhoCanRideHorseEvent());
            _incomingPackets.Add(Events.GetPetTrainingPanelMessageEvent, new GetPetTrainingPanelEvent());
            _incomingPackets.Add(Events.GetPetCommandsEvent, new GetPetCommandsEvent());
        }

        private void RegisterBots()
        {
            _incomingPackets.Add(Events.PlaceBotMessageEvent, new PlaceBotEvent());
            _incomingPackets.Add(Events.PickUpBotMessageEvent, new PickUpBotEvent());
            _incomingPackets.Add(Events.OpenBotActionMessageEvent, new OpenBotActionEvent());
            _incomingPackets.Add(Events.SaveBotActionMessageEvent, new SaveBotActionEvent());
        }

        private void RegisterFurni()
        {
            //this._incomingPackets.Add(ClientPacketHeader.GetHCCatalogGiftsEvent, new GetHCCatalogGiftsEvent());
            _incomingPackets.Add(Events.UpdateMagicTileMessageEvent, new UpdateMagicTileEvent());
            _incomingPackets.Add(Events.GetYouTubeTelevisionMessageEvent, new GetYouTubeTelevisionEvent());
            _incomingPackets.Add(Events.ToggleYouTubeVideoMessageEvent, new ToggleYouTubeVideoEvent());
            _incomingPackets.Add(Events.YouTubeVideoInformationMessageEvent, new YouTubeVideoInformationEvent());
            _incomingPackets.Add(Events.YouTubeGetNextVideo, new YouTubeGetNextVideo());
            _incomingPackets.Add(Events.SaveWiredTriggerConfigMessageEvent, new SaveWiredConfigEvent());
            _incomingPackets.Add(Events.SaveWiredEffectConfigMessageEvent, new SaveWiredConfigEvent());
            _incomingPackets.Add(Events.SaveWiredConditionConfigMessageEvent, new SaveWiredConfigEvent());
            _incomingPackets.Add(Events.SaveBrandingItemMessageEvent, new SaveBrandingItemEvent());
            _incomingPackets.Add(Events.SetTonerMessageEvent, new SetTonerEvent());
            _incomingPackets.Add(Events.DiceOffMessageEvent, new DiceOffEvent());
            _incomingPackets.Add(Events.ThrowDiceMessageEvent, new ThrowDiceEvent());
            _incomingPackets.Add(Events.SetMannequinNameMessageEvent, new SetMannequinNameEvent());
            _incomingPackets.Add(Events.SetMannequinFigureMessageEvent, new SetMannequinFigureEvent());
            _incomingPackets.Add(Events.CreditFurniRedeemMessageEvent, new CreditFurniRedeemEvent());
            _incomingPackets.Add(Events.GetStickyNoteMessageEvent, new GetStickyNoteEvent());
            _incomingPackets.Add(Events.AddStickyNoteMessageEvent, new AddStickyNoteEvent());
            _incomingPackets.Add(Events.UpdateStickyNoteMessageEvent, new UpdateStickyNoteEvent());
            _incomingPackets.Add(Events.DeleteStickyNoteMessageEvent, new DeleteStickyNoteEvent());
            _incomingPackets.Add(Events.GetMoodlightConfigMessageEvent, new GetMoodlightConfigEvent());
            _incomingPackets.Add(Events.MoodlightUpdateMessageEvent, new MoodlightUpdateEvent());
            _incomingPackets.Add(Events.ToggleMoodlightMessageEvent, new ToggleMoodlightEvent());
            _incomingPackets.Add(Events.UseOneWayGateMessageEvent, new UseFurnitureEvent());
            _incomingPackets.Add(Events.UseHabboWheelMessageEvent, new UseFurnitureEvent());
            _incomingPackets.Add(Events.OpenGiftMessageEvent, new OpenGiftEvent());
            _incomingPackets.Add(Events.GetGroupFurniSettingsMessageEvent, new GetGroupFurniSettingsEvent());
            _incomingPackets.Add(Events.UseSellableClothingMessageEvent, new UseSellableClothingEvent());
            _incomingPackets.Add(Events.ConfirmLoveLockMessageEvent, new ConfirmLoveLockEvent());
            _incomingPackets.Add(Events.FootballGateComponent, new FootballGateComponent());

            // CRAFT
            _incomingPackets.Add(Events.CraftSecretMessageEvent, new CraftSecretEvent());
            _incomingPackets.Add(Events.ExecuteCraftingRecipeMessageEvent, new ExecuteCraftingRecipeEvent());
            _incomingPackets.Add(Events.GetCraftingItemMessageEvent, new GetCraftingItemEvent());
            _incomingPackets.Add(Events.GetCraftingRecipesAvailableMessageEvent, new GetCraftingRecipesAvailableEvent());
            _incomingPackets.Add(Events.SetCraftingRecipeMessageEvent, new SetCraftingRecipeEvent());

            //RENTABLE
            _incomingPackets.Add(Events.BuyRentableSpaceEvent, new BuyRentableSpaceEvent());
            _incomingPackets.Add(Events.CancelRentableSpaceEvent, new CancelRentableSpaceEvent());
            _incomingPackets.Add(Events.GetRentableSpaceMessageEvent, new GetRentableSpaceEvent());
        }

        private void RegisterRoomPolls()
        {
            _incomingPackets.Add(Events.PollStartMessageEvent, new PollStartEvent());
            //this._incomingPackets.Add(ClientPacketHeader.PollAnswerMessageEvent, new PollAnswerEvent());
            _incomingPackets.Add(Events.PollRejectMessageEvent, new PollRejectEvent());
        }

        private void FloorPlanEditor()
        {
            _incomingPackets.Add(Events.SaveFloorPlanModelMessageEvent, new SaveFloorPlanModelEvent());
            _incomingPackets.Add(Events.InitializeFloorPlanSessionMessageEvent, new InitializeFloorPlanSessionEvent());
            _incomingPackets.Add(Events.FloorPlanEditorRoomPropertiesMessageEvent, new FloorPlanEditorRoomPropertiesEvent());
        }

        private void RegisterModeration()
        {
            _incomingPackets.Add(Events.OpenHelpToolMessageEvent, new OpenHelpToolEvent());
            _incomingPackets.Add(Events.GetModeratorRoomInfoMessageEvent, new GetModeratorRoomInfoEvent());
            _incomingPackets.Add(Events.GetModeratorUserInfoMessageEvent, new GetModeratorUserInfoEvent());
            _incomingPackets.Add(Events.GetModeratorUserRoomVisitsMessageEvent, new GetModeratorUserRoomVisitsEvent());
            _incomingPackets.Add(Events.ModerateRoomMessageEvent, new ModerateRoomEvent());
            _incomingPackets.Add(Events.ModeratorActionMessageEvent, new ModeratorActionEvent());
            _incomingPackets.Add(Events.SubmitNewTicketMessageEvent, new SubmitNewTicketEvent());
            _incomingPackets.Add(Events.GetModeratorRoomChatlogMessageEvent, new GetModeratorRoomChatlogEvent());
            _incomingPackets.Add(Events.GetModeratorUserChatlogMessageEvent, new GetModeratorUserChatlogEvent());
            _incomingPackets.Add(Events.GetModeratorTicketChatlogsMessageEvent, new GetModeratorTicketChatlogsEvent());
            _incomingPackets.Add(Events.PickTicketMessageEvent, new PickTicketEvent());
            _incomingPackets.Add(Events.ReleaseTicketMessageEvent, new ReleaseTicketEvent());
            _incomingPackets.Add(Events.CloseTicketMesageEvent, new CloseTicketEvent());
            _incomingPackets.Add(Events.ModerationMuteMessageEvent, new ModerationMuteEvent());
            _incomingPackets.Add(Events.ModerationKickMessageEvent, new ModerationKickEvent());
            _incomingPackets.Add(Events.ModerationBanMessageEvent, new ModerationBanEvent());
            _incomingPackets.Add(Events.ModerationMsgMessageEvent, new ModerationMsgEvent());
            _incomingPackets.Add(Events.ModerationCautionMessageEvent, new ModerationCautionEvent());
            _incomingPackets.Add(Events.ModerationTradeLockMessageEvent, new ModerationTradeLockEvent());
        }

        public void RegisterGameCenter()
        {
            _incomingPackets.Add(Events.GetGameListingMessageEvent, new GetGameListingEvent());
            _incomingPackets.Add(Events.InitializeGameCenterMessageEvent, new InitializeGameCenterEvent());
            _incomingPackets.Add(Events.GetPlayableGamesMessageEvent, new GetPlayableGamesEvent());
            _incomingPackets.Add(Events.JoinPlayerQueueMessageEvent, new JoinPlayerQueueEvent());
            _incomingPackets.Add(Events.Game2GetWeeklyLeaderboardMessageEvent, new Game2GetWeeklyLeaderboardEvent());
            _incomingPackets.Add(Events.GetGameCenterLeaderboardsEvent, new GetGameCenterLeaderboardsEvent());
            _incomingPackets.Add(Events.UnknownGameCenterEvent, new UnknownGameCenterEvent());
            _incomingPackets.Add(Events.UnknownGameCenterEvent2, new UnknownGameCenterEvent2());
            _incomingPackets.Add(Events.UnknownGameCenterEvent3, new UnknownGameCenterEvent3());
            _incomingPackets.Add(Events.UnknownGameCenterEvent4, new UnknownGameCenterEvent4());
            _incomingPackets.Add(Events.UnknownGameCenterEvent5, new UnknownGameCenterEvent5());
            _incomingPackets.Add(Events.GetWeeklyLeaderBoardEvent, new GetWeeklyLeaderBoardEvent());
        }



    }
}