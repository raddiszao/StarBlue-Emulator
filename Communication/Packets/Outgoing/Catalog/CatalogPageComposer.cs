using StarBlue.HabboHotel.Catalog;
using StarBlue.HabboHotel.Catalog.Utilities;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    public class CatalogPageComposer : MessageComposer
    {
        private CatalogPage Page { get; }
        private string CataMode { get; }
        private GameClient Session { get; }

        public CatalogPageComposer(CatalogPage Page, string CataMode, GameClient Session)
            : base(Composers.CatalogPageMessageComposer)
        {
            this.Page = Page;
            this.CataMode = CataMode;
            this.Session = Session;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Page.Id);
            packet.WriteString(CataMode);
            packet.WriteString(Page.Template);

            packet.WriteInteger(Page.PageStrings1.Count);
            foreach (string s in Page.PageStrings1)
            {
                packet.WriteString(s);
            }

            packet.WriteInteger(Page.PageStrings2.Count);
            foreach (string s in Page.PageStrings2)
            {
                packet.WriteString(s);
            }

            if (!Page.Template.Equals("frontpage") && !Page.Template.Equals("club_buy") && Page.PageLink != "last_purchases")
            {

                packet.WriteInteger(Page.Items.Count);
                foreach (CatalogItem Item in Page.Items.Values)
                {
                    packet.WriteInteger(Item.Id);
                    packet.WriteString(Item.Name);
                    packet.WriteBoolean(false);//IsRentable
                    packet.WriteInteger(Item.CostCredits);
                    if (Item.CostDiamonds > 0)
                    {
                        packet.WriteInteger(Item.CostDiamonds);
                        packet.WriteInteger(5); // Diamonds
                    }
                    else if (Item.CostPixels > 0)
                    {
                        packet.WriteInteger(Item.CostPixels);
                        packet.WriteInteger(0); // Type of PixelCost

                    }
                    else
                    {
                        packet.WriteInteger(Item.CostGOTWPoints);
                        packet.WriteInteger(103); // Gotw
                    }

                    packet.WriteBoolean(Item.PredesignedId > 0 ? false : ItemUtility.CanGiftItem(Item));
                    if (Item.PredesignedId > 0)
                    {
                        packet.WriteInteger(Page.PredesignedItems.Items.Count);
                        foreach (KeyValuePair<int, int> predesigned in Page.PredesignedItems.Items.ToList())
                        {
                            if (StarBlueServer.GetGame().GetItemManager().GetItem(predesigned.Key, out ItemData Data)) { }
                            packet.WriteString(Data.Type.ToString());
                            packet.WriteInteger(Data.SpriteId);
                            packet.WriteString(string.Empty);
                            packet.WriteInteger(predesigned.Value);
                            packet.WriteBoolean(false);
                        }

                        packet.WriteInteger(0);
                        packet.WriteBoolean(false);
                        packet.WriteBoolean(true); // Niu Rilí
                        packet.WriteString(""); // Niu Rilí
                    }
                    else
                        if (Item.Data.InteractionType == InteractionType.DEAL)
                    {
                        foreach (CatalogDeal Deal in Page.Deals.Values)
                        {
                            packet.WriteInteger(Deal.ItemDataList.Count);//Count

                            foreach (CatalogItem DealItem in Deal.ItemDataList.ToList())
                            {
                                packet.WriteString(DealItem.Data.Type.ToString());
                                packet.WriteInteger(DealItem.Data.SpriteId);
                                packet.WriteString("");
                                packet.WriteInteger(DealItem.Amount);
                                packet.WriteBoolean(false);
                            }
                            packet.WriteInteger(0);//club_level
                            packet.WriteBoolean(ItemUtility.CanSelectAmount(Item));
                            packet.WriteBoolean(true);
                            packet.WriteString("");
                        }
                    }
                    else
                    {
                        packet.WriteInteger(string.IsNullOrEmpty(Item.Badge) ? 1 : 2);//Count 1 item if there is no badge, otherwise count as 2.

                        if (!string.IsNullOrEmpty(Item.Badge))
                        {
                            packet.WriteString("b");
                            packet.WriteString(Item.Badge);
                        }

                        packet.WriteString(Item.Data.Type.ToString());
                        if (Item.Data.Type.ToString().ToLower() == "b")
                        {
                            //This is just a badge, append the name.
                            packet.WriteString(Item.Data.ItemName);
                        }
                        else
                        {
                            packet.WriteInteger(Item.Data.SpriteId);
                            if (Item.Data.InteractionType == InteractionType.WALLPAPER || Item.Data.InteractionType == InteractionType.FLOOR || Item.Data.InteractionType == InteractionType.LANDSCAPE)
                            {
                                packet.WriteString(Item.Name.Split('_')[2]);
                            }
                            else if (Item.Data.InteractionType == InteractionType.BOT)//Bots
                            {
                                if (!StarBlueServer.GetGame().GetCatalog().TryGetBot(Item.ItemId, out CatalogBot CatalogBot))
                                {
                                    packet.WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                                }
                                else
                                {
                                    packet.WriteString(CatalogBot.Figure);
                                }
                            }
                            else if (Item.ExtraData != null)
                            {
                                packet.WriteString(Item.ExtraData != null ? Item.ExtraData : string.Empty);
                            }
                            packet.WriteInteger(Item.Amount);
                            packet.WriteBoolean(Item.IsLimited); // IsLimited
                            if (Item.IsLimited)
                            {
                                packet.WriteInteger(Item.LimitedEditionSells);
                                packet.WriteInteger(Item.LimitedEditionStack - Item.LimitedEditionSells);
                            }
                        }
                        packet.WriteInteger(0); //club_level
                        packet.WriteBoolean(ItemUtility.CanSelectAmount(Item));

                        packet.WriteBoolean(true); // Niu Rilí
                        packet.WriteString(""); // Niu Rilí
                    }

                }
            }

            else if (!Page.Template.Equals("frontpage") && !Page.Template.Equals("club_buy") && Page.PageLink == "last_purchases")
            {
                packet.WriteInteger(Session.GetHabbo().LastPurchasesItems.Count());
                foreach (KeyValuePair<int, CatalogItem> Item in Session.GetHabbo().LastPurchasesItems.ToList())
                {
                    packet.WriteInteger(Item.Value.Id);
                    packet.WriteString(Item.Value.Name);
                    packet.WriteBoolean(false);
                    packet.WriteInteger(Item.Value.CostCredits);

                    if (Item.Value.CostDiamonds > 0)
                    {
                        packet.WriteInteger(Item.Value.CostDiamonds);
                        packet.WriteInteger(5); // Diamonds
                    }
                    else if (Item.Value.CostGOTWPoints > 0)
                    {
                        packet.WriteInteger(Item.Value.CostGOTWPoints);
                        packet.WriteInteger(103); // Pixeles
                    }
                    else
                    {
                        packet.WriteInteger(Item.Value.CostPixels);
                        packet.WriteInteger(0); // Type of PixelCost
                    }
                    packet.WriteBoolean(false);
                    packet.WriteInteger(string.IsNullOrEmpty(Item.Value.Badge) ? 1 : 2);//Count 1 item if there is no badge, otherwise count as 2.

                    if (!string.IsNullOrEmpty(Item.Value.Badge))
                    {
                        packet.WriteString("b");
                        packet.WriteString(Item.Value.Badge);
                    }

                    packet.WriteString(Item.Value.Data.Type.ToString());
                    if (Item.Value.Data.Type.ToString().ToLower() == "b")
                    {
                        //This is just a badge, append the name.
                        packet.WriteString(Item.Value.Data.ItemName);
                    }
                    else
                    {
                        packet.WriteInteger(Item.Value.Data.SpriteId);
                        if (Item.Value.Data.InteractionType == InteractionType.WALLPAPER || Item.Value.Data.InteractionType == InteractionType.FLOOR || Item.Value.Data.InteractionType == InteractionType.LANDSCAPE)
                        {
                            packet.WriteString(Item.Value.Name.Split('_')[2]);
                        }
                        else if (Item.Value.Data.InteractionType == InteractionType.BOT)//Bots
                        {
                            if (!StarBlueServer.GetGame().GetCatalog().TryGetBot(Item.Value.ItemId, out CatalogBot CatalogBot))
                            {
                                packet.WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                            }
                            else
                            {
                                packet.WriteString(CatalogBot.Figure);
                            }
                        }
                        else if (Item.Value.ExtraData != null)
                        {
                            packet.WriteString(Item.Value.ExtraData != null ? Item.Value.ExtraData : string.Empty);
                        }
                        packet.WriteInteger(Item.Value.Amount);
                        packet.WriteBoolean(Item.Value.IsLimited);
                        if (Item.Value.IsLimited)
                        {
                            packet.WriteInteger(Item.Value.LimitedEditionSells);
                            packet.WriteInteger(Item.Value.LimitedEditionStack - Item.Value.LimitedEditionSells);
                        }
                    }

                    packet.WriteInteger(0);
                    packet.WriteBoolean(false);
                    packet.WriteBoolean(true);
                    packet.WriteString("");
                }

            }
            else
            {
                packet.WriteInteger(0);
            }

            packet.WriteInteger(-1);
            packet.WriteBoolean(false);



            if (Page.Template == "frontpage4" && CataMode == "NORMAL")
            {
                ICollection<Frontpage> FrontPage = StarBlueServer.GetGame().GetCatalogFrontPageManager().GetCatalogFrontPage();
                packet.WriteInteger(FrontPage.Count); // count

                foreach (Frontpage front in FrontPage.ToList<Frontpage>())
                {
                    packet.WriteInteger(front.Id());
                    packet.WriteString(front.FrontName());
                    packet.WriteString(front.FrontImage());
                    packet.WriteInteger(0);
                    packet.WriteString(front.FrontLink());
                    packet.WriteInteger(-1);

                }
            }

        }
    }
}