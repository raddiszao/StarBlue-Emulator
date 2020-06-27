using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Catalog;
using StarBlue.Communication.Packets.Outgoing.Handshake;
using StarBlue.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using StarBlue.Communication.Packets.Outgoing.Inventory.Bots;
using StarBlue.Communication.Packets.Outgoing.Inventory.Furni;
using StarBlue.Communication.Packets.Outgoing.Inventory.Pets;
using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Communication.Packets.Outgoing.Navigator;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Communication.Packets.Outgoing.Users;
using StarBlue.Core;
using StarBlue.HabboHotel.Catalog;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Groups;
using StarBlue.HabboHotel.Groups.Forums;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Items.Utilities;
using StarBlue.HabboHotel.Rooms.AI;
using StarBlue.HabboHotel.Users.Effects;
using StarBlue.HabboHotel.Users.Inventory.Bots;
using StarBlue.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    public class PurchaseFromCatalogEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (StarBlueServer.GetSettingsManager().TryGetValue("catalogue_enabled") != "1")
            {
                Session.SendNotification("The hotel managers have disabled the catalogue.");
                return;
            }

            int PageId = Packet.PopInt();
            int ItemId = Packet.PopInt();
            string ExtraData = Packet.PopString();
            int Amount = Packet.PopInt();

            if (!StarBlueServer.GetGame().GetCatalog().TryGetPage(PageId, out CatalogPage Page))
            {
                return;
            }

            if (Session.GetHabbo().Rank > 3 && !Session.GetHabbo().StaffOk || StarBlueServer.GoingIsToBeClose)
            {
                return;
            }

            if (!Page.Enabled || !Page.Visible || Page.MinimumRank > Session.GetHabbo().Rank || (Page.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 2))
            {
                return;
            }

            bool ValidItem = true;

            if (!Page.Items.TryGetValue(ItemId, out CatalogItem Item))
            {
                if (Page.ItemOffers.ContainsKey(ItemId))
                {
                    Item = Page.ItemOffers[ItemId];
                    if (Item == null)
                    {
                        ValidItem = false;
                    }
                }
                else
                {
                    ValidItem = false;
                }
            }

            if (!ValidItem)
            {
                Console.WriteLine("[" + ItemId + "] Catalog cant load item.");
                return;
            }


            ItemData baseItem = Item.GetBaseItem(Item.ItemId);
            if (baseItem != null)
            {
                if (baseItem.InteractionType == InteractionType.club_1_month || baseItem.InteractionType == InteractionType.club_3_month || baseItem.InteractionType == InteractionType.club_6_month)
                {
                    if (Item.CostCredits > Session.GetHabbo().Credits)
                    {
                        return;
                    }

                    int Months = 0;

                    switch (baseItem.InteractionType)
                    {
                        case InteractionType.club_1_month:
                            Months = 1;
                            break;

                        case InteractionType.club_3_month:
                            Months = 3;
                            break;

                        case InteractionType.club_6_month:
                            Months = 6;
                            break;
                    }

                    int num = num = 31 * Months;

                    if (Item.CostCredits > 0)
                    {
                        Session.GetHabbo().Credits -= Item.CostCredits;
                        Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));

                    }

                    Session.GetHabbo().GetClubManager().AddOrExtendSubscription("habbo_vip", num * 24 * 3600, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("HC1", true, Session);

                    Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
                    Session.SendMessage(new FurniListUpdateComposer());
                    return;
                }

                if (baseItem.InteractionType == InteractionType.namecolor)
                {
                    if (Item.CostGOTWPoints > Session.GetHabbo().GOTWPoints)
                    {
                        return;
                    }

                    if (Item.CostGOTWPoints > 0)
                    {
                        Session.GetHabbo().GOTWPoints -= Item.CostGOTWPoints;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
                    }

                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunFastQuery("UPDATE `users` SET `namecolor` = '" + Item.Name + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    }

                    Session.GetHabbo()._nameColor = Item.Name;
                    Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
                    Session.SendMessage(new FurniListUpdateComposer());
                    return;
                }

                if (baseItem.InteractionType == InteractionType.changename)
                {
                    if (Item.CostDiamonds > Session.GetHabbo().Diamonds)
                    {
                        return;
                    }

                    if (Item.CostDiamonds > 0)
                    {
                        Session.GetHabbo().Diamonds -= Item.CostDiamonds;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
                    }

                    Session.GetHabbo().LastNameChange = 0;
                    Session.GetHabbo().ChangingName = true;
                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunFastQuery("UPDATE users SET changename = '1' WHERE id = " + Session.GetHabbo().Id + "");
                    }

                    Session.GetHabbo()._changename = 1;
                    Session.SendMessage(new UserObjectComposer(Session.GetHabbo()));

                    Session.SendWhisper("Você acabou de adquirir a troca de nome, clique no seu boneco para trocá-lo.", 34);

                    Session.SendMessage(new FurniListUpdateComposer());
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
                    return;
                }



                if (baseItem.InteractionType == InteractionType.tag)
                {
                    if (ExtraData.Length == 0 || ExtraData.Length > 8 || ExtraData.Length < 0)
                    {
                        Session.SendMessage(RoomNotificationComposer.SendBubble("advice", "Oops, o seu prefixo só pode conter de 1 a 7 caracteres.", ""));
                        return;
                    }
                    else if (ExtraData.Contains("<br>") || ExtraData.Contains("<b>") || ExtraData.Contains("<i>") || ExtraData.Contains("<Br>") || ExtraData.Contains("<BR>") ||
                        ExtraData.Contains("<bR>") || ExtraData.Contains("<B>") || ExtraData.Contains("<I>"))
                    {
                        Session.SendMessage(RoomNotificationComposer.SendBubble("advice", "Oops, não pode usar TAG HTML nos prefixos.", ""));
                        return;
                    }

                    else if (ExtraData.ToUpper().Contains("MNG") || ExtraData.ToUpper().Contains("BOT") || ExtraData.ToUpper().Contains("B0T") || ExtraData.ToUpper().Contains("BAW") || ExtraData.ToUpper().Contains("CLB") || ExtraData.ToUpper().Contains("GUIA") || ExtraData.ToUpper().Contains("INTER") ||
                        ExtraData.ToUpper().Contains("INT") || ExtraData.ToUpper().Contains("EDC") || ExtraData.ToUpper().Contains("VIP") || ExtraData.ToUpper().Contains("EDP") ||
                        ExtraData.ToUpper().Contains("ADM") || ExtraData.ToUpper().Contains("MOD") || ExtraData.ToUpper().Contains("M0D") || ExtraData.ToUpper().Contains("STAFF") ||
                        ExtraData.ToUpper().Contains("0WNER") || ExtraData.ToUpper().Contains("OWNER") || ExtraData.ToUpper().Contains("GM") || ExtraData.ToUpper().Contains("EDM") ||
                        ExtraData.ToUpper().Contains("ROOKIE") || ExtraData.ToUpper().Contains("R00KIE") || ExtraData.ToUpper().Contains("BAW") || ExtraData.ToUpper().Contains("HFM") || ExtraData.ToUpper().Contains("\r"))
                    {
                        Session.SendNotification("Oops, não pode colocar tag staff!");
                        return;
                    }

                    if (!StarBlueServer.IsValidAlphaNumeric(ExtraData))
                    {
                        Session.SendMessage(RoomNotificationComposer.SendBubble("advice", "Seu prefixo contém carácteres inválidos.", ""));
                        return;
                    }

                    ExtraData = StarBlueServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(ExtraData, out string character) ? "" : ExtraData;

                    if (string.IsNullOrEmpty(ExtraData))
                    {
                        Session.SendNotification("Esta palavra " + character + " não é uma palavra permitida pelo " + StarBlueServer.HotelName + " hotel.");
                        return;
                    }


                    if (Item.CostCredits > Session.GetHabbo().Credits || Item.CostPixels > Session.GetHabbo().Duckets || Item.CostDiamonds > Session.GetHabbo().Diamonds || Item.CostGOTWPoints > Session.GetHabbo().GOTWPoints)
                    {
                        return;
                    }

                    if (Item.CostCredits > 0)
                    {
                        Session.GetHabbo().Credits -= Item.CostCredits;
                        Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                    }

                    if (Item.CostPixels > 0)
                    {
                        Session.GetHabbo().Duckets -= Item.CostPixels;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));//Love you, Tom.
                    }

                    if (Item.CostDiamonds > 0)
                    {
                        Session.GetHabbo().Diamonds -= Item.CostDiamonds;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
                    }

                    if (Item.CostGOTWPoints > 0)
                    {
                        Session.GetHabbo().GOTWPoints -= Item.CostGOTWPoints;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
                    }


                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunFastQuery("UPDATE `users` SET `tag` = '[" + ExtraData + "]' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    }

                    Session.GetHabbo()._tag = "[" + ExtraData + "]";
                    Session.SendMessage(new AlertNotificationHCMessageComposer(4));
                    Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
                    Session.SendMessage(new FurniListUpdateComposer());
                    return;
                }

                if (baseItem.InteractionType == InteractionType.tag_vip)
                {

                    if (Item.CostCredits > Session.GetHabbo().Credits || Item.CostPixels > Session.GetHabbo().Duckets || Item.CostDiamonds > Session.GetHabbo().Diamonds || Item.CostGOTWPoints > Session.GetHabbo().GOTWPoints)
                    {
                        return;
                    }

                    if (Item.CostCredits > 0)
                    {
                        Session.GetHabbo().Credits -= Item.CostCredits;
                        Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                    }

                    if (Item.CostPixels > 0)
                    {
                        Session.GetHabbo().Duckets -= Item.CostPixels;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));//Love you, Tom.
                    }

                    if (Item.CostDiamonds > 0)
                    {
                        Session.GetHabbo().Diamonds -= Item.CostDiamonds;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
                    }

                    if (Item.CostGOTWPoints > 0)
                    {
                        Session.GetHabbo().GOTWPoints -= Item.CostGOTWPoints;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
                    }


                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunFastQuery("UPDATE `users` SET `tag` = '" + Item.ExtraData + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    }

                    Session.GetHabbo()._tag = Item.ExtraData;
                    Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
                    Session.SendMessage(new FurniListUpdateComposer());
                    return;
                }


                if (baseItem.InteractionType == InteractionType.tagcolor)
                {
                    if (Item.CostGOTWPoints > Session.GetHabbo().GOTWPoints)
                    {
                        return;
                    }

                    if (Item.CostGOTWPoints > 0)
                    {
                        Session.GetHabbo().GOTWPoints -= Item.CostGOTWPoints;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
                    }

                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunFastQuery("UPDATE `users` SET `tagcolor` = '" + Item.Name + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    }

                    Session.GetHabbo()._tagcolor = Item.Name;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
                    Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
                    return;
                }

                if (baseItem.InteractionType == InteractionType.tagrainbow)
                {
                    if (Item.CostDiamonds > Session.GetHabbo().Diamonds)
                    {
                        return;
                    }

                    if (Item.CostDiamonds > 0)
                    {
                        Session.GetHabbo().Diamonds -= Item.CostDiamonds;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 103));
                    }

                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunFastQuery("UPDATE `users` SET `tagcolor` = '" + Item.Name + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    }

                    Session.GetHabbo()._tagcolor = Item.Name;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
                    Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
                    Session.SendMessage(new FurniListUpdateComposer());
                    return;
                }


                if (baseItem.InteractionType == InteractionType.club_vip || baseItem.InteractionType == InteractionType.club_vip2)
                {
                    if (Item.CostDiamonds > Session.GetHabbo().Diamonds)
                    {
                        return;
                    }

                    int DurationSeconds = 1;
                    switch (baseItem.InteractionType)
                    {
                        case InteractionType.club_vip:
                            DurationSeconds = 2592000;
                            break;

                        case InteractionType.club_vip2:
                            DurationSeconds = 7889400;
                            break;
                    }

                    Session.GetHabbo().Diamonds -= Item.CostDiamonds;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));

                    var IsVIP = Session.GetHabbo().GetClubManager().HasSubscription("club_vip");
                    if (IsVIP)
                    {
                        Session.SendMessage(new AlertNotificationHCMessageComposer(4));
                    }
                    else
                    {
                        Session.SendMessage(new AlertNotificationHCMessageComposer(5));
                    }

                    Session.GetHabbo().GetClubManager().AddOrExtendSubscription("club_vip", DurationSeconds, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("DVIP", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("ACH_VipClub12", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("ES28A", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("ES551", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("BR967", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("DE720", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("BR415", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("shop", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("PT054", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("PX4", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("PX3", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("UK277", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("THI95", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("NL185", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("NL537", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("ES720", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("ES78A", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("HST27", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("ROOMP", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("ES800", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("ROOMP", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("ES679", true, Session);

                    Session.SendMessage(RoomNotificationComposer.SendBubble("shop", "" + Session.GetHabbo().Username + ", Você recebeu uma combinação de emblemas por ingressar no VIP Club do " + StarBlueServer.HotelName + ".", ""));

                    StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_VipClub", 1);
                    Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
                    Session.SendMessage(new FurniListUpdateComposer());

                    if (Session.GetHabbo().Rank > 2)
                    {
                        Session.SendWhisper("Você possui rank, por que quer comprar vip?");
                        return;
                    }

                    else if (Session.GetHabbo().Rank < 2)
                    {
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunFastQuery("UPDATE `users` SET `rank` = '2' WHERE `id` = '" + Session.GetHabbo().Id + "'");
                            dbClient.RunFastQuery("UPDATE `users` SET `rank_vip` = '1' WHERE `id` = '" + Session.GetHabbo().Id + "'");
                            dbClient.RunFastQuery("UPDATE `users` SET `respetos` = '5' WHERE `id` = '" + Session.GetHabbo().Id + "'");
                            Session.GetHabbo().Rank = 2;
                            Session.GetHabbo().VIPRank = 1;

                            if (Session.GetRoomUser() != null)
                            {
                                Session.GetRoomUser().ApplyEffect(593);
                            }
                        }
                    }

                    return;
                }
            }

            if (Amount < 1 || Amount > 100)
            {
                Amount = 1;
            }

            int AmountPurchase = Item.Amount > 1 ? Item.Amount : Amount;
            int TotalCreditsCost = Amount > 1 ? ((Item.CostCredits * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostCredits)) : Item.CostCredits;
            int TotalPixelCost = Amount > 1 ? ((Item.CostPixels * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostPixels)) : Item.CostPixels;
            int TotalDiamondCost = Amount > 1 ? ((Item.CostDiamonds * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostDiamonds)) : Item.CostDiamonds;
            int TotalGOTWPointsCost = Amount > 1 ? ((Item.CostGOTWPoints * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostGOTWPoints)) : Item.CostGOTWPoints;
            //int TotalPumpkinsCost = Amount > 1 ? ((Item.CostPumpkins * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostPumpkins)) : Item.CostPumpkins;

            if (Session.GetHabbo().Credits < TotalCreditsCost || Session.GetHabbo().Duckets < TotalPixelCost || Session.GetHabbo().Diamonds < TotalDiamondCost || Session.GetHabbo().GOTWPoints < TotalGOTWPointsCost)
            {
                return;
            }

            int LimitedEditionSells = 0;
            int LimitedEditionStack = 0;


            if (Item.IsLimited)
            {
                if (Item.LimitedEditionStack <= Item.LimitedEditionSells)
                {
                    Session.SendMessage(new LTDSoldAlertComposer());
                    Session.SendMessage(new CatalogUpdatedComposer());
                    Session.SendMessage(new PurchaseOKComposer());
                    return;
                }

                Item.LimitedEditionSells++;
                StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_LTDPurchased", 1);

                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("UPDATE `catalog_items` SET `limited_sells` = '" + Item.LimitedEditionSells + "' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    LimitedEditionSells = Item.LimitedEditionSells;
                    LimitedEditionStack = Item.LimitedEditionStack;
                }
            }



            if (Item.CostCredits > 0)
            {
                Session.GetHabbo().Credits -= TotalCreditsCost;
                Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
            }

            if (Item.CostPixels > 0)
            {
                Session.GetHabbo().Duckets -= TotalPixelCost;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));//Love you, Tom.
            }

            if (Item.CostDiamonds > 0)
            {
                Session.GetHabbo().Diamonds -= TotalDiamondCost;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
            }

            if (Item.CostGOTWPoints > 0)
            {
                Session.GetHabbo().GOTWPoints -= TotalGOTWPointsCost;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
            }


            #region PREDESIGNED_ROOM BY KOMOK
            if (Item.PredesignedId > 0 && StarBlueServer.GetGame().GetCatalog().GetPredesignedRooms().predesignedRoom.ContainsKey((uint)Item.PredesignedId))
            {
                #region SELECT ROOM AND CREATE NEW
                var predesigned = StarBlueServer.GetGame().GetCatalog().GetPredesignedRooms().predesignedRoom[(uint)Item.PredesignedId];
                var decoration = predesigned.RoomDecoration;

                var createRoom = StarBlueServer.GetGame().GetRoomManager().CreateRoom(Session, Session.GetHabbo().Username + "'s room", "¡Una Sala pre-decorada!", predesigned.RoomModel, 1, 25, 1);

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
                {
                    foreach (var floorItems in predesigned.FloorItemData)
                    {
                        using (var dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunFastQuery("INSERT INTO items (id, user_id, room_id, base_item, extra_data, x, y, z, rot, wall_pos, limited_number, limited_stack) VALUES (null, " + Session.GetHabbo().Id + ", " + newRoom.RoomId + ", " + floorItems.BaseItem + ", '" + floorItems.ExtraData + "', " +
                                floorItems.X + ", " + floorItems.Y + ", " + TextHandling.GetString(floorItems.Z) + ", " + floorItems.Rot + ", '', 0, 0);");
                        }
                    }
                }
                #endregion

                #region CREATE WALL ITEMS
                if (predesigned.WallItems != null)
                {
                    foreach (var wallItems in predesigned.WallItemData)
                    {
                        using (var dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunFastQuery("INSERT INTO items (id, user_id, room_id, base_item, extra_data, x, y, z, rot, wall_pos, limited_number, limited_stack) VALUES (null, " + Session.GetHabbo().Id + ", " + newRoom.RoomId + ", " + wallItems.BaseItem + ", '" + wallItems.ExtraData +
                                "', 0, 0, 0, 0, '" + wallItems.WallCoord + "', 0, 0);");
                        }
                    }
                }
                #endregion

                #region VERIFY IF CONTAINS BADGE AND GIVE
                if (Item.Badge != string.Empty)
                {
                    Session.GetHabbo().GetBadgeComponent().GiveBadge(Item.Badge, true, Session);
                }
                #endregion

                #region GENERATE ROOM AND SEND PACKET
                Session.SendMessage(new PurchaseOKComposer());
                Session.GetHabbo().GetInventoryComponent().UpdateItems(false);
                StarBlueServer.GetGame().GetRoomManager().LoadRoom(newRoom.Id).GetRoomItemHandler().LoadFurniture();
                var newFloorItems = newRoom.GetRoomItemHandler().GetFloor;
                foreach (var roomItem in newFloorItems)
                {
                    newRoom.GetRoomItemHandler().SetFloorItem(roomItem, roomItem.GetX, roomItem.GetY, roomItem.GetZ);
                }

                var newWallItems = newRoom.GetRoomItemHandler().GetWall;
                foreach (var roomItem in newWallItems)
                {
                    newRoom.GetRoomItemHandler().SetWallItem(Session, roomItem);
                }

                Session.SendMessage(new FlatCreatedComposer(newRoom.Id, newRoom.Name));
                #endregion
                return;
            }
            #endregion

            #region Create the extradata
            switch (Item.Data.InteractionType)
            {
                case InteractionType.NONE:
                    ExtraData = "";
                    break;



                case InteractionType.GUILD_CHAT:
                    Group thegroup;
                    if (!StarBlueServer.GetGame().GetGroupManager().TryGetGroup(Convert.ToInt32(ExtraData), out thegroup))
                    {
                        return;
                    }

                    if (!(StarBlueServer.GetGame().GetGroupManager().GetGroupsForUser(Session.GetHabbo().Id).Contains(thegroup)))
                    {
                        return;
                    }

                    int groupID = Convert.ToInt32(ExtraData);
                    if (thegroup.CreatorId == Session.GetHabbo().Id)
                    {
                        thegroup.CreateGroupChat(thegroup);

                    }
                    else if (thegroup.CreatorId != Session.GetHabbo().Id)
                    {
                        Session.SendNotification("Solo el dueño del grupo puede comprar esto");
                        return;
                    }
                    ExtraData = "" + groupID;


                    break;

                case InteractionType.GUILD_FORUM:
                    Group Gp;
                    GroupForum Gf;
                    int GpId;
                    if (!int.TryParse(ExtraData, out GpId))
                    {
                        Session.SendNotification("Oopss! Some error when getting the group ID...");
                        Session.SendMessage(new PurchaseOKComposer());
                        return;
                    }
                    if (!StarBlueServer.GetGame().GetGroupManager().TryGetGroup(GpId, out Gp))
                    {
                        Session.SendNotification("Error! this group doesn't exists");
                        Session.SendMessage(new PurchaseOKComposer());
                        return;
                    }

                    if (Gp.CreatorId != Session.GetHabbo().Id)
                    {
                        Session.SendNotification("¡Error! No eres el dueño del grupo así que no puedes crear el foro.\n\nPrimero el foro debe ser creado por el dueño del grupo...");
                        Session.SendMessage(new PurchaseOKComposer());
                        return;
                    }
                    Gf = StarBlueServer.GetGame().GetGroupForumManager().CreateGroupForum(Gp);
                    Session.SendMessage(new RoomNotificationComposer("forums.delivered", new Dictionary<string, string>
                            { { "groupId", Gp.Id.ToString() },  { "groupName", Gp.Name } }));
                    break;

                case InteractionType.GUILD_ITEM:
                case InteractionType.GUILD_GATE:
                case InteractionType.HCGATE:
                case InteractionType.VIPGATE:
                    break;

                case InteractionType.PINATA:
                case InteractionType.PLANT_SEED:
                case InteractionType.PINATATRIGGERED:
                case InteractionType.MAGICEGG:
                    ExtraData = "0";
                    break;

                case InteractionType.FOOTBALL_GATE:
                    ExtraData = "hd-180-14.ch-210-1408.lg-270-1408,hd-600-14.ch-630-1408.lg-695-1408";
                    break;

                #region Pet handling

                case InteractionType.PET:
                    try
                    {
                        string[] Bits = ExtraData.Split('\n');
                        string PetName = Bits[0];
                        string Race = Bits[1];
                        string Color = Bits[2];

                        if (!PetUtility.CheckPetName(PetName))
                        {
                            return;
                        }

                        if (Race.Length > 2)
                        {
                            return;
                        }

                        if (Color.Length != 6)
                        {
                            return;
                        }

                        StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PetLover", 1);
                    }
                    catch (Exception e)
                    {
                        Logging.LogException(e.ToString());
                        return;
                    }

                    break;

                #endregion

                case InteractionType.FLOOR:
                case InteractionType.WALLPAPER:
                case InteractionType.LANDSCAPE:

                    Double Number = 0;

                    try
                    {
                        if (string.IsNullOrEmpty(ExtraData))
                        {
                            Number = 0;
                        }
                        else
                        {
                            Number = Double.Parse(ExtraData, StarBlueServer.CultureInfo);
                        }
                    }
                    catch (Exception e)
                    {
                        Logging.HandleException(e, "Catalog.HandlePurchase: " + ExtraData);
                    }

                    ExtraData = Number.ToString().Replace(',', '.');
                    break;

                case InteractionType.POSTIT:
                    ExtraData = "FFFF33";
                    break;

                case InteractionType.MOODLIGHT:
                    ExtraData = "1,1,1,#000000,255";
                    break;

                case InteractionType.TROPHY:
                    ExtraData = Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) + ExtraData;
                    break;

                case InteractionType.MANNEQUIN:
                    ExtraData = "m" + Convert.ToChar(5) + ".ch-210-1321.lg-285-92" + Convert.ToChar(5) + "Default Maniquim";
                    break;

                case InteractionType.MUSIC_DISC:
                    ExtraData = Item.ExtraData;
                    break;

                case InteractionType.BADGE_DISPLAY:
                    if (!Session.GetHabbo().GetBadgeComponent().HasBadge(ExtraData))
                    {
                        Session.SendMessage(new BroadcastMessageAlertComposer("Oops, ocorreu um erro."));
                        return;
                    }

                    ExtraData = ExtraData + Convert.ToChar(9) + Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                    break;

                case InteractionType.BADGE:
                    {
                        if (Session.GetHabbo().GetBadgeComponent().HasBadge(Item.Data.ItemName))
                        {
                            Session.SendMessage(new PurchaseErrorComposer(1));
                            return;
                        }
                        break;
                    }
                default:
                    ExtraData = "";
                    break;
            }
            #endregion

            Item NewItem = null;
            switch (Item.Data.Type.ToString().ToLower())
            {
                default:
                    List<Item> GeneratedGenericItems = new List<Item>();

                    switch (Item.Data.InteractionType)
                    {
                        default:
                            if (!Session.GetHabbo().LastPurchasesItems.ContainsKey(Item.Id))
                            {
                                Session.GetHabbo().LastPurchasesItems.Add(Item.Id, Item);
                                CatalogPage LastPurchasesPage = StarBlueServer.GetGame().GetCatalog().TryGetPageByPageLink("last_purchases");
                                if (LastPurchasesPage != null && !LastPurchasesPage.Items.ContainsKey(Item.Id))
                                {
                                    LastPurchasesPage.Items.Add(Item.Id, Item);
                                }
                            }

                            if (AmountPurchase > 1)
                            {
                                List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase);

                                if (Items != null)
                                {
                                    GeneratedGenericItems.AddRange(Items);
                                }
                            }
                            else
                            {
                                NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData, 0, LimitedEditionSells, LimitedEditionStack);

                                if (NewItem != null)
                                {
                                    GeneratedGenericItems.Add(NewItem);
                                }
                            }
                            break;

                        case InteractionType.GUILD_GATE:
                        case InteractionType.GUILD_ITEM:
                        case InteractionType.GUILD_FORUM:
                            if (AmountPurchase > 1)
                            {

                                List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase, Convert.ToInt32(ExtraData));

                                if (Items != null)
                                {
                                    GeneratedGenericItems.AddRange(Items);
                                }
                            }
                            else
                            {
                                NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData, Convert.ToInt32(ExtraData));

                                if (NewItem != null)
                                {
                                    GeneratedGenericItems.Add(NewItem);
                                }
                            }
                            break;

                        case InteractionType.ARROW:
                        case InteractionType.TELEPORT:
                            for (int i = 0; i < AmountPurchase; i++)
                            {
                                List<Item> TeleItems = ItemFactory.CreateTeleporterItems(Item.Data, Session.GetHabbo());

                                if (TeleItems != null)
                                {
                                    GeneratedGenericItems.AddRange(TeleItems);
                                }
                            }
                            break;

                        case InteractionType.MOODLIGHT:
                            {
                                if (AmountPurchase > 1)
                                {
                                    List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase);

                                    if (Items != null)
                                    {
                                        GeneratedGenericItems.AddRange(Items);
                                        foreach (Item I in Items)
                                        {
                                            ItemFactory.CreateMoodlightData(I);
                                        }
                                    }
                                }
                                else
                                {
                                    NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData);

                                    if (NewItem != null)
                                    {
                                        GeneratedGenericItems.Add(NewItem);
                                        ItemFactory.CreateMoodlightData(NewItem);
                                    }
                                }
                            }
                            break;

                        case InteractionType.reward_box:
                            {
                                string ED = Session.GetHabbo().Username + Convert.ToChar(5) + "Você escolheu um baú Legado de nível comum, pode obter raros que pesam entre 0 e 12 vips. Lembre-se de que eles estão disponíveis apenas por um certo tempo." + Convert.ToChar(5) + Session.GetHabbo().Id + Convert.ToChar(5) + Item.Data.Id + Convert.ToChar(5) + 206 + Convert.ToChar(5) + 1 + Convert.ToChar(5) + 1;
                                ExtraData = ED;
                                int NewItemId = 0;

                                int Reward = RandomNumber.GenerateRandom(1, 10);
                                #region Rewards
                                switch (Reward)
                                {
                                    case 1:
                                        Reward = 9501; // Humadera Azul Colorable - rare_colourable_scifirocket*1
                                        break;
                                    case 2:
                                        Reward = 9510; // Elefante Azul Colorable - rare_colourable_elephant_statue*1
                                        break;
                                    case 3:
                                        Reward = 1587; // Lámpara Calippo - ads_calip_lava
                                        break;
                                    case 4:
                                        Reward = 540004; // Alce Fiel - loyalty_elk
                                        break;
                                    case 5:
                                        Reward = 385; // Toldo Amarillo - marquee*4
                                        break;
                                    case 6:
                                        Reward = 9502; // Fontana Azul - rare_colourable_fountain*1
                                        break;
                                    case 7:
                                        Reward = 212; // VIP - club_sofa
                                        break;
                                    case 8:
                                        Reward = 9506; // Parasol Azul - rare_colourable_parasol*1
                                        break;
                                    case 9:
                                        Reward = 9514; // Puerta Laser Azul - rare_colourable_scifiport*1
                                        break;
                                    case 10:
                                        Reward = 353; // Humadera Rosa - scifirocket*4
                                        break;
                                }
                                #endregion

                                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("INSERT INTO `items` (`base_item`,`user_id`,`extra_data`) VALUES (9377, '" + Session.GetHabbo().Id + "', @extra_data)");
                                    dbClient.AddParameter("extra_data", ED);
                                    NewItemId = Convert.ToInt32(dbClient.InsertQuery());

                                    dbClient.SetQuery("INSERT INTO `user_presents` (`item_id`,`base_id`,`extra_data`) VALUES ('" + NewItemId + "', '" + Reward + "', @extra_data)");
                                    dbClient.AddParameter("extra_data", (string.IsNullOrEmpty(ExtraData) ? "" : ExtraData));
                                    dbClient.RunQuery();
                                }

                                Session.GetHabbo().GetInventoryComponent().UpdateItems(true);

                                break;
                            }

                        case InteractionType.reward_box2:
                            {
                                string ED = Session.GetHabbo().Username + Convert.ToChar(5) + "Você escolheu um baú Legado de nível comum, pode obter raros que pesam entre 0 e 12 vips. Lembre-se de que eles estão disponíveis apenas por um certo tempo." + Convert.ToChar(5) + Session.GetHabbo().Id + Convert.ToChar(5) + Item.Data.Id + Convert.ToChar(5) + 206 + Convert.ToChar(5) + 1 + Convert.ToChar(5) + 1;
                                ExtraData = ED;
                                int NewItemId = 0;

                                int Reward = RandomNumber.GenerateRandom(1, 10);
                                #region Rewards
                                switch (Reward)
                                {
                                    case 1:
                                        Reward = 9501; // Humadera Azul Colorable - rare_colourable_scifirocket*1
                                        break;
                                    case 2:
                                        Reward = 9510; // Elefante Azul Colorable - rare_colourable_elephant_statue*1
                                        break;
                                    case 3:
                                        Reward = 1587; // Lámpara Calippo - ads_calip_lava
                                        break;
                                    case 4:
                                        Reward = 540004; // Alce Fiel - loyalty_elk
                                        break;
                                    case 5:
                                        Reward = 385; // Toldo Amarillo - marquee*4
                                        break;
                                    case 6:
                                        Reward = 9502; // Fontana Azul - rare_colourable_fountain*1
                                        break;
                                    case 7:
                                        Reward = 212; // VIP - club_sofa
                                        break;
                                    case 8:
                                        Reward = 9506; // Parasol Azul - rare_colourable_parasol*1
                                        break;
                                    case 9:
                                        Reward = 9514; // Puerta Laser Azul - rare_colourable_scifiport*1
                                        break;
                                    case 10:
                                        Reward = 353; // Humadera Rosa - scifirocket*4
                                        break;
                                }
                                #endregion

                                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("INSERT INTO `items` (`base_item`,`user_id`,`extra_data`) VALUES (9378, '" + Session.GetHabbo().Id + "', @extra_data)");
                                    dbClient.AddParameter("extra_data", ED);
                                    NewItemId = Convert.ToInt32(dbClient.InsertQuery());

                                    dbClient.SetQuery("INSERT INTO `user_presents` (`item_id`,`base_id`,`extra_data`) VALUES ('" + NewItemId + "', '" + Reward + "', @extra_data)");
                                    dbClient.AddParameter("extra_data", (string.IsNullOrEmpty(ExtraData) ? "" : ExtraData));
                                    dbClient.RunQuery();
                                }

                                Session.GetHabbo().GetInventoryComponent().UpdateItems(true);

                                break;
                            }

                        case InteractionType.TONER:
                            {
                                if (AmountPurchase > 1)
                                {
                                    List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase);

                                    if (Items != null)
                                    {
                                        GeneratedGenericItems.AddRange(Items);
                                        foreach (Item I in Items)
                                        {
                                            ItemFactory.CreateTonerData(I);

                                        }
                                    }
                                }
                                else
                                {
                                    NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData);

                                    if (NewItem != null)
                                    {
                                        GeneratedGenericItems.Add(NewItem);
                                        ItemFactory.CreateTonerData(NewItem);
                                    }
                                }
                            }
                            break;

                        case InteractionType.DEAL:
                            {
                                var DealItems = (from d in Page.Deals.Values.ToList() where d.Id == Item.Id select d);
                                foreach (CatalogDeal DealItem in DealItems.ToList())
                                {
                                    foreach (CatalogItem CatalogItem in DealItem.ItemDataList.ToList())
                                    {
                                        List<Item> Items = ItemFactory.CreateMultipleItems(CatalogItem.Data, Session.GetHabbo(), "", CatalogItem.Amount);

                                        if (Items != null)
                                        {

                                            GeneratedGenericItems.AddRange(Items);
                                        }
                                    }
                                }
                            }
                            break;
                    }

                    foreach (Item PurchasedItem in GeneratedGenericItems)
                    {
                        if (Session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                        {
                            Session.SendMessage(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                        }
                    }

                    break;

                case "e":
                    AvatarEffect Effect = null;

                    if (Session.GetHabbo().Effects().HasEffect(Item.Data.SpriteId))
                    {
                        Effect = Session.GetHabbo().Effects().GetEffectNullable(Item.Data.SpriteId);

                        if (Effect != null)
                        {
                            Effect.AddToQuantity();
                        }
                    }
                    else
                    {
                        Effect = AvatarEffectFactory.CreateNullable(Session.GetHabbo(), Item.Data.SpriteId, 3600);
                    }

                    if (Effect != null)// && Session.GetHabbo().Effects().TryAdd(Effect))
                    {
                        Session.SendMessage(new AvatarEffectAddedComposer(Item.Data.SpriteId, 3600));
                    }
                    break;

                case "r":
                    Bot Bot = BotUtility.CreateBot(Item.Data, Session.GetHabbo().Id);
                    if (Bot != null)
                    {
                        Session.GetHabbo().GetInventoryComponent().TryAddBot(Bot);
                        Session.SendMessage(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
                        Session.SendMessage(new FurniListNotificationComposer(Bot.Id, 5));
                    }
                    else
                    {
                        Session.SendNotification("Opa! Ocorreu um erro enquanto você estava comprando o Bot, aparentemente não há dados sobre ele, denuncie para a staff!!");
                    }

                    break;

                case "b":
                    {
                        Session.GetHabbo().GetBadgeComponent().GiveBadge(Item.Data.ItemName, true, Session);
                        Session.SendMessage(new FurniListNotificationComposer(0, 4));
                        break;
                    }

                case "p":
                    {
                        string[] PetData = ExtraData.Split('\n');

                        Pet Pet = PetUtility.CreatePet(Session.GetHabbo().Id, PetData[0], Item.Data.BehaviourData, PetData[1], PetData[2]);
                        if (Pet != null)
                        {
                            if (Session.GetHabbo().GetInventoryComponent().TryAddPet(Pet))
                            {
                                Pet.RoomId = 0;
                                Pet.PlacedInRoom = false;

                                Session.SendMessage(new FurniListNotificationComposer(Pet.PetId, 3));
                                Session.SendMessage(new PetInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetPets()));

                                if (StarBlueServer.GetGame().GetItemManager().GetItem(320, out ItemData petFood))
                                {
                                    Item Food = ItemFactory.CreateSingleItemNullable(petFood, Session.GetHabbo(), "", "");
                                    if (Food != null)
                                    {
                                        Session.GetHabbo().GetInventoryComponent().TryAddItem(Food);
                                        Session.SendMessage(new FurniListNotificationComposer(Food.Id, 1));
                                    }
                                }
                            }
                        }
                        break;
                    }
            }

            if (Item.Badge != string.Empty)
            {
                Session.GetHabbo().GetBadgeComponent().GiveBadge(Item.Badge, true, Session);
            }

            Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
            Session.SendMessage(new FurniListUpdateComposer());
        }

        public static string GenerateRainbowText(string Name)
        {
            StringBuilder NewName = new StringBuilder();

            string[] Colours = { "FF0000", "FFA500", "FFFF00", "008000", "0000FF", "800080" };

            int Count = 0;
            int Count2 = 0;
            while (Count < Name.Length)
            {
                NewName.Append("<font color='#" + Colours[Count2] + "'>" + Name[Count] + "</font>");

                Count++;
                Count2++;

                if (Count2 >= 6)
                {
                    Count2 = 0;
                }
            }

            return NewName.ToString();
        }

        public static string GeneratedoscoloresText(string Name)
        {
            StringBuilder NewName = new StringBuilder();

            string[] Colours = { "00527C", "000000", "00527C", "000000", "00527C", "000000" };

            int Count = 0;
            int Count2 = 0;
            while (Count < Name.Length)
            {
                NewName.Append("<font color='#" + Colours[Count2] + "'>" + Name[Count] + "</font>");

                Count++;
                Count2++;

                if (Count2 >= 6)
                {
                    Count2 = 0;
                }
            }

            return NewName.ToString();
        }

        public static string GeneratedoscoloresmasText(string Name)
        {
            StringBuilder NewName = new StringBuilder();

            string[] Colours = { "CF001C", "7B0010", "CF001C", "7B0010", "CF001C", "7B0010" };

            int Count = 0;
            int Count2 = 0;
            while (Count < Name.Length)
            {
                NewName.Append("<font color='#" + Colours[Count2] + "'>" + Name[Count] + "</font>");

                Count++;
                Count2++;

                if (Count2 >= 6)
                {
                    Count2 = 0;
                }
            }

            return NewName.ToString();
        }

        public static string GenerateColorRandomText(string Name)
        {
            StringBuilder NewName = new StringBuilder();

            string[] Colours = { "0080A6", "00516A", "0080A6", "00516A", "0080A6", "00516A" };

            int Count = 0;
            int Count2 = 0;
            while (Count < Name.Length)
            {
                NewName.Append("<font color='#" + Colours[Count2] + "'>" + Name[Count] + "</font>");

                Count++;
                Count2++;

                if (Count2 >= 6)
                {
                    Count2 = 0;
                }
            }

            return NewName.ToString();
        }

        public static string GeneraterojoynegroText(string Name)
        {
            StringBuilder NewName = new StringBuilder();

            string[] Colours = { "FF0000", "000000", "FF0000", "000000", "FF0000", "000000" };

            int Count = 0;
            int Count2 = 0;
            while (Count < Name.Length)
            {
                NewName.Append("<font color='#" + Colours[Count2] + "'>" + Name[Count] + "</font>");

                Count++;
                Count2++;

                if (Count2 >= 6)
                {
                    Count2 = 0;
                }
            }

            return NewName.ToString();
        }

        public static string GeneratemoradoynegroText(string Name)
        {
            StringBuilder NewName = new StringBuilder();

            string[] Colours = { "65009B", "000000", "65009B", "000000", "65009B", "000000" };

            int Count = 0;
            int Count2 = 0;
            while (Count < Name.Length)
            {
                NewName.Append("<font color='#" + Colours[Count2] + "'>" + Name[Count] + "</font>");

                Count++;
                Count2++;

                if (Count2 >= 6)
                {
                    Count2 = 0;
                }
            }

            return NewName.ToString();
        }

        public static string GenerateverdeynegroText(string Name)
        {
            StringBuilder NewName = new StringBuilder();

            string[] Colours = { "00742C", "000000", "00742C", "000000", "00742C", "000000" };

            int Count = 0;
            int Count2 = 0;
            while (Count < Name.Length)
            {
                NewName.Append("<font color='#" + Colours[Count2] + "'>" + Name[Count] + "</font>");

                Count++;
                Count2++;

                if (Count2 >= 6)
                {
                    Count2 = 0;
                }
            }

            return NewName.ToString();
        }

        public static string GeneraterosadoyrosadoText(string Name)
        {
            StringBuilder NewName = new StringBuilder();

            string[] Colours = { "B70092", "F12AC9", "B70092", "F12AC9", "B70092", "F12AC9" };

            int Count = 0;
            int Count2 = 0;
            while (Count < Name.Length)
            {
                NewName.Append("<font color='#" + Colours[Count2] + "'>" + Name[Count] + "</font>");

                Count++;
                Count2++;

                if (Count2 >= 6)
                {
                    Count2 = 0;
                }
            }

            return NewName.ToString();
        }

    }
}