using StarBlue.HabboHotel.Catalog;
using StarBlue.HabboHotel.GameClients;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    public class CatalogIndexComposer : MessageComposer
    {
        private GameClient Session { get; }
        private ICollection<CatalogPage> Pages { get; }
        private string Mode { get; }

        public CatalogIndexComposer(GameClient Session, ICollection<CatalogPage> Pages, string Mode)
            : base(Composers.CatalogIndexMessageComposer)
        {
            this.Session = Session;
            this.Pages = Pages;
            this.Mode = Mode;
        }

        public override void Compose(Composer packet)
        {
            WriteRootIndex(Session, Pages, packet);
            foreach (CatalogPage Parent in Pages)
            {
                if (Parent.ParentId != -1 || (Parent.MinimumRank > Session.GetHabbo().Rank && Parent.MinimumVIP == 0) || (Parent.MinimumVIP > 0 && Parent.MinimumVIP > Session.GetHabbo().VIPRank && Parent.MinimumRank > Session.GetHabbo().Rank))
                {
                    continue;
                }

                WritePage(Session, Parent, CalcTreeSize(Session, Pages, Parent.Id), packet);
                foreach (CatalogPage child in Pages)
                {
                    if (child.ParentId != Parent.Id || (child.MinimumRank > Session.GetHabbo().Rank && child.MinimumVIP == 0) || (child.MinimumVIP > 0 && child.MinimumVIP > Session.GetHabbo().VIPRank && child.MinimumRank > Session.GetHabbo().Rank))
                    {
                        continue;
                    }

                    if (child.Enabled)
                    {
                        WritePage(Session, child, CalcTreeSize(Session, Pages, child.Id), packet);
                    }
                    else
                    {
                        WriteNodeIndex(Session, child, CalcTreeSize(Session, Pages, child.Id), packet);
                    }

                    foreach (CatalogPage SubChild in Pages)
                    {
                        if (SubChild.ParentId != child.Id || (SubChild.MinimumRank > Session.GetHabbo().Rank && SubChild.MinimumVIP == 0) || (SubChild.MinimumVIP > 0 && SubChild.MinimumVIP > Session.GetHabbo().VIPRank && SubChild.MinimumRank > Session.GetHabbo().Rank))
                        {
                            continue;
                        }

                        if (SubChild.Enabled)
                        {
                            WritePage(Session, SubChild, CalcTreeSize(Session, Pages, SubChild.Id), packet);
                        }
                        else
                        {
                            WriteNodeIndex(Session, SubChild, CalcTreeSize(Session, Pages, SubChild.Id), packet);
                        }

                        foreach (CatalogPage SubSChild in Pages)
                        {
                            if (SubSChild.ParentId != SubChild.Id || (SubSChild.MinimumRank > Session.GetHabbo().Rank && SubSChild.MinimumVIP == 0) || (SubSChild.MinimumVIP > 0 && SubSChild.MinimumVIP > Session.GetHabbo().VIPRank && !(SubSChild.MinimumRank > Session.GetHabbo().Rank)))
                            {
                                continue;
                            }

                            WritePage(Session, SubSChild, 0, packet);
                        }
                    }
                }
            }

            packet.WriteBoolean(false);
            packet.WriteString(Mode);
        }

        public void WriteRootIndex(GameClient session, ICollection<CatalogPage> pages, Composer packet)
        {
            packet.WriteBoolean(true);
            packet.WriteInteger(0);
            packet.WriteInteger(-1);
            packet.WriteString("root");
            packet.WriteString(string.Empty);
            packet.WriteInteger(0);
            packet.WriteInteger(CalcTreeSize(session, pages, -1));
        }
        public void WriteNodeIndex(GameClient Session, CatalogPage page, int treeSize, Composer packet)
        {
            string Caption = page.Caption;
            if (!treeSize.Equals(0) && Session.GetHabbo().Rank >= 14)
            {
                Caption += " (" + treeSize + ")";
            }

            packet.WriteBoolean(page.Visible);
            packet.WriteInteger(page.Icon);
            packet.WriteInteger(-1);
            packet.WriteString(page.PageLink);
            packet.WriteString(Caption);
            packet.WriteInteger(0);
            packet.WriteInteger(treeSize);
        }
        public void WritePage(GameClient Session, CatalogPage page, int treeSize, Composer packet)
        {
            string Caption = page.Caption;
            if (!treeSize.Equals(0) && Session.GetHabbo().Rank >= 14)
            {
                Caption += " (" + treeSize + ")";
            }

            packet.WriteBoolean(page.Visible);
            packet.WriteInteger(page.Icon);
            packet.WriteInteger(page.Id);
            packet.WriteString(page.PageLink);
            packet.WriteString(Caption);
            packet.WriteInteger(page.ItemOffers.Count);
            foreach (int i in page.ItemOffers.Keys)
            {
                packet.WriteInteger(i);
            }
            packet.WriteInteger(treeSize);
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
    }
}