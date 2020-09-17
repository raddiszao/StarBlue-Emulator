using DotNetty.Transport.Channels;
using StarBlue.Communication.Packets.Outgoing;
using StarBlue.Communication.Packets.Outgoing.BuildersClub;
using StarBlue.Communication.Packets.Outgoing.Handshake;
using StarBlue.Communication.Packets.Outgoing.Help.Helpers;
using StarBlue.Communication.Packets.Outgoing.Inventory.Achievements;
using StarBlue.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Communication.Packets.Outgoing.Navigator;
using StarBlue.Communication.Packets.Outgoing.Rooms.Camera;
using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.Communication.Packets.Outgoing.Rooms.Furni;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Communication.Packets.Outgoing.Rooms.Nux;
using StarBlue.Communication.Packets.Outgoing.Sound;
using StarBlue.Communication.Packets.Outgoing.Users;
using StarBlue.Core;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Catalog;
using StarBlue.HabboHotel.Helpers;
using StarBlue.HabboHotel.Moderation;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Subscriptions;
using StarBlue.HabboHotel.Users;
using StarBlue.HabboHotel.Users.Messenger.FriendBar;
using StarBlue.HabboHotel.Users.UserDataManagement;
using StarBlue.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using static StarBlue.Core.Rank.RankManager;

namespace StarBlue.HabboHotel.GameClients
{
    public class GameClient
    {
        private readonly int _id;
        private Habbo _habbo;
        public string MachineId;
        private bool _disconnected;
        public string ssoTicket;
        private IChannelHandlerContext channel;
        public int PingCount { get; set; }

        public GameClient(IChannelHandlerContext context)
        {
            channel = context;
        }

