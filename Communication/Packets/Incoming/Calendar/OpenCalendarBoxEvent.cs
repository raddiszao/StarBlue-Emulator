using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Campaigns;
using StarBlue.Communication.Packets.Outgoing.Inventory.Furni;
using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Communication.Packets.Outgoing.Users;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;

namespace StarBlue.Communication.Packets.Incoming.Calendar
{
    class OpenCalendarBoxEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string CampaignName = Packet.PopString();
            int CampaignDay = Packet.PopInt(); // INDEX VALUE.

            // Si no es el nombre de campaña actual.
            if (CampaignName != StarBlueServer.GetGame().GetCalendarManager().GetCampaignName())
            {
                return;
            }

            // Si es un día inválido.
            if (CampaignDay < 0 || CampaignDay > StarBlueServer.GetGame().GetCalendarManager().GetTotalDays() - 1 || CampaignDay < StarBlueServer.GetGame().GetCalendarManager().GetUnlockDays())
            {
                // Mini fix
                return;
            }



            // Días próximos
            if (CampaignDay > StarBlueServer.GetGame().GetCalendarManager().GetUnlockDays())
            {
                return;
            }


            // Esta recompensa ya ha sido recogida.
            if (Session.GetHabbo().calendarGift[CampaignDay])
            {
                return;
            }

            Session.GetHabbo().calendarGift[CampaignDay] = true;

            // PACKET PARA ACTUALIZAR?
            Session.SendMessage(new CalendarPrizesComposer(StarBlueServer.GetGame().GetCalendarManager().GetCampaignDay(CampaignDay + 1)));
            Session.SendMessage(new CampaignCalendarDataComposer(Session.GetHabbo().calendarGift));


            string Gift = StarBlueServer.GetGame().GetCalendarManager().GetGiftByDay(CampaignDay + 1);
            string GiftType = Gift.Split(':')[0];
            string GiftValue = Gift.Split(':')[1];

            switch (GiftType.ToLower())
            {
                case "itemid":
                    {
                        if (!StarBlueServer.GetGame().GetItemManager().GetItem(int.Parse(GiftValue), out ItemData Item))
                        {
                            // No existe este ItemId.
                            return;
                        }

                        Item GiveItem = ItemFactory.CreateSingleItemNullable(Item, Session.GetHabbo(), "", "");
                        if (GiveItem != null)
                        {
                            Session.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);

                            Session.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));
                            Session.SendMessage(new FurniListUpdateComposer());
                        }

                        Session.GetHabbo().GetInventoryComponent().UpdateItems(false);
                    }
                    break;

                case "badge":
                    {
                        Session.GetHabbo().GetBadgeComponent().GiveBadge(GiftValue, true, Session);
                    }
                    break;

                case "diamonds":
                    {
                        Session.GetHabbo().Diamonds += int.Parse(GiftValue);
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
                    }
                    break;

                case "gotwpoints":
                    {
                        Session.GetHabbo().GOTWPoints += int.Parse(GiftValue);
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
                    }
                    break;

                case "vip":
                    {
                        var IsVIP = Session.GetHabbo().GetClubManager().HasSubscription("club_vip");
                        if (IsVIP)
                        {
                            Session.SendMessage(new AlertNotificationHCMessageComposer(4));
                        }
                        else
                        {
                            Session.SendMessage(new AlertNotificationHCMessageComposer(5));
                        }
                        if (Session.GetHabbo().Rank > 2)
                        {
                            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.RunFastQuery("UPDATE `users` SET `rank_vip` = '1' WHERE `user_id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                            }
                        }
                        else
                        {
                            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.RunFastQuery("UPDATE `users` SET `rank` = '2' WHERE `user_id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                                dbClient.RunFastQuery("UPDATE `users` SET `rank_vip` = '1' WHERE `user_id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                            }
                        }

                        Session.GetHabbo().GetClubManager().AddOrExtendSubscription("club_vip", int.Parse(GiftValue) * 24 * 3600, Session);
                        Session.GetHabbo().GetBadgeComponent().GiveBadge("VIP", true, Session);

                        StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_VipClub", 1);
                        Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    }
                    break;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("INSERT INTO user_campaign_gifts VALUES (NULL, '" + Session.GetHabbo().Id + "','" + CampaignName + "','" + (CampaignDay + 1) + "')");
            }
        }
    }
}
