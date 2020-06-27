using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Catalog;
using StarBlue.Communication.Packets.Outgoing.Inventory.Furni;
using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.HabboHotel.Catalog;
using StarBlue.HabboHotel.Catalog.Utilities;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Items.Utilities;
using StarBlue.HabboHotel.Quests;
using StarBlue.HabboHotel.Users;
using StarBlue.Utilities;
using System;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    public class PurchaseFromCatalogAsGiftEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int PageId = Packet.PopInt();
            int ItemId = Packet.PopInt();
            string Data = Packet.PopString();
            string GiftUser = StringCharFilter.Escape(Packet.PopString());
            string GiftMessage = StringCharFilter.Escape(Packet.PopString().Replace(Convert.ToChar(5), ' '));
            int SpriteId = Packet.PopInt();
            int Ribbon = Packet.PopInt();
            int Colour = Packet.PopInt();
            bool dnow = Packet.PopBoolean();

            if (StarBlueServer.GetSettingsManager().TryGetValue("gifts_enabled") != "1")
            {
                Session.SendNotification("O administrador desativou os presentes do hotel.");
                return;
            }

            if (!StarBlueServer.GetGame().GetCatalog().TryGetPage(PageId, out CatalogPage Page))
            {
                return;
            }

            if (Session.GetHabbo().Rank > 3 && !Session.GetHabbo().StaffOk)
            {
                return;
            }

            if (!Page.Enabled || !Page.Visible || Page.MinimumRank > Session.GetHabbo().Rank || (Page.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 2))
            {
                return;
            }

            if (!Page.Items.TryGetValue(ItemId, out CatalogItem Item))
            {
                if (Page.ItemOffers.ContainsKey(ItemId))
                {
                    Item = Page.ItemOffers[ItemId];
                    if (Item == null)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            if (!ItemUtility.CanGiftItem(Item))
            {
                return;
            }

            if (!StarBlueServer.GetGame().GetItemManager().GetGift(SpriteId, out ItemData PresentData) || PresentData.InteractionType != InteractionType.GIFT)
            {
                return;
            }

            if (Session.GetHabbo().Credits < Item.CostCredits)
            {
                Session.SendMessage(new PresentDeliverErrorMessageComposer(true, false));
                return;
            }

            if (Session.GetHabbo().Duckets < Item.CostPixels)
            {
                Session.SendMessage(new PresentDeliverErrorMessageComposer(false, true));
                return;
            }

            Habbo Habbo = StarBlueServer.GetHabboByUsername(GiftUser);
            if (Habbo == null)
            {
                Session.SendMessage(new GiftWrappingErrorComposer());
                return;
            }

            if (!Habbo.AllowGifts)
            {
                Session.SendNotification("Oops, este usuario no permite recibir regalos!");
                return;
            }

            if (Session.GetHabbo().Rank < 4)
            {
                if ((DateTime.Now - Session.GetHabbo().LastGiftPurchaseTime).TotalSeconds <= 15.0)
                {
                    Session.SendNotification("Estas enviando regalos muy rapido, espere un minimo de 15 segundos para enviar el siguiente!");

                    Session.GetHabbo().GiftPurchasingWarnings += 1;
                    if (Session.GetHabbo().GiftPurchasingWarnings >= 25)
                    {
                        Session.GetHabbo().SessionGiftBlocked = true;
                    }

                    return;
                }
            }


            if (Session.GetHabbo().SessionGiftBlocked)
            {
                return;
            }

            string ED = GiftUser + Convert.ToChar(5) + GiftMessage + Convert.ToChar(5) + Session.GetHabbo().Id + Convert.ToChar(5) + Item.Data.Id + Convert.ToChar(5) + SpriteId + Convert.ToChar(5) + Ribbon + Convert.ToChar(5) + Colour;

            int NewItemId = 0;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                //Insert the dummy item.
                dbClient.SetQuery("INSERT INTO `items` (`base_item`,`user_id`,`extra_data`) VALUES ('" + PresentData.Id + "', '" + Habbo.Id + "', @extra_data)");
                dbClient.AddParameter("extra_data", ED);
                NewItemId = Convert.ToInt32(dbClient.InsertQuery());

                string ItemExtraData = null;
                switch (Item.Data.InteractionType)
                {
                    case InteractionType.NONE:
                        ItemExtraData = "";
                        break;

                    #region Pet handling

                    case InteractionType.PET:

                        try
                        {
                            string[] Bits = Data.Split('\n');
                            string PetName = Bits[0];
                            string Race = Bits[1];
                            string Color = Bits[2];

                            int.Parse(Race);

                            if (PetUtility.CheckPetName(PetName))
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
                        catch
                        {
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
                            if (string.IsNullOrEmpty(Data))
                            {
                                Number = 0;
                            }
                            else
                            {
                                Number = Double.Parse(Data, StarBlueServer.CultureInfo);
                            }
                        }
                        catch
                        {

                        }

                        ItemExtraData = Number.ToString().Replace(',', '.');
                        break; // maintain extra data // todo: validate

                    case InteractionType.POSTIT:
                        ItemExtraData = "FFFF33";
                        break;

                    case InteractionType.MOODLIGHT:
                        ItemExtraData = "1,1,1,#000000,255";
                        break;

                    case InteractionType.TROPHY:
                        ItemExtraData = Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) + Data;
                        break;

                    case InteractionType.MANNEQUIN:
                        ItemExtraData = "m" + Convert.ToChar(5) + ".ch-210-1321.lg-285-92" + Convert.ToChar(5) + "Default Maniqui";
                        break;

                    case InteractionType.BADGE_DISPLAY:
                        if (!Session.GetHabbo().GetBadgeComponent().HasBadge(Data))
                        {
                            Session.SendMessage(new BroadcastMessageAlertComposer("Opa, aparentemente você não possui este fórum."));
                            return;
                        }

                        ItemExtraData = Data + Convert.ToChar(9) + Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                        break;

                    default:
                        ItemExtraData = Data;
                        break;
                }

                //Insert the present, forever.
                dbClient.SetQuery("INSERT INTO `user_presents` (`item_id`,`base_id`,`extra_data`) VALUES ('" + NewItemId + "', '" + Item.Data.Id + "', @extra_data)");
                dbClient.AddParameter("extra_data", (string.IsNullOrEmpty(ItemExtraData) ? "" : ItemExtraData));
                dbClient.RunQuery();

                //Here we're clearing up a record, this is dumb, but okay.
                dbClient.RunFastQuery("DELETE FROM `items` WHERE `id` = " + NewItemId + " LIMIT 1;");
            }


            Item GiveItem = ItemFactory.CreateGiftItem(PresentData, Habbo, ED, ED, NewItemId, 0, 0);
            if (GiveItem != null)
            {
                GameClient Receiver = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Habbo.Id);
                if (Receiver != null)
                {
                    if (Receiver.GetHabbo().Rank <= 5)
                    {

                        Receiver.SendNotification("Você acabou de receber um presente de " + Session.GetHabbo().Username);

                    }
                    {

                        Receiver.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);
                        Receiver.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));
                        Receiver.SendMessage(new PurchaseOKComposer());
                        Receiver.SendMessage(new FurniListAddComposer(GiveItem));
                        Receiver.SendMessage(new FurniListUpdateComposer());

                    }
                }
                StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_GiftGiver", 1);
                StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Receiver, "ACH_GiftReceiver", 1);
                StarBlueServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.GIFT_OTHERS);
                Session.SendNotification("Ha enviado correctamente un regalo");

            }

            Session.SendMessage(new PurchaseOKComposer(Item, PresentData));

            if (Item.CostCredits > 0)
            {
                Session.GetHabbo().Credits -= Item.CostCredits;
                Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
            }

            if (Item.CostPixels > 0)
            {
                Session.GetHabbo().Duckets -= Item.CostPixels;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));
            }

            Session.GetHabbo().LastGiftPurchaseTime = DateTime.Now;
        }
    }
}