        public bool TryAuthenticate(string AuthTicket)
        {
            try
            {
                UserData userData = UserDataFactory.GetUserData(AuthTicket, out byte errorCode);
                if (errorCode == 1 || errorCode == 2)
                {
                    Disconnect();
                    return false;
                }

                #region Ban Checking
                //Let's have a quick search for a ban before we successfully authenticate..
                ModerationBan BanRecord = null;
                if (!string.IsNullOrEmpty(MachineId))
                {
                    if (StarBlueServer.GetGame().GetModerationManager().IsBanned(MachineId, out BanRecord))
                    {
                        if (StarBlueServer.GetGame().GetModerationManager().MachineBanCheck(MachineId))
                        {
                            Disconnect();
                            return false;
                        }
                    }
                }

                if (userData.user != null)
                {
                    //Now let us check for a username ban record..
                    BanRecord = null;
                    if (StarBlueServer.GetGame().GetModerationManager().IsBanned(userData.user.Username, out BanRecord))
                    {
                        if (StarBlueServer.GetGame().GetModerationManager().UsernameBanCheck(userData.user.Username))
                        {
                            Disconnect();
                            return false;
                        }
                    }
                }
                #endregion

                if (userData.user == null) //Possible NPE
                {
                    return false;
                }

                StarBlueServer.GetGame().GetClientManager().RegisterClient(this, userData.userID, userData.user.Username);
                _habbo = userData.user;

                if (_habbo != null)
                {
                    ssoTicket = AuthTicket;
                    userData.user.Init(this, userData);

                    SendQueue(new AuthenticationOKComposer());
                    SendQueue(new AvatarEffectsComposer(_habbo.Effects().GetAllEffects));
                    SendQueue(new NavigatorSettingsComposer(_habbo.HomeRoom));
                    SendQueue(new FavouritesComposer(userData.user.FavoriteRooms));
                    SendQueue(new FigureSetIdsComposer(_habbo.GetClothing().GetClothingParts));
                    SendQueue(new UserRightsComposer(_habbo));
                    SendQueue(new AvailabilityStatusComposer());
                    SendQueue(new AchievementScoreComposer(_habbo.GetStats().AchievementPoints));
                    SendQueue(new HabboClubSubscriptionComposer());
                    SendQueue(new BuildersClubMembershipComposer());
                    SendQueue(new CfhTopicsInitComposer());
                    SendQueue(new BadgeDefinitionsComposer(StarBlueServer.GetGame().GetAchievementManager()._achievements));
                    SendQueue(new SoundSettingsComposer(_habbo.ClientVolume, _habbo.ChatPreference, _habbo.AllowMessengerInvites, _habbo.FocusPreference, FriendBarStateUtility.GetInt(_habbo.FriendbarState)));
                    SendQueue(new SetCameraPicturePriceMessageComposer(100, 10, 0));

                    //SendMessage(new TalentTrackLevelComposer());

                    if (GetHabbo().GetMessenger() != null)
                    {
                        GetHabbo().GetMessenger().OnStatusChanged(true);
                    }

                    if (_habbo.Rank == 2 || _habbo.VIPRank == 1)
                    {
                        if (!_habbo.GetClubManager().HasSubscription("club_vip"))
                        {
                            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("DELETE FROM user_subscriptions WHERE user_id = @id");
                                dbClient.AddParameter("id", _habbo.Id);
                                dbClient.RunQuery();
                                dbClient.RunFastQuery("UPDATE `users` SET `rank` = '1' WHERE `id` = '" + _habbo.Id + "'");
                                dbClient.RunFastQuery("UPDATE `users` SET `rank_vip` = '0' WHERE `id` = '" + _habbo.Id + "'");
                                dbClient.RunFastQuery("UPDATE `users` SET `respetos` = '3' WHERE `id` = '" + _habbo.Id + "'");
                                _habbo.Rank = 1;
                                _habbo.VIPRank = 0;
                            }

                            if (!_habbo.GetBadgeComponent().HasBadge("VIP"))
                            {
                                _habbo.GetClient().GetHabbo().GetBadgeComponent().RemoveBadge("VIP");
                            }

                            if (!_habbo.GetBadgeComponent().HasBadge("DVIP"))
                            {
                                GetHabbo().GetBadgeComponent().RemoveBadge("DVIP");
                            }

                            GetHabbo().GetClubManager().ReloadSubscription(_habbo.GetClient());
                            SendQueue(new ScrSendUserInfoComposer(_habbo));
                            SendQueue(new SendHotelAlertLinkEventComposer("Infelizmente o seu VIP acabou, esperamos que tenha aproveitado bastante!"));
                        }
                    }

                    if (_habbo.Rank < 2 && !Convert.ToBoolean(StarBlueServer.GetConfig().data["hotel.open.for.users"]))
                    {
                        SendMessage(new SendHotelAlertLinkEventComposer("Atualmente somente a staff pode entrar no hotel, estamos testando tudo para você."));
                        Thread.Sleep(10000);
                        Disconnect();
                        return false;
                    }

                    if (!string.IsNullOrEmpty(MachineId))
                    {
                        if (_habbo.MachineId != MachineId)
                        {
                            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `users` SET `machine_id` = @MachineId WHERE `id` = @id LIMIT 1");
                                dbClient.AddParameter("MachineId", MachineId);
                                dbClient.AddParameter("id", _habbo.Id);
                                dbClient.RunQuery();
                            }
                        }

                        _habbo.MachineId = MachineId;
                    }

                    if (StarBlueServer.GetGame().GetSubscriptionManager().TryGetSubscriptionData(_habbo.VIPRank, out SubscriptionData SubData))
                    {

                        if (!string.IsNullOrEmpty(SubData.Badge))
                        {
                            if (!_habbo.GetBadgeComponent().HasBadge(SubData.Badge))
                            {
                                _habbo.GetBadgeComponent().GiveBadge(SubData.Badge, true, this);
                            }
                        }
                    }

                    if (!StarBlueServer.GetGame().GetCacheManager().ContainsUser(_habbo.Id))
                    {
                        StarBlueServer.GetGame().GetCacheManager().GenerateUser(_habbo.Id);
                    }

                    _habbo.InitProcess();

                    if (!Convert.ToBoolean(StarBlueServer.GetConfig().data["pin.system.enable"]))
                    {
                        GetHabbo().StaffOk = true;
                    }

                    if (GetHabbo().StaffOk)
                    {
                        if (GetHabbo().GetPermissions().HasRight("mod_tickets"))
                        {
                            SendQueue(new ModeratorInitComposer(
                            StarBlueServer.GetGame().GetModerationManager().UserMessagePresets,
                            StarBlueServer.GetGame().GetModerationManager().RoomMessagePresets,
                            StarBlueServer.GetGame().GetModerationManager().GetTickets));
                        }
                    }

                    if (GetHabbo().Rank >= 6)
                    {
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            if (StarBlueServer.GetRankManager().TryGetValue(GetHabbo().Rank, out RankData Rank))
                            {
                                if (!GetHabbo().GetBadgeComponent().HasBadge("HEIBBOSTAFF"))
                                {
                                    GetHabbo().GetBadgeComponent().GiveBadge("HEIBBOSTAFF", true, GetHabbo().GetClient());

                                }

                                if (!GetHabbo().GetBadgeComponent().HasBadge(Rank.Badge))
                                {
                                    GetHabbo().GetBadgeComponent().GiveBadge(Rank.Badge, true, GetHabbo().GetClient());
                                    SendQueue(RoomNotificationComposer.SendBubble("heibbostaff", "Você recebeu o emblema " + Rank.Badge + "!", "/inventory/open/badge"));
                                }
                            }
                        }
                    }

