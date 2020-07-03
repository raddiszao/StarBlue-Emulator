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
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms.Instance;
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

        private readonly ConcurrentDictionary<int, Task> _runningTasks;

        public PacketManager()
        {
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
        }

        public void TryExecutePacket(GameClient Session, ClientPacket Packet)
        {

            if (!_incomingPackets.TryGetValue(Packet.Id, out IPacketEvent OutPacket))
            {
                if (Convert.ToBoolean(StarBlueServer.GetConfig().data["debug"]))
                {
                    log.Debug("Unhandled Packet: " + Packet.ToString());
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

        private void ExecutePacketAsync(GameClient Session, ClientPacket Packet, IPacketEvent Pak)
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

                //log.Debug("Event took " + (DateTime.Now - Start).Milliseconds + "ms to complete.");
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

        private void RegisterHandshake()
        {
            _incomingPackets.Add(ClientPacketHeader.GetClientVersionMessageEvent, new GetClientVersionEvent());
            _incomingPackets.Add(ClientPacketHeader.InitCryptoMessageEvent, new InitCryptoEvent());
            _incomingPackets.Add(ClientPacketHeader.GenerateSecretKeyMessageEvent, new GenerateSecretKeyEvent());
            _incomingPackets.Add(ClientPacketHeader.UniqueIDMessageEvent, new UniqueIDEvent());
            _incomingPackets.Add(ClientPacketHeader.SSOTicketMessageEvent, new SSOTicketEvent());
            _incomingPackets.Add(ClientPacketHeader.InfoRetrieveMessageEvent, new InfoRetrieveEvent());
            _incomingPackets.Add(ClientPacketHeader.PingMessageEvent, new PingEvent());

        }

        private void RegisterLandingView()
        {
            _incomingPackets.Add(ClientPacketHeader.RefreshCampaignMessageEvent, new RefreshCampaignEvent());
            _incomingPackets.Add(ClientPacketHeader.GetPromoArticlesMessageEvent, new GetPromoArticlesEvent());
            _incomingPackets.Add(ClientPacketHeader.GiveConcurrentUsersRewardEvent, new GiveConcurrentUsersReward());
            _incomingPackets.Add(ClientPacketHeader.VoteCommunityGoalVS, new VoteCommunityGoalVS());
            _incomingPackets.Add(ClientPacketHeader.CommunityGoalEvent, new CommunityGoalEvent());
        }

        private void RegisterRoomCamera()
        {
            _incomingPackets.Add(ClientPacketHeader.SetRoomThumbnailMessageEvent, new SetRoomThumbnailMessageEvent());
            _incomingPackets.Add(ClientPacketHeader.HabboCameraPictureDataMessageEvent, new HabboCameraPictureDataMessageEvent());
            _incomingPackets.Add(ClientPacketHeader.PurchaseCameraPictureMessageEvent, new PurchaseCameraPictureMessageEvent());
            _incomingPackets.Add(ClientPacketHeader.PublishCameraPictureMessageEvent, new PublishCameraPictureMessageEvent());
            _incomingPackets.Add(ClientPacketHeader.ParticipatePictureCameraCompetitionMessageEvent, new ParticipatePictureCameraCompetitionMessageEvent());

        }

        private void RegisterNux()
        {

            _incomingPackets.Add(ClientPacketHeader.RoomNuxAlert, new RoomNuxAlert());
        }

        private void RegisterSMS()
        {
            _incomingPackets.Add(ClientPacketHeader.SmsVerification, new SMSVerification());
        }

        private void RegisterCatalog()
        {
            _incomingPackets.Add(ClientPacketHeader.GetCatalogModeMessageEvent, new GetCatalogModeEvent());
            _incomingPackets.Add(ClientPacketHeader.BuyTargettedOfferMessageEvent, new BuyTargettedOfferMessage());
            _incomingPackets.Add(ClientPacketHeader.GetCatalogIndexMessageEvent, new GetCatalogIndexEvent());
            _incomingPackets.Add(ClientPacketHeader.GetCatalogPageMessageEvent, new GetCatalogPageEvent());
            _incomingPackets.Add(ClientPacketHeader.GetCatalogOfferMessageEvent, new GetCatalogOfferEvent());
            _incomingPackets.Add(ClientPacketHeader.PurchaseFromCatalogMessageEvent, new PurchaseFromCatalogEvent());
            _incomingPackets.Add(ClientPacketHeader.PurchaseFromCatalogAsGiftMessageEvent, new PurchaseFromCatalogAsGiftEvent());
            _incomingPackets.Add(ClientPacketHeader.PurchaseRoomPromotionMessageEvent, new PurchaseRoomPromotionEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGiftWrappingConfigurationMessageEvent, new GetGiftWrappingConfigurationEvent());
            _incomingPackets.Add(ClientPacketHeader.GetMarketplaceConfigurationMessageEvent, new GetMarketplaceConfigurationEvent());
            _incomingPackets.Add(ClientPacketHeader.CheckPetNameMessageEvent, new CheckPetNameEvent());
            _incomingPackets.Add(ClientPacketHeader.RedeemVoucherMessageEvent, new RedeemVoucherEvent());
            _incomingPackets.Add(ClientPacketHeader.GetSellablePetBreedsMessageEvent, new GetSellablePetBreedsEvent());
            _incomingPackets.Add(ClientPacketHeader.GetPromotableRoomsMessageEvent, new GetPromotableRoomsEvent());
            _incomingPackets.Add(ClientPacketHeader.GetCatalogRoomPromotionMessageEvent, new GetCatalogRoomPromotionEvent());
            _incomingPackets.Add(ClientPacketHeader.GetNuxPresentEvent, new GetNuxPresentEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGroupFurniConfigMessageEvent, new GetGroupFurniConfigEvent());
            _incomingPackets.Add(ClientPacketHeader.CheckGnomeNameMessageEvent, new CheckGnomeNameEvent());
            //this._incomingPackets.Add(ClientPacketHeader.GetClubGiftsMessageEvent, new GetClubGiftsEvent());
            _incomingPackets.Add(ClientPacketHeader.LTDCountdownEvent, new LTDCountdownEvent());
            _incomingPackets.Add(ClientPacketHeader.RedeemHCGiftEvent, new RedeemHCGiftEvent());
            _incomingPackets.Add(ClientPacketHeader.FurniMaticPageEvent, new FurniMaticPageEvent());
            _incomingPackets.Add(ClientPacketHeader.FurniMaticRecycleEvent, new FurniMaticRecycleEvent());
            _incomingPackets.Add(ClientPacketHeader.FurniMaticRewardsEvent, new FurniMaticRewardsEvent());

        }

        private void RegisterMarketplace()
        {
            _incomingPackets.Add(ClientPacketHeader.GetOffersMessageEvent, new GetOffersEvent());
            _incomingPackets.Add(ClientPacketHeader.GetOwnOffersMessageEvent, new GetOwnOffersEvent());
            _incomingPackets.Add(ClientPacketHeader.GetMarketplaceCanMakeOfferMessageEvent, new GetMarketplaceCanMakeOfferEvent());
            _incomingPackets.Add(ClientPacketHeader.GetMarketplaceItemStatsMessageEvent, new GetMarketplaceItemStatsEvent());
            _incomingPackets.Add(ClientPacketHeader.MakeOfferMessageEvent, new MakeOfferEvent());
            _incomingPackets.Add(ClientPacketHeader.CancelOfferMessageEvent, new CancelOfferEvent());
            _incomingPackets.Add(ClientPacketHeader.BuyOfferMessageEvent, new BuyOfferEvent());
            _incomingPackets.Add(ClientPacketHeader.RedeemOfferCreditsMessageEvent, new RedeemOfferCreditsEvent());
        }

        private void RegisterNavigator()
        {
            _incomingPackets.Add(ClientPacketHeader.AddFavouriteRoomMessageEvent, new AddFavouriteRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.GetUserFlatCatsMessageEvent, new GetUserFlatCatsEvent());
            _incomingPackets.Add(ClientPacketHeader.DeleteFavouriteRoomMessageEvent, new RemoveFavouriteRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.GoToHotelViewMessageEvent, new GoToHotelViewEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateNavigatorSettingsMessageEvent, new UpdateNavigatorSettingsEvent());
            _incomingPackets.Add(ClientPacketHeader.CanCreateRoomMessageEvent, new CanCreateRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.CreateFlatMessageEvent, new CreateFlatEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGuestRoomMessageEvent, new GetGuestRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.EditRoomPromotionMessageEvent, new EditRoomEventEvent());
            _incomingPackets.Add(ClientPacketHeader.GetEventCategoriesMessageEvent, new GetNavigatorFlatsEvent());
            _incomingPackets.Add(ClientPacketHeader.StaffPickRoomEvent, new StaffPickRoomEvent());
        }

        public void RegisterNewNavigator()
        {
            _incomingPackets.Add(ClientPacketHeader.InitializeNewNavigatorMessageEvent, new InitializeNewNavigatorEvent());
            _incomingPackets.Add(ClientPacketHeader.NewNavigatorSearchMessageEvent, new NewNavigatorSearchEvent());
            _incomingPackets.Add(ClientPacketHeader.FindRandomFriendingRoomMessageEvent, new FindRandomFriendingRoomEvent());
        }

        private void RegisterQuests()
        {
            _incomingPackets.Add(ClientPacketHeader.GetQuestListMessageEvent, new GetQuestListEvent());
            _incomingPackets.Add(ClientPacketHeader.StartQuestMessageEvent, new StartQuestEvent());
            _incomingPackets.Add(ClientPacketHeader.CancelQuestMessageEvent, new CancelQuestEvent());
            _incomingPackets.Add(ClientPacketHeader.GetCurrentQuestMessageEvent, new GetCurrentQuestEvent());
            _incomingPackets.Add(ClientPacketHeader.GetDailyQuestMessageEvent, new GetDailyQuestEvent());
            //this._incomingPackets.Add(ClientPacketHeader.GetCommunityGoalHallOfFameMessageEvent, new GetCommunityGoalHallOfFameEvent());
        }

        private void RegisterHelp()
        {
            _incomingPackets.Add(ClientPacketHeader.OnBullyClickMessageEvent, new OnBullyClickEvent());
            _incomingPackets.Add(ClientPacketHeader.SendBullyReportMessageEvent, new SendBullyReportEvent());
            _incomingPackets.Add(ClientPacketHeader.SubmitBullyReportMessageEvent, new SubmitBullyReportEvent());
            _incomingPackets.Add(ClientPacketHeader.GetSanctionStatusMessageEvent, new GetSanctionStatusEvent());
        }

        private void RegisterRoomAction()
        {
            _incomingPackets.Add(ClientPacketHeader.LetUserInMessageEvent, new LetUserInEvent());
            _incomingPackets.Add(ClientPacketHeader.BanUserMessageEvent, new BanUserEvent());
            _incomingPackets.Add(ClientPacketHeader.KickUserMessageEvent, new KickUserEvent());
            _incomingPackets.Add(ClientPacketHeader.AssignRightsMessageEvent, new AssignRightsEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveRightsMessageEvent, new RemoveRightsEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveAllRightsMessageEvent, new RemoveAllRightsEvent());
            _incomingPackets.Add(ClientPacketHeader.MuteUserMessageEvent, new MuteUserEvent());
            _incomingPackets.Add(ClientPacketHeader.AmbassadorWarningMessageEvent, new AmbassadorWarningMessageEvent());
            _incomingPackets.Add(ClientPacketHeader.GiveHandItemMessageEvent, new GiveHandItemEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveMyRightsMessageEvent, new RemoveMyRightsEvent());
        }

        private void RegisterHelperTool()
        {
            _incomingPackets.Add(ClientPacketHeader.HandleHelperToolMessageEvent, new HandleHelperToolEvent());
            _incomingPackets.Add(ClientPacketHeader.CallForHelperMessageEvent, new CallForHelperEvent());
            _incomingPackets.Add(ClientPacketHeader.AcceptHelperSessionMessageEvent, new AcceptHelperSessionEvent());
            _incomingPackets.Add(ClientPacketHeader.CancelCallForHelperMessageEvent, new CancelCallForHelperEvent());
            _incomingPackets.Add(ClientPacketHeader.FinishHelperSessionMessageEvent, new FinishHelperSessionEvent());
            _incomingPackets.Add(ClientPacketHeader.CloseHelperChatSessionMessageEvent, new CloseHelperChatSessionEvent());
            _incomingPackets.Add(ClientPacketHeader.HelperSessioChatTypingMessageEvent, new HelperSessionChatTypingEvent());
            _incomingPackets.Add(ClientPacketHeader.HelperSessioChatSendMessageMessageEvent, new HelperSessionChatSendMessageEvent());
            _incomingPackets.Add(ClientPacketHeader.InvinteHelperUserSessionMessageEvent, new InvinteHelperUserSessionEvent());
            _incomingPackets.Add(ClientPacketHeader.VisitHelperUserSessionMessageEvent, new VisitHelperUserSessionEvent());
            _incomingPackets.Add(ClientPacketHeader.ReportBullyUserMessageEvent, new ReportBullyUserEvent());
            _incomingPackets.Add(ClientPacketHeader.AcceptJoinJudgeChatMessageEvent, new AcceptJoinJudgeChatEvent());
        }

        private void RegisterAvatar()
        {
            _incomingPackets.Add(ClientPacketHeader.GetWardrobeMessageEvent, new GetWardrobeEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveWardrobeOutfitMessageEvent, new SaveWardrobeOutfitEvent());
        }

        private void RegisterRoomAvatar()
        {
            _incomingPackets.Add(ClientPacketHeader.ActionMessageEvent, new ActionEvent());
            _incomingPackets.Add(ClientPacketHeader.ApplySignMessageEvent, new ApplySignEvent());
            _incomingPackets.Add(ClientPacketHeader.DanceMessageEvent, new DanceEvent());
            _incomingPackets.Add(ClientPacketHeader.SitMessageEvent, new SitEvent());
            _incomingPackets.Add(ClientPacketHeader.ChangeMottoMessageEvent, new ChangeMottoEvent());
            _incomingPackets.Add(ClientPacketHeader.LookToMessageEvent, new LookToEvent());
            _incomingPackets.Add(ClientPacketHeader.DropHandItemMessageEvent, new DropHandItemEvent());
            _incomingPackets.Add(ClientPacketHeader.GiveRoomScoreMessageEvent, new GiveRoomScoreEvent());
            _incomingPackets.Add(ClientPacketHeader.IgnoreUserMessageEvent, new IgnoreUserEvent());
            _incomingPackets.Add(ClientPacketHeader.UnIgnoreUserMessageEvent, new UnIgnoreUserEvent());
        }

        private void RegisterRoomConnection()
        {
            _incomingPackets.Add(ClientPacketHeader.OpenFlatConnectionMessageEvent, new OpenFlatConnectionEvent());
            _incomingPackets.Add(ClientPacketHeader.GoToFlatMessageEvent, new GoToFlatEvent());
        }

        private void RegisterRoomChat()
        {
            _incomingPackets.Add(ClientPacketHeader.ChatMessageEvent, new ChatEvent());
            _incomingPackets.Add(ClientPacketHeader.ShoutMessageEvent, new ShoutEvent());
            _incomingPackets.Add(ClientPacketHeader.WhisperMessageEvent, new WhisperEvent());
            _incomingPackets.Add(ClientPacketHeader.StartTypingMessageEvent, new StartTypingEvent());
            _incomingPackets.Add(ClientPacketHeader.CancelTypingMessageEvent, new CancelTypingEvent());
        }

        private void RegisterRoomEngine()
        {
            _incomingPackets.Add(ClientPacketHeader.GetRoomEntryDataMessageEvent, new GetRoomEntryDataEvent());
            _incomingPackets.Add(ClientPacketHeader.GoToFlatAsSpectatorEvent, new GoToFlatAsSpectatorEvent());
            _incomingPackets.Add(ClientPacketHeader.GetFurnitureAliasesMessageEvent, new GetFurnitureAliasesEvent());
            _incomingPackets.Add(ClientPacketHeader.MoveAvatarMessageEvent, new MoveAvatarEvent());
            _incomingPackets.Add(ClientPacketHeader.MoveObjectMessageEvent, new MoveObjectEvent());
            _incomingPackets.Add(ClientPacketHeader.PickupObjectMessageEvent, new PickupObjectEvent());
            _incomingPackets.Add(ClientPacketHeader.MoveWallItemMessageEvent, new MoveWallItemEvent());
            _incomingPackets.Add(ClientPacketHeader.ApplyDecorationMessageEvent, new ApplyDecorationEvent());
            _incomingPackets.Add(ClientPacketHeader.PlaceObjectMessageEvent, new PlaceObjectEvent());
            _incomingPackets.Add(ClientPacketHeader.UseFurnitureMessageEvent, new UseFurnitureEvent());
            _incomingPackets.Add(ClientPacketHeader.UseWallItemMessageEvent, new UseWallItemEvent());
            _incomingPackets.Add(ClientPacketHeader.PlaceBuilderItemMessageEvent, new PlaceBuilderObjectEvent());
            _incomingPackets.Add(ClientPacketHeader.EventTrackerMessageEvent, new EventTrackerEvent());
        }

        private void RegisterInventory()
        {
            _incomingPackets.Add(ClientPacketHeader.InitTradeMessageEvent, new InitTradeEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingOfferItemMessageEvent, new TradingOfferItemEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingOfferItemsMessageEvent, new TradingOfferItemsEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingRemoveItemMessageEvent, new TradingRemoveItemEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingAcceptMessageEvent, new TradingAcceptEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingCancelMessageEvent, new TradingCancelEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingConfirmMessageEvent, new TradingConfirmEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingModifyMessageEvent, new TradingModifyEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingCancelConfirmMessageEvent, new TradingCancelConfirmEvent());
            _incomingPackets.Add(ClientPacketHeader.RequestFurniInventoryMessageEvent, new RequestFurniInventoryEvent());
            _incomingPackets.Add(ClientPacketHeader.GetBadgesMessageEvent, new GetBadgesEvent());
            _incomingPackets.Add(ClientPacketHeader.GetAchievementsMessageEvent, new GetAchievementsEvent());
            _incomingPackets.Add(ClientPacketHeader.SetActivatedBadgesMessageEvent, new SetActivatedBadgesEvent());
            _incomingPackets.Add(ClientPacketHeader.GetBotInventoryMessageEvent, new GetBotInventoryEvent());
            _incomingPackets.Add(ClientPacketHeader.GetPetInventoryMessageEvent, new GetPetInventoryEvent());
            _incomingPackets.Add(ClientPacketHeader.AvatarEffectActivatedMessageEvent, new AvatarEffectActivatedEvent());
            _incomingPackets.Add(ClientPacketHeader.AvatarEffectSelectedMessageEvent, new AvatarEffectSelectedEvent());
        }

        private void RegisterTalents()
        {
            _incomingPackets.Add(ClientPacketHeader.GetTalentTrackMessageEvent, new GetTalentTrackEvent());
            _incomingPackets.Add(ClientPacketHeader.CheckQuizTypeEvent, new CheckQuizType());
            _incomingPackets.Add(ClientPacketHeader.PostQuizAnswersMessageEvent, new PostQuizAnswersMessage());

        }

        private void RegisterPolls()
        {
            _incomingPackets.Add(ClientPacketHeader.SubmitPollAnswerMessageEvent, new SubmitPollAnswerMessageEvent());

        }

        private void RegisterPurse()
        {
            _incomingPackets.Add(ClientPacketHeader.GetCreditsInfoMessageEvent, new GetCreditsInfoEvent());
            _incomingPackets.Add(ClientPacketHeader.GetHabboClubWindowMessageEvent, new GetHabboClubWindowEvent());
            _incomingPackets.Add(ClientPacketHeader.GetHabboClubCenterInfoMessageEvent, new GetHabboClubCenterInfoMessageEvent());
            _incomingPackets.Add(ClientPacketHeader.GetCameraPriceEvent, new GetCameraPriceEvent());
        }

        private void RegisterUsers()
        {
            _incomingPackets.Add(ClientPacketHeader.ScrGetUserInfoMessageEvent, new ScrGetUserInfoEvent());
            _incomingPackets.Add(ClientPacketHeader.SetChatPreferenceMessageEvent, new SetChatPreferenceEvent());
            _incomingPackets.Add(ClientPacketHeader.SetUserFocusPreferenceEvent, new SetUserFocusPreferenceEvent());
            _incomingPackets.Add(ClientPacketHeader.SetMessengerInviteStatusMessageEvent, new SetMessengerInviteStatusEvent());
            _incomingPackets.Add(ClientPacketHeader.RespectUserMessageEvent, new RespectUserEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateFigureDataMessageEvent, new UpdateFigureDataEvent());
            _incomingPackets.Add(ClientPacketHeader.OpenPlayerProfileMessageEvent, new OpenPlayerProfileEvent());
            _incomingPackets.Add(ClientPacketHeader.GetSelectedBadgesMessageEvent, new GetSelectedBadgesEvent());
            _incomingPackets.Add(ClientPacketHeader.GetRelationshipsMessageEvent, new GetRelationshipsEvent());
            _incomingPackets.Add(ClientPacketHeader.SetRelationshipMessageEvent, new SetRelationshipEvent());
            _incomingPackets.Add(ClientPacketHeader.CheckValidNameMessageEvent, new CheckValidNameEvent());
            _incomingPackets.Add(ClientPacketHeader.ChangeNameMessageEvent, new ChangeNameEvent());
            _incomingPackets.Add(ClientPacketHeader.SetUsernameMessageEvent, new SetUsernameEvent());
            _incomingPackets.Add(ClientPacketHeader.GetHabboGroupBadgesMessageEvent, new GetHabboGroupBadgesEvent());
            _incomingPackets.Add(ClientPacketHeader.GetUserTagsMessageEvent, new GetUserTagsEvent());
            //_incomingPackets.Add(ClientPacketHeader.GetIgnoredUsersMessageEvent, new GetIgnoredUsersEvent());
        }

        private void RegisterSound()
        {
            _incomingPackets.Add(ClientPacketHeader.SetSoundSettingsMessageEvent, new SetSoundSettingsEvent());
            _incomingPackets.Add(ClientPacketHeader.GetSongInfoMessageEvent, new GetSongInfoEvent());
            _incomingPackets.Add(ClientPacketHeader.GetJukeboxPlaylistMessageEvent, new GetJukeboxPlayListEvent());
            _incomingPackets.Add(ClientPacketHeader.LoadJukeboxDiscsMessageEvent, new LoadJukeboxDiscsEvent());
            _incomingPackets.Add(ClientPacketHeader.GetJukeboxDiscsDataMessageEvent, new GetJukeboxDiscsDataEvent());
            _incomingPackets.Add(ClientPacketHeader.AddDiscToPlayListMessageEvent, new AddDiscToPlayListEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveDiscFromPlayListMessageEvent, new RemoveDiscFromPlayListEvent());
        }


        private void RegisterMisc()
        {
            _incomingPackets.Add(ClientPacketHeader.UnknownQuestMessageEvent, new GetQuestListEvent());
            _incomingPackets.Add(ClientPacketHeader.ClientVariablesMessageEvent, new ClientVariablesEvent());
            _incomingPackets.Add(ClientPacketHeader.DisconnectionMessageEvent, new DisconnectEvent());
            _incomingPackets.Add(ClientPacketHeader.LatencyTestMessageEvent, new LatencyTestEvent());
            _incomingPackets.Add(ClientPacketHeader.MemoryPerformanceMessageEvent, new MemoryPerformanceEvent());
            _incomingPackets.Add(ClientPacketHeader.SetFriendBarStateMessageEvent, new SetFriendBarStateEvent());
            _incomingPackets.Add(ClientPacketHeader.GetAdsOfferEvent, new GetAdsOfferEvent());
        }


        private void RegisterMessenger()
        {
            _incomingPackets.Add(ClientPacketHeader.MessengerInitMessageEvent, new MessengerInitEvent());
            _incomingPackets.Add(ClientPacketHeader.GetBuddyRequestsMessageEvent, new GetBuddyRequestsEvent());
            _incomingPackets.Add(ClientPacketHeader.FollowFriendMessageEvent, new FollowFriendEvent());
            _incomingPackets.Add(ClientPacketHeader.FindNewFriendsMessageEvent, new FindNewFriendsEvent());
            _incomingPackets.Add(ClientPacketHeader.FriendListUpdateMessageEvent, new FriendListUpdateEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveBuddyMessageEvent, new RemoveBuddyEvent());
            _incomingPackets.Add(ClientPacketHeader.RequestBuddyMessageEvent, new RequestBuddyEvent());
            _incomingPackets.Add(ClientPacketHeader.SendMsgMessageEvent, new SendMsgEvent());
            _incomingPackets.Add(ClientPacketHeader.SendRoomInviteMessageEvent, new SendRoomInviteEvent());
            _incomingPackets.Add(ClientPacketHeader.HabboSearchMessageEvent, new HabboSearchEvent());
            _incomingPackets.Add(ClientPacketHeader.AcceptBuddyMessageEvent, new AcceptBuddyEvent());
            _incomingPackets.Add(ClientPacketHeader.DeclineBuddyMessageEvent, new DeclineBuddyEvent());
        }

        public void RegisterAdventCalendar()
        {
            _incomingPackets.Add(ClientPacketHeader.OpenCalendarBoxMessageEvent, new OpenCalendarBoxEvent());
        }

        private void RegisterGroups()
        {
            _incomingPackets.Add(ClientPacketHeader.JoinGroupMessageEvent, new JoinGroupEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveGroupFavouriteMessageEvent, new RemoveGroupFavouriteEvent());
            _incomingPackets.Add(ClientPacketHeader.SetGroupFavouriteMessageEvent, new SetGroupFavouriteEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGroupInfoMessageEvent, new GetGroupInfoEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGroupMembersMessageEvent, new GetGroupMembersEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGroupCreationWindowMessageEvent, new GetGroupCreationWindowEvent());
            _incomingPackets.Add(ClientPacketHeader.GetBadgeEditorPartsMessageEvent, new GetBadgeEditorPartsEvent());
            _incomingPackets.Add(ClientPacketHeader.PurchaseGroupMessageEvent, new PurchaseGroupEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateGroupIdentityMessageEvent, new UpdateGroupIdentityEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateGroupBadgeMessageEvent, new UpdateGroupBadgeEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateGroupColoursMessageEvent, new UpdateGroupColoursEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateGroupSettingsMessageEvent, new UpdateGroupSettingsEvent());
            _incomingPackets.Add(ClientPacketHeader.ManageGroupMessageEvent, new ManageGroupEvent());
            _incomingPackets.Add(ClientPacketHeader.GiveAdminRightsMessageEvent, new GiveAdminRightsEvent());
            _incomingPackets.Add(ClientPacketHeader.TakeAdminRightsMessageEvent, new TakeAdminRightsEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveGroupMemberMessageEvent, new RemoveGroupMemberEvent());
            _incomingPackets.Add(ClientPacketHeader.AcceptGroupMembershipMessageEvent, new AcceptGroupMembershipEvent());
            _incomingPackets.Add(ClientPacketHeader.DeclineGroupMembershipMessageEvent, new DeclineGroupMembershipEvent());
            _incomingPackets.Add(ClientPacketHeader.DeleteGroupMessageEvent, new DeleteGroupEvent());
        }

        private void RegisterForums()
        {
            _incomingPackets.Add(ClientPacketHeader.GetForumsListDataMessageEvent, new GetForumsListDataEvent());
            _incomingPackets.Add(ClientPacketHeader.GetForumStatsMessageEvent, new GetForumStatsEvent());
            _incomingPackets.Add(ClientPacketHeader.GetThreadsListDataMessageEvent, new GetThreadsListDataEvent());
            _incomingPackets.Add(ClientPacketHeader.GetThreadDataMessageEvent, new GetThreadDataEvent());
            _incomingPackets.Add(ClientPacketHeader.PostGroupContentMessageEvent, new PostGroupContentEvent());
            _incomingPackets.Add(ClientPacketHeader.DeleteGroupThreadMessageEvent, new DeleteGroupThreadEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateForumSettingsMessageEvent, new UpdateForumSettingsEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateThreadMessageEvent, new UpdateForumThreadStatusEvent());
            _incomingPackets.Add(ClientPacketHeader.DeleteGroupPostMessageEvent, new DeleteGroupPostEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGroupForumsMessageEvent, new GetForumsListDataEvent());
            _incomingPackets.Add(ClientPacketHeader.GetForumUserProfileMessageEvent, new GetForumUserProfileEvent());
        }

        private void RegisterRoomSettings()
        {
            _incomingPackets.Add(ClientPacketHeader.GetRoomSettingsMessageEvent, new GetRoomSettingsEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveRoomSettingsMessageEvent, new SaveRoomSettingsEvent());
            _incomingPackets.Add(ClientPacketHeader.DeleteRoomMessageEvent, new DeleteRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.ToggleMuteToolMessageEvent, new ToggleMuteToolEvent());
            _incomingPackets.Add(ClientPacketHeader.GetRoomFilterListMessageEvent, new GetRoomFilterListEvent());
            _incomingPackets.Add(ClientPacketHeader.ModifyRoomFilterListMessageEvent, new ModifyRoomFilterListEvent());
            _incomingPackets.Add(ClientPacketHeader.GetRoomRightsMessageEvent, new GetRoomRightsEvent());
            _incomingPackets.Add(ClientPacketHeader.GetRoomBannedUsersMessageEvent, new GetRoomBannedUsersEvent());
            _incomingPackets.Add(ClientPacketHeader.UnbanUserFromRoomMessageEvent, new UnbanUserFromRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveEnforcedCategorySettingsMessageEvent, new SaveEnforcedCategorySettingsEvent());
        }

        private void RegisterPets()
        {
            _incomingPackets.Add(ClientPacketHeader.RespectPetMessageEvent, new RespectPetEvent());
            _incomingPackets.Add(ClientPacketHeader.GetPetInformationMessageEvent, new GetPetInformationEvent());
            _incomingPackets.Add(ClientPacketHeader.PickUpPetMessageEvent, new PickUpPetEvent());
            _incomingPackets.Add(ClientPacketHeader.PlacePetMessageEvent, new PlacePetEvent());
            _incomingPackets.Add(ClientPacketHeader.RideHorseMessageEvent, new RideHorseEvent());
            _incomingPackets.Add(ClientPacketHeader.ApplyHorseEffectMessageEvent, new ApplyHorseEffectEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveSaddleFromHorseMessageEvent, new RemoveSaddleFromHorseEvent());
            _incomingPackets.Add(ClientPacketHeader.ModifyWhoCanRideHorseMessageEvent, new ModifyWhoCanRideHorseEvent());
            _incomingPackets.Add(ClientPacketHeader.GetPetTrainingPanelMessageEvent, new GetPetTrainingPanelEvent());
            _incomingPackets.Add(ClientPacketHeader.GetPetCommandsEvent, new GetPetCommandsEvent());
        }

        private void RegisterBots()
        {
            _incomingPackets.Add(ClientPacketHeader.PlaceBotMessageEvent, new PlaceBotEvent());
            _incomingPackets.Add(ClientPacketHeader.PickUpBotMessageEvent, new PickUpBotEvent());
            _incomingPackets.Add(ClientPacketHeader.OpenBotActionMessageEvent, new OpenBotActionEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveBotActionMessageEvent, new SaveBotActionEvent());
        }

        private void RegisterFurni()
        {
            //this._incomingPackets.Add(ClientPacketHeader.GetHCCatalogGiftsEvent, new GetHCCatalogGiftsEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateMagicTileMessageEvent, new UpdateMagicTileEvent());
            _incomingPackets.Add(ClientPacketHeader.GetYouTubeTelevisionMessageEvent, new GetYouTubeTelevisionEvent());
            _incomingPackets.Add(ClientPacketHeader.ToggleYouTubeVideoMessageEvent, new ToggleYouTubeVideoEvent());
            _incomingPackets.Add(ClientPacketHeader.YouTubeVideoInformationMessageEvent, new YouTubeVideoInformationEvent());
            _incomingPackets.Add(ClientPacketHeader.YouTubeGetNextVideo, new YouTubeGetNextVideo());
            _incomingPackets.Add(ClientPacketHeader.SaveWiredTriggerConfigMessageEvent, new SaveWiredConfigEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveWiredEffectConfigMessageEvent, new SaveWiredConfigEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveWiredConditionConfigMessageEvent, new SaveWiredConfigEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveBrandingItemMessageEvent, new SaveBrandingItemEvent());
            _incomingPackets.Add(ClientPacketHeader.SetTonerMessageEvent, new SetTonerEvent());
            _incomingPackets.Add(ClientPacketHeader.DiceOffMessageEvent, new DiceOffEvent());
            _incomingPackets.Add(ClientPacketHeader.ThrowDiceMessageEvent, new ThrowDiceEvent());
            _incomingPackets.Add(ClientPacketHeader.SetMannequinNameMessageEvent, new SetMannequinNameEvent());
            _incomingPackets.Add(ClientPacketHeader.SetMannequinFigureMessageEvent, new SetMannequinFigureEvent());
            _incomingPackets.Add(ClientPacketHeader.CreditFurniRedeemMessageEvent, new CreditFurniRedeemEvent());
            _incomingPackets.Add(ClientPacketHeader.GetStickyNoteMessageEvent, new GetStickyNoteEvent());
            _incomingPackets.Add(ClientPacketHeader.AddStickyNoteMessageEvent, new AddStickyNoteEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateStickyNoteMessageEvent, new UpdateStickyNoteEvent());
            _incomingPackets.Add(ClientPacketHeader.DeleteStickyNoteMessageEvent, new DeleteStickyNoteEvent());
            _incomingPackets.Add(ClientPacketHeader.GetMoodlightConfigMessageEvent, new GetMoodlightConfigEvent());
            _incomingPackets.Add(ClientPacketHeader.MoodlightUpdateMessageEvent, new MoodlightUpdateEvent());
            _incomingPackets.Add(ClientPacketHeader.ToggleMoodlightMessageEvent, new ToggleMoodlightEvent());
            _incomingPackets.Add(ClientPacketHeader.UseOneWayGateMessageEvent, new UseFurnitureEvent());
            _incomingPackets.Add(ClientPacketHeader.UseHabboWheelMessageEvent, new UseFurnitureEvent());
            _incomingPackets.Add(ClientPacketHeader.OpenGiftMessageEvent, new OpenGiftEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGroupFurniSettingsMessageEvent, new GetGroupFurniSettingsEvent());
            _incomingPackets.Add(ClientPacketHeader.UseSellableClothingMessageEvent, new UseSellableClothingEvent());
            _incomingPackets.Add(ClientPacketHeader.ConfirmLoveLockMessageEvent, new ConfirmLoveLockEvent());
            _incomingPackets.Add(ClientPacketHeader.FootballGateComponent, new FootballGateComponent());

            // CRAFT
            _incomingPackets.Add(ClientPacketHeader.CraftSecretMessageEvent, new CraftSecretEvent());
            _incomingPackets.Add(ClientPacketHeader.ExecuteCraftingRecipeMessageEvent, new ExecuteCraftingRecipeEvent());
            _incomingPackets.Add(ClientPacketHeader.GetCraftingItemMessageEvent, new GetCraftingItemEvent());
            _incomingPackets.Add(ClientPacketHeader.GetCraftingRecipesAvailableMessageEvent, new GetCraftingRecipesAvailableEvent());
            _incomingPackets.Add(ClientPacketHeader.SetCraftingRecipeMessageEvent, new SetCraftingRecipeEvent());

            //RENTABLE
            _incomingPackets.Add(ClientPacketHeader.BuyRentableSpaceEvent, new BuyRentableSpaceEvent());
            _incomingPackets.Add(ClientPacketHeader.CancelRentableSpaceEvent, new CancelRentableSpaceEvent());
            _incomingPackets.Add(ClientPacketHeader.GetRentableSpaceMessageEvent, new GetRentableSpaceEvent());
        }

        private void RegisterRoomPolls()
        {
            _incomingPackets.Add(ClientPacketHeader.PollStartMessageEvent, new PollStartEvent());
            //this._incomingPackets.Add(ClientPacketHeader.PollAnswerMessageEvent, new PollAnswerEvent());
            _incomingPackets.Add(ClientPacketHeader.PollRejectMessageEvent, new PollRejectEvent());
        }

        private void FloorPlanEditor()
        {
            _incomingPackets.Add(ClientPacketHeader.SaveFloorPlanModelMessageEvent, new SaveFloorPlanModelEvent());
            _incomingPackets.Add(ClientPacketHeader.InitializeFloorPlanSessionMessageEvent, new InitializeFloorPlanSessionEvent());
            _incomingPackets.Add(ClientPacketHeader.FloorPlanEditorRoomPropertiesMessageEvent, new FloorPlanEditorRoomPropertiesEvent());
        }

        private void RegisterModeration()
        {
            _incomingPackets.Add(ClientPacketHeader.OpenHelpToolMessageEvent, new OpenHelpToolEvent());
            _incomingPackets.Add(ClientPacketHeader.GetModeratorRoomInfoMessageEvent, new GetModeratorRoomInfoEvent());
            _incomingPackets.Add(ClientPacketHeader.GetModeratorUserInfoMessageEvent, new GetModeratorUserInfoEvent());
            _incomingPackets.Add(ClientPacketHeader.GetModeratorUserRoomVisitsMessageEvent, new GetModeratorUserRoomVisitsEvent());
            _incomingPackets.Add(ClientPacketHeader.ModerateRoomMessageEvent, new ModerateRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.ModeratorActionMessageEvent, new ModeratorActionEvent());
            _incomingPackets.Add(ClientPacketHeader.SubmitNewTicketMessageEvent, new SubmitNewTicketEvent());
            _incomingPackets.Add(ClientPacketHeader.GetModeratorRoomChatlogMessageEvent, new GetModeratorRoomChatlogEvent());
            _incomingPackets.Add(ClientPacketHeader.GetModeratorUserChatlogMessageEvent, new GetModeratorUserChatlogEvent());
            _incomingPackets.Add(ClientPacketHeader.GetModeratorTicketChatlogsMessageEvent, new GetModeratorTicketChatlogsEvent());
            _incomingPackets.Add(ClientPacketHeader.PickTicketMessageEvent, new PickTicketEvent());
            _incomingPackets.Add(ClientPacketHeader.ReleaseTicketMessageEvent, new ReleaseTicketEvent());
            _incomingPackets.Add(ClientPacketHeader.CloseTicketMesageEvent, new CloseTicketEvent());
            _incomingPackets.Add(ClientPacketHeader.ModerationMuteMessageEvent, new ModerationMuteEvent());
            _incomingPackets.Add(ClientPacketHeader.ModerationKickMessageEvent, new ModerationKickEvent());
            _incomingPackets.Add(ClientPacketHeader.ModerationBanMessageEvent, new ModerationBanEvent());
            _incomingPackets.Add(ClientPacketHeader.ModerationMsgMessageEvent, new ModerationMsgEvent());
            _incomingPackets.Add(ClientPacketHeader.ModerationCautionMessageEvent, new ModerationCautionEvent());
            _incomingPackets.Add(ClientPacketHeader.ModerationTradeLockMessageEvent, new ModerationTradeLockEvent());
        }

        public void RegisterGameCenter()
        {
            _incomingPackets.Add(ClientPacketHeader.GetGameListingMessageEvent, new GetGameListingEvent());
            _incomingPackets.Add(ClientPacketHeader.InitializeGameCenterMessageEvent, new InitializeGameCenterEvent());
            _incomingPackets.Add(ClientPacketHeader.GetPlayableGamesMessageEvent, new GetPlayableGamesEvent());
            _incomingPackets.Add(ClientPacketHeader.JoinPlayerQueueMessageEvent, new JoinPlayerQueueEvent());
            _incomingPackets.Add(ClientPacketHeader.Game2GetWeeklyLeaderboardMessageEvent, new Game2GetWeeklyLeaderboardEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGameCenterLeaderboardsEvent, new GetGameCenterLeaderboardsEvent());
            _incomingPackets.Add(ClientPacketHeader.UnknownGameCenterEvent, new UnknownGameCenterEvent());
            _incomingPackets.Add(ClientPacketHeader.UnknownGameCenterEvent2, new UnknownGameCenterEvent2());
            _incomingPackets.Add(ClientPacketHeader.UnknownGameCenterEvent3, new UnknownGameCenterEvent3());
            _incomingPackets.Add(ClientPacketHeader.UnknownGameCenterEvent4, new UnknownGameCenterEvent4());
            _incomingPackets.Add(ClientPacketHeader.UnknownGameCenterEvent5, new UnknownGameCenterEvent5());
            _incomingPackets.Add(ClientPacketHeader.GetWeeklyLeaderBoardEvent, new GetWeeklyLeaderBoardEvent());
        }



    }
}