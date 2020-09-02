using StarBlue.HabboHotel.Catalog;
using StarBlue.HabboHotel.GameClients;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    public class CatalogIndexComposer : ServerPacket
    {
        public CatalogIndexComposer(GameClient Session, ICollection<CatalogPage> Pages, string Mode, int Sub = 0)
            : base(ServerPacketHeader.CatalogIndexMessageComposer)
        {
            WriteRootIndex(Session, Pages);
            foreach (CatalogPage Parent in Pages)
            {
                if (Parent.ParentId != -1 || (Parent.MinimumRank > Session.GetHabbo().Rank && Parent.MinimumVIP == 0) || (Parent.MinimumVIP > 0 && Parent.MinimumVIP > Session.GetHabbo().VIPRank && Parent.MinimumRank > Session.GetHabbo().Rank))
                {
                    continue;
                }

                WritePage(Session, Parent, CalcTreeSize(Session, Pages, Parent.Id));
                foreach (CatalogPage child in Pages)
                {
                    if (child.ParentId != Parent.Id || (child.MinimumRank > Session.GetHabbo().Rank && child.MinimumVIP == 0) || (child.MinimumVIP > 0 && child.MinimumVIP > Session.GetHabbo().VIPRank && child.MinimumRank > Session.GetHabbo().Rank))
                    {
                        continue;
                    }

                    if (child.Enabled)
                    {
                        WritePage(Session, child, CalcTreeSize(Session, Pages, child.Id));
                    }
                    else
                    {
                        WriteNodeIndex(Session, child, CalcTreeSize(Session, Pages, child.Id));
                    }

                    foreach (CatalogPage SubChild in Pages)
                    {
                        if (SubChild.ParentId != child.Id || (SubChild.MinimumRank > Session.GetHabbo().Rank && SubChild.MinimumVIP == 0) || (SubChild.MinimumVIP > 0 && SubChild.MinimumVIP > Session.GetHabbo().VIPRank && SubChild.MinimumRank > Session.GetHabbo().Rank))
                        {
                            continue;
                        }

                        if (SubChild.Enabled)
                        {
                            WritePage(Session, SubChild, CalcTreeSize(Session, Pages, SubChild.Id));
                        }
                        else
                        {
                            WriteNodeIndex(Session, SubChild, CalcTreeSize(Session, Pages, SubChild.Id));
                        }

                        foreach (CatalogPage SubSChild in Pages)
                        {
                            if (SubSChild.ParentId != SubChild.Id || (SubSChild.MinimumRank > Session.GetHabbo().Rank && SubSChild.MinimumVIP == 0) || (SubSChild.MinimumVIP > 0 && SubSChild.MinimumVIP > Session.GetHabbo().VIPRank && !(SubSChild.MinimumRank > Session.GetHabbo().Rank)))
                            {
                                continue;
                            }

                            WritePage(Session, SubSChild, 0);
                        }
                    }
                }
            }

            base.WriteBoolean(false);
            base.WriteString(Mode);
        }

        public CatalogIndexComposer(GameClient Session, ICollection<BCCatalogPage> Pages, string Mode, int Sub = 0)
            : base(ServerPacketHeader.CatalogIndexMessageComposer)
        {
            WriteRootIndex(Session, Pages);
            foreach (BCCatalogPage Parent in Pages)
            {
                if (Parent.ParentId != -1 || Parent.MinimumRank > Session.GetHabbo().Rank || (Parent.MinimumVIP < Session.GetHabbo().VIPRank))
                {
                    continue;
                }

                WritePage(Parent, CalcTreeSize(Session, Pages, Parent.Id));
                foreach (BCCatalogPage child in Pages)
                {
                    if (child.ParentId != Parent.Id || child.MinimumRank > Session.GetHabbo().Rank || (child.MinimumVIP < Session.GetHabbo().VIPRank))
                    {
                        continue;
                    }

                    if (child.Enabled)
                    {
                        WritePage(child, CalcTreeSize(Session, Pages, child.Id));
                    }
                    else
                    {
                        WriteNodeIndex(child, CalcTreeSize(Session, Pages, child.Id));
                    }

                    foreach (BCCatalogPage SubChild in Pages)
                    {
                        if (SubChild.ParentId != child.Id || SubChild.MinimumRank > Session.GetHabbo().Rank)
                        {
                            continue;
                        }

                        WritePage(SubChild, 0);
                    }
                }
            }
            base.WriteBoolean(false);
            base.WriteString(Mode);
        }
        public void WriteRootIndex(GameClient session, ICollection<CatalogPage> pages)
        {
            base.WriteBoolean(true);
            base.WriteInteger(0);
            base.WriteInteger(-1);
            base.WriteString("root");
            base.WriteString(string.Empty);
            base.WriteInteger(0);
            base.WriteInteger(CalcTreeSize(session, pages, -1));
        }
        public void WriteNodeIndex(GameClient Session, CatalogPage page, int treeSize)
        {
            string Caption = page.Caption;
            if (!treeSize.Equals(0) && Session.GetHabbo().Rank >= 14)
            {
                Caption += " (" + treeSize + ")";
            }

            base.WriteBoolean(page.Visible);
            base.WriteInteger(page.Icon);
            base.WriteInteger(-1);
            base.WriteString(page.PageLink);
            base.WriteString(Caption);
            base.WriteInteger(0);
            base.WriteInteger(treeSize);
        }
        public void WritePage(GameClient Session, CatalogPage page, int treeSize)
        {
            string Caption = page.Caption;
            if (!treeSize.Equals(0) && Session.GetHabbo().Rank >= 14)
            {
                Caption += " (" + treeSize + ")";
            }

            base.WriteBoolean(page.Visible);
            base.WriteInteger(page.Icon);
            base.WriteInteger(page.Id);
            base.WriteString(page.PageLink);
            base.WriteString(Caption);
            base.WriteInteger(page.ItemOffers.Count);
            foreach (int i in page.ItemOffers.Keys)
            {
                base.WriteInteger(i);
            }
            base.WriteInteger(treeSize);
        }

        public int CalcTreeSize(GameClient Session, ICollection<CatalogPage> Pages, int ParentId)
        {
            int i = 0;
            foreach (CatalogPage Page in Pages)
            {
                if (Page.ParentId != ParentId || (Page.MinimumRank > Session.GetHabbo().Rank && Page.MinimumVIP == 0) || (Page.MinimumVIP > 0 && Page.MinimumVIP > Session.GetHabbo().VIPRank && Page.MinimumRank > Session.GetHabbo().Rank))
                {
                    continue;
                }

                if (Page.ParentId == ParentId)
                {
                    i++;
                }
            }
            return i;
        }
        public void WriteRootIndex(GameClient session, ICollection<BCCatalogPage> pages)
        {
            base.WriteBoolean(true);
            base.WriteInteger(0);
            base.WriteInteger(-1);
            base.WriteString("root");
            base.WriteString(string.Empty);
            base.WriteInteger(0);
            base.WriteInteger(CalcTreeSize(session, pages, -1));
        }
        public void WriteNodeIndex(BCCatalogPage page, int treeSize)
        {
            base.WriteBoolean(page.Visible);
            base.WriteInteger(page.Icon);
            base.WriteInteger(-1);
            base.WriteString(page.PageLink);
            base.WriteString(page.Caption);
            base.WriteInteger(0);
            base.WriteInteger(treeSize);
        }
        public void WritePage(BCCatalogPage page, int treeSize)
        {
            base.WriteBoolean(page.Visible);
            base.WriteInteger(page.Icon);
            base.WriteInteger(page.Id);
            base.WriteString(page.PageLink);
            base.WriteString(page.Caption);
            base.WriteInteger(page.ItemOffers.Count);
            foreach (int i in page.ItemOffers.Keys)
            {
                base.WriteInteger(i);
            }
            base.WriteInteger(treeSize);
        }
        public int CalcTreeSize(GameClient Session, ICollection<BCCatalogPage> Pages, int ParentId)
        {
            int i = 0;
            foreach (BCCatalogPage Page in Pages)
            {
                if (Page.MinimumRank > Session.GetHabbo().Rank || (Page.MinimumVIP < Session.GetHabbo().VIPRank) || Page.ParentId != ParentId)
                {
                    continue;
                }

                if (Page.ParentId == ParentId)
                {
                    i++;
                }
            }
            return i;
        }
    }
}