                    if ((GetHabbo().Rank > 2 && GetHabbo().Rank < 14) || GetHabbo()._guidelevel > 0)
                    {
                        HelperToolsManager.AddHelper(_habbo.GetClient(), false, true, true);
                        SendQueue(new HandleHelperToolComposer(true));
                    }

                    /*if (GetHabbo()._NuxRoom)
                    {
                        if (StarBlueServer.GetGame().GetCatalog().GetPredesignedRooms().predesignedRoom.ContainsKey((uint)1))
                        {

                            #region SELECT ROOM AND CREATE NEW
                            var predesigned = StarBlueServer.GetGame().GetCatalog().GetPredesignedRooms().predesignedRoom[(uint)1];
                            var decoration = predesigned.RoomDecoration;

                            var createRoom = StarBlueServer.GetGame().GetRoomManager().CreateRoom(userData.user.GetClient(), "Quarto de " + userData.user.GetClient().GetHabbo().Username, "Bem-vindo ao " + StarBlueServer.HotelName + " Hotel.", predesigned.RoomModel, 29, 25, 1);

                            createRoom.FloorThickness = int.Parse(decoration[0]);
                            createRoom.WallThickness = int.Parse(decoration[1]);
                            createRoom.Model.WallHeight = int.Parse(decoration[2]);
                            createRoom.Hidewall = ((decoration[3] == "True") ? 1 : 0);
                            createRoom.Wallpaper = decoration[4];
                            createRoom.Landscape = decoration[5];
                            createRoom.Floor = decoration[6];
                            var newRoom = StarBlueServer.GetGame().GetRoomManager().LoadRoom(createRoom.Id);
                            #endregion

                            #region CREATE FLOOR ITEMS
                            if (predesigned.FloorItems != null)
                                foreach (var floorItems in predesigned.FloorItemData)
                                    using (var dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                                        dbClient.RunFastQuery("INSERT INTO items (id, user_id, room_id, base_item, extra_data, x, y, z, rot, wall_pos, limited_number, limited_stack) VALUES (null, " + userData.user.GetClient().GetHabbo().Id + ", " + newRoom.RoomId + ", " + floorItems.BaseItem + ", '" + floorItems.ExtraData + "', " +
                                            floorItems.X + ", " + floorItems.Y + ", " + TextHandling.GetString(floorItems.Z) + ", " + floorItems.Rot + ", '', 0, 0);");
                            #endregion

                            #region CREATE WALL ITEMS
                            if (predesigned.WallItems != null)
                                foreach (var wallItems in predesigned.WallItemData)
                                    using (var dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                                        dbClient.RunFastQuery("INSERT INTO items (id, user_id, room_id, base_item, extra_data, x, y, z, rot, wall_pos, limited_number, limited_stack) VALUES (null, " + userData.user.GetClient().GetHabbo().Id + ", " + newRoom.RoomId + ", " + wallItems.BaseItem + ", '" + wallItems.ExtraData +
                                            "', 0, 0, 0, 0, '" + wallItems.WallCoord + "', 0, 0);");
                            #endregion

                            #region GENERATE ROOM AND SEND PACKET
                            userData.user.GetClient().GetHabbo().GetInventoryComponent().UpdateItems(false);
                            StarBlueServer.GetGame().GetRoomManager().LoadRoom(newRoom.Id).GetRoomItemHandler().LoadFurniture();
                            var newFloorItems = newRoom.GetRoomItemHandler().GetFloor;
                            foreach (var roomItem in newFloorItems) newRoom.GetRoomItemHandler().SetFloorItem(roomItem, roomItem.GetX, roomItem.GetY, roomItem.GetZ);
                            var newWallItems = newRoom.GetRoomItemHandler().GetWall;
                            foreach (var roomItem in newWallItems) newRoom.GetRoomItemHandler().SetWallItem(userData.user.GetClient(), roomItem);
                            //userData.user.GetClient().SendMessage(new FlatCreatedComposer(newRoom.Id, newRoom.Name));

                            Room Room = StarBlueServer.GetGame().GetRoomManager().LoadRoom(newRoom.Id);
                            userData.user.GetClient().GetHabbo()._NuxRoom = false;
                            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.RunFastQuery("UPDATE users SET nux_room = 'false' WHERE id = " + userData.user.GetClient().GetHabbo().Id + ";");
                            }

                            RoomAccess Access = RoomAccessUtility.ToRoomAccess("open");

                            Room.Access = Access;
                            Room.RoomData.Access = Access;
                            #endregion
                        }
                        else
                        {
                            Logging.WriteLine("Predesigned Room Id not found.", ConsoleColor.Red);
                        }
                    }*/

