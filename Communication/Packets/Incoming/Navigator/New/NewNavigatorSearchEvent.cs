using StarBlue.Communication.Packets.Outgoing.Navigator;
using StarBlue.HabboHotel.Navigator;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.Navigator
{
    class NewNavigatorSearchEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            string Category = packet.PopString();
            string Search = packet.PopString();

            ICollection<SearchResultList> Categories = new List<SearchResultList>();

            if (!string.IsNullOrEmpty(Search))
            {
                if (StarBlueServer.GetGame().GetNavigator().TryGetSearchResultList(0, out SearchResultList QueryResult))
                {
                    Categories.Add(QueryResult);
                }
            }
            else
            {
                Categories = StarBlueServer.GetGame().GetNavigator().GetCategorysForSearch(Category);
                if (Categories.Count == 0)
                {
                    //Are we going in deep?!
                    Categories = StarBlueServer.GetGame().GetNavigator().GetResultByIdentifier(Category);
                    if (Categories.Count > 0)
                    {
                        session.SendMessage(new NavigatorSearchResultSetComposer(Category, Search, Categories, session, 2, 100));
                        return;
                    }
                }
            }

            session.SendMessage(new NavigatorSearchResultSetComposer(Category, Search, Categories, session));
        }
    }
}
