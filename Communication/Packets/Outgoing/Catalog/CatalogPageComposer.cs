using StarBlue.HabboHotel.Catalog;
using StarBlue.HabboHotel.Catalog.Utilities;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    public class CatalogPageComposer : ServerPacket
    {
        public CatalogPageComposer(CatalogPage Page, string CataMode, GameClient Session)
            : base(ServerPacketHeader.CatalogPageMessageComposer)
        {
            base.WriteInteger(Page.Id);
            base.WriteString(CataMode);
            base.WriteString(Page.Template);

            base.WriteInteger(Page.PageStrings1.Count);
            foreach (string s in Page.PageStrings1)
            {
                base.WriteString(s);
            }

            base.WriteInteger(Page.PageStrings2.Count);
            foreach (string s in Page.PageStrings2)
            {
                base.WriteString(s);
            }

            if (!Page.Template.Equals("frontpage") && !Page.Template.Equals("club_buy") && Page.PageLink != "last_purchases")
            {

                base.WriteInteger(Page.Items.Count);
                foreach (CatalogItem Item in Page.Items.Values)
                {
                    base.WriteInteger(Item.Id);
                    base.WriteString(Item.Name);
                    base.WriteBoolean(false);//IsRentable
                    base.WriteInteger(Item.CostCredits);
                    if (Item.CostDiamonds > 0)
                    {
                        base.WriteInteger(Item.CostDiamonds);
                        base.WriteInteger(5); // Diamonds
                    }
                    else if (Item.CostPixels > 0)
                    {
                        base.WriteInteger(Item.CostPixels);
                        base.WriteInteger(0); // Type of PixelCost

                    }
                    else
                    {
                        base.WriteInteger(Item.CostGOTWPoints);
                        base.WriteInteger(103); // Gotw
                    }

                    base.WriteBoolean(Item.PredesignedId > 0 ? false : ItemUtility.CanGiftItem(Item));
                    if (Item.PredesignedId > 0)
                    {
                        base.WriteInteger(Page.PredesignedItems.Items.Count);
                        foreach (KeyValuePair<int, int> predesigned in Page.PredesignedItems.Items.ToList())
                        {
                            if (StarBlueServer.GetGame().GetItemManager().GetItem(predesigned.Key, out ItemData Data)) { }
                            base.WriteString(Data.Type.ToString());
                            base.WriteInteger(Data.SpriteId);
                            base.WriteString(string.Empty);
                            base.WriteInteger(predesigned.Value);
                            base.WriteBoolean(false);
                        }

                        base.WriteInteger(0);
                        base.WriteBoolean(false);
                        base.WriteBoolean(true); // Niu Rilí
                        base.WriteString(""); // Niu Rilí
                    }
                    else
                        if (Item.Data.InteractionType == InteractionType.DEAL)
                    {
                        foreach (CatalogDeal Deal in Page.Deals.Values)
                        {
                            base.WriteInteger(Deal.ItemDataList.Count);//Count

                            foreach (CatalogItem DealItem in Deal.ItemDataList.ToList())
                            {
                                base.WriteString(DealItem.Data.Type.ToString());
                                base.WriteInteger(DealItem.Data.SpriteId);
                                base.WriteString("");
                                base.WriteInteger(DealItem.Amount);
                                base.WriteBoolean(false);
                            }
                            base.WriteInteger(0);//club_level
                            base.WriteBoolean(ItemUtility.CanSelectAmount(Item));
                            base.WriteBoolean(true);
                            base.WriteString("");
                        }
                    }
                    else
                    {
                        base.WriteInteger(string.IsNullOrEmpty(Item.Badge) ? 1 : 2);//Count 1 item if there is no badge, otherwise count as 2.

                        if (!string.IsNullOrEmpty(Item.Badge))
                        {
                            base.WriteString("b");
                            base.WriteString(Item.Badge);
                        }

                        base.WriteString(Item.Data.Type.ToString());
                        if (Item.Data.Type.ToString().ToLower() == "b")
                        {
                            //This is just a badge, append the name.
                            base.WriteString(Item.Data.ItemName);
                        }
                        else
                        {
                            base.WriteInteger(Item.Data.SpriteId);
                            if (Item.Data.InteractionType == InteractionType.WALLPAPER || Item.Data.InteractionType == InteractionType.FLOOR || Item.Data.InteractionType == InteractionType.LANDSCAPE)
                            {
                                base.WriteString(Item.Name.Split('_')[2]);
                            }
                            else if (Item.Data.InteractionType == InteractionType.BOT)//Bots
                            {
                                if (!StarBlueServer.GetGame().GetCatalog().TryGetBot(Item.ItemId, out CatalogBot CatalogBot))
                                {
                                    base.WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                                }
                                else
                                {
                                    base.WriteString(CatalogBot.Figure);
                                }
                            }
                            else if (Item.ExtraData != null)
                            {
                                base.WriteString(Item.ExtraData != null ? Item.ExtraData : string.Empty);
                            }
                            base.WriteInteger(Item.Amount);
                            base.WriteBoolean(Item.IsLimited); // IsLimited
                            if (Item.IsLimited)
                            {
                                base.WriteInteger(Item.LimitedEditionSells);
                                base.WriteInteger(Item.LimitedEditionStack - Item.LimitedEditionSells);
                            }
                        }
                        base.WriteInteger(0); //club_level
                        base.WriteBoolean(ItemUtility.CanSelectAmount(Item));

                        base.WriteBoolean(true); // Niu Rilí
                        base.WriteString(""); // Niu Rilí
                    }

                }
            }

            else if (!Page.Template.Equals("frontpage") && !Page.Template.Equals("club_buy") && Page.PageLink == "last_purchases")
            {
                base.WriteInteger(Session.GetHabbo().LastPurchasesItems.Count());
                foreach (KeyValuePair<int, CatalogItem> Item in Session.GetHabbo().LastPurchasesItems.ToList())
                {
                    base.WriteInteger(Item.Value.Id);
                    base.WriteString(Item.Value.Name);
                    base.WriteBoolean(false);
                    base.WriteInteger(Item.Value.CostCredits);

                    if (Item.Value.CostDiamonds > 0)
                    {
                        base.WriteInteger(Item.Value.CostDiamonds);
                        base.WriteInteger(5); // Diamonds
                    }
                    else if (Item.Value.CostGOTWPoints > 0)
                    {
                        base.WriteInteger(Item.Value.CostGOTWPoints);
                        base.WriteInteger(103); // Pixeles
                    }
                    else
                    {
                        base.WriteInteger(Item.Value.CostPixels);
                        base.WriteInteger(0); // Type of PixelCost
                    }
                    base.WriteBoolean(false);
                    base.WriteInteger(string.IsNullOrEmpty(Item.Value.Badge) ? 1 : 2);//Count 1 item if there is no badge, otherwise count as 2.

                    if (!string.IsNullOrEmpty(Item.Value.Badge))
                    {
                        base.WriteString("b");
                        base.WriteString(Item.Value.Badge);
                    }

                    base.WriteString(Item.Value.Data.Type.ToString());
                    if (Item.Value.Data.Type.ToString().ToLower() == "b")
                    {
                        //This is just a badge, append the name.
                        base.WriteString(Item.Value.Data.ItemName);
                    }
                    else
                    {
                        base.WriteInteger(Item.Value.Data.SpriteId);
                        if (Item.Value.Data.InteractionType == InteractionType.WALLPAPER || Item.Value.Data.InteractionType == InteractionType.FLOOR || Item.Value.Data.InteractionType == InteractionType.LANDSCAPE)
                        {
                            base.WriteString(Item.Value.Name.Split('_')[2]);
                        }
                        else if (Item.Value.Data.InteractionType == InteractionType.BOT)//Bots
                        {
                            if (!StarBlueServer.GetGame().GetCatalog().TryGetBot(Item.Value.ItemId, out CatalogBot CatalogBot))
                            {
                                base.WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                            }
                            else
                            {
                                base.WriteString(CatalogBot.Figure);
                            }
                        }
                        else if (Item.Value.ExtraData != null)
                        {
                            base.WriteString(Item.Value.ExtraData != null ? Item.Value.ExtraData : string.Empty);
                        }
                        base.WriteInteger(Item.Value.Amount);
                        base.WriteBoolean(Item.Value.IsLimited);
                        if (Item.Value.IsLimited)
                        {
                            base.WriteInteger(Item.Value.LimitedEditionSells);
                            base.WriteInteger(Item.Value.LimitedEditionStack - Item.Value.LimitedEditionSells);
                        }
                    }

                    base.WriteInteger(0);
                    base.WriteBoolean(false);
                    base.WriteBoolean(true);
                    base.WriteString("");
                }

            }
            else
            {
                base.WriteInteger(0);
            }

            base.WriteInteger(-1);
            base.WriteBoolean(false);



            if (Page.Template == "frontpage4" && CataMode == "NORMAL")
            {
                ICollection<Frontpage> FrontPage = StarBlueServer.GetGame().GetCatalogFrontPageManager().GetCatalogFrontPage();
                base.WriteInteger(FrontPage.Count); // count

                foreach (Frontpage front in FrontPage.ToList<Frontpage>())
                {
                    base.WriteInteger(front.Id());
                    base.WriteString(front.FrontName());
                    base.WriteString(front.FrontImage());
                    base.WriteInteger(0);
                    base.WriteString(front.FrontLink());
                    base.WriteInteger(-1);

                }
            }

        }
    }
}