                    if (StarBlueServer.GetGame().GetTargetedOffersManager().TargetedOffer != null)
                    {
                        StarBlueServer.GetGame().GetTargetedOffersManager().Initialize(StarBlueServer.GetDatabaseManager().GetQueryReactor());
                        TargetedOffers TargetedOffer = StarBlueServer.GetGame().GetTargetedOffersManager().TargetedOffer;

                        if (TargetedOffer.Expire > StarBlueServer.GetIUnixTimestamp())
                        {

                            if (TargetedOffer.Limit != GetHabbo()._TargetedBuy)
                            {

                                SendQueue(StarBlueServer.GetGame().GetTargetedOffersManager().TargetedOffer.Serialize());
                            }
                        }
                        else if (TargetedOffer.Expire == -1)
                        {

                            if (TargetedOffer.Limit != GetHabbo()._TargetedBuy)
                            {

                                SendQueue(StarBlueServer.GetGame().GetTargetedOffersManager().TargetedOffer.Serialize());
                            }
                        }
                        else
                        {
                            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.RunFastQuery("UPDATE targeted_offers SET active = 'false';UPDATE users SET targeted_buy = '0' WHERE targeted_buy > 0");
                            }
                        }
                    }

                    if (_habbo.MysticBoxes.Count == 0 && _habbo.MysticKeys.Count == 0)
                    {
                        int box = RandomNumber.GenerateRandom(1, 8);
                        string boxcolor = "";
                        switch (box)
                        {
                            case 1:
                                boxcolor = "purple";
                                break;
                            case 2:
                                boxcolor = "blue";
                                break;
                            case 3:
                                boxcolor = "green";
                                break;
                            case 4:
                                boxcolor = "yellow";
                                break;
                            case 5:
                                boxcolor = "lilac";
                                break;
                            case 6:
                                boxcolor = "orange";
                                break;
                            case 7:
                                boxcolor = "turquoise";
                                break;
                            case 8:
                                boxcolor = "red";
                                break;
                        }

                        int key = RandomNumber.GenerateRandom(1, 8);
                        string keycolor = "";
                        switch (key)
                        {
                            case 1:
                                keycolor = "purple";
                                break;
                            case 2:
                                keycolor = "blue";
                                break;
                            case 3:
                                keycolor = "green";
                                break;
                            case 4:
                                keycolor = "yellow";
                                break;
                            case 5:
                                keycolor = "lilac";
                                break;
                            case 6:
                                keycolor = "orange";
                                break;
                            case 7:
                                keycolor = "turquoise";
                                break;
                            case 8:
                                keycolor = "red";
                                break;
                        }

                        _habbo.MysticKeys.Add(keycolor);
                        _habbo.MysticBoxes.Add(boxcolor);

                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunFastQuery("INSERT INTO user_mystic_data (user_id, mystic_keys, mystic_boxes) VALUES(" + GetHabbo().Id + ", '" + keycolor + "', '" + boxcolor + "');");
                        }
                    }

                    SendQueue(new MysteryBoxDataComposer(_habbo));
                    Flush();
                    //SendMessage(new NuxAlertComposer("habbopages/welcome.txt?" + RandomNumber.GenerateRandom(0, 999999)));
                    //SendNotification("Bem-vindo ao <b><font color='#FF4500'>" + StarBlueServer.HotelName + " Hotel</font></b>, " + GetHabbo().Username + ".\n\nEstamos felizes por você estar aqui. |\n\n<b>PREMIAÇÕES EM EVENTOS:</b>\nNível 1 ao 50: <b>5 diamantes, 1 honra.</b>\nNível 50 ao 100: <b>10 diamantes, 2 honras.</b>\nNível 100+: <b>20 diamantes, 2 honras.</b>\nParticipe e acumule prêmios!\n\nFique online e receba diamantes e honras!\nConfira também nossas promoções ativas e ganhe prêmios exclusivos!\n\nDivulgue o hotel e chame seus amigos!\n\n<b>Atenciosamente, Equipe " + StarBlueServer.HotelName + "</b>.");

                    StarBlueServer.GetGame().GetRewardManager().CheckRewards(this);
                    StarBlueServer.GetGame().GetAchievementManager().TryProgressHabboClubAchievements(this);
                    StarBlueServer.GetGame().GetAchievementManager().TryProgressRegistrationAchievements(this);
                    StarBlueServer.GetGame().GetAchievementManager().TryProgressLoginAchievements(this);

                    return true;
                }
            }
            catch (Exception e)
            {
                Logging.LogCriticalException("Bug during user login: " + e);
            }
            return false;
        }

        public IChannelHandlerContext GetChannel()
        {
            return channel;
        }

        public void SendWhisper(string Message, int Colour = 0)
        {
            if (this == null || GetHabbo() == null || GetHabbo().CurrentRoom == null)
            {
                return;
            }

            RoomUser User = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Username);
            if (User == null)
            {
                return;
            }

            SendMessage(new WhisperComposer(User.VirtualId, Message, 0, (Colour == 0 ? User.LastBubble : Colour)));
        }

        public void SendChat(string Message, int Colour = 0)
        {
            if (this == null || GetHabbo() == null || GetHabbo().CurrentRoom == null)
            {
                return;
            }

            RoomUser User = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Username);
            if (User == null)
            {
                return;
            }

            SendMessage(new ChatComposer(User.VirtualId, Message, 0, (Colour == 0 ? User.LastBubble : Colour)));
        }

        public void SendNotification(string Message)
        {
            SendMessage(new BroadcastMessageAlertComposer(Message));
        }

        public void SendMessage(MessageComposer Message)
        {
            channel.WriteAndFlushAsync(Message);
        }

        public void SendQueue(MessageComposer Message)
        {
            channel.WriteAsync(Message);
        }

        public void Flush()
        {
            channel.Flush();
        }

        public void SendMessages(List<MessageComposer> messages)
        {
            foreach (MessageComposer message in messages)
            {
                channel.WriteAsync(message);
            }

            channel.Flush();
        }

        public void SendMessage(byte[] Message)
        {
            channel.WriteAndFlushAsync(Message);
        }

        public string GetIpAddress()
        {
            try
            {
                return ((IPEndPoint)channel.Channel.RemoteAddress).Address.MapToIPv4().ToString();
            }
            catch
            {
                return "0.0.0.0";
            }
        }

        public void SendShout(string Message, int Colour = 0)
        {
            if (this == null || GetHabbo() == null || GetHabbo().CurrentRoom == null)
            {
                return;
            }

            RoomUser User = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Username);
            if (User == null)
            {
                return;
            }

            SendMessage(new ShoutComposer(User.VirtualId, Message, 0, (Colour == 0 ? User.LastBubble : Colour)));
        }

        public int ConnectionID => _id;

        public Habbo GetHabbo()
        {
            return _habbo;
        }

        public void Disconnect()
        {
            try
            {
                if (GetHabbo() != null)
                {
                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunFastQuery(GetHabbo().GetQueryString);
                    }

                    GetHabbo().OnDisconnect();
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e.ToString());
            }


            if (!_disconnected)
            {
                if (channel != null)
                {
                    channel.DisconnectAsync();
                    channel.CloseAsync();
                }
                _disconnected = true;
            }
        }

        public void Dispose()
        {
            if (GetHabbo() != null)
            {
                GetHabbo().OnDisconnect();
            }

            MachineId = string.Empty;
            _disconnected = true;
            _habbo = null;
            channel.DisconnectAsync();
            channel.CloseAsync();
        }

        public RoomUser GetRoomUser()
        {
            RoomUser RUser = null;
            try
            {
                if (this == null || GetHabbo() == null || GetHabbo().CurrentRoom == null)
                {
                    return null;
                }

                RUser = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Id);
            }
            catch
            {
                return RUser;
            }
            return RUser;
        }
    }
}