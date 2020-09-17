using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Navigator;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class NavigatorSearchResultSetComposer : MessageComposer
    {
        private string Category { get; }
        private string Data { get; }
        private ICollection<SearchResultList> SearchResultLists { get; }
        private GameClient Session { get; }
        private int GoBack { get; }
        private int FetchLimit { get; }

        public NavigatorSearchResultSetComposer(string Category, string Data, ICollection<SearchResultList> SearchResultLists, GameClient Session, int GoBack = 1, int FetchLimit = 50)
            : base(Composers.NavigatorSearchResultSetMessageComposer)
        {
            this.Category = Category;
            this.Data = Data;
            this.SearchResultLists = SearchResultLists;
            this.Session = Session;
            this.GoBack = GoBack;
            this.FetchLimit = FetchLimit;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(Category);//Search code.
            packet.WriteString(Data);//Text?

            packet.WriteInteger(SearchResultLists.Count);//Count
            foreach (SearchResultList SearchResult in SearchResultLists.ToList())
            {
                packet.WriteString(SearchResult.CategoryIdentifier);
                packet.WriteString(SearchResult.PublicName);
                packet.WriteInteger(NavigatorSearchAllowanceUtility.GetIntegerValue(SearchResult.SearchAllowance) != 0 ? GoBack : NavigatorSearchAllowanceUtility.GetIntegerValue(SearchResult.SearchAllowance));//0 = nothing, 1 = show more, 2 = back Action allowed.
                packet.WriteBoolean(false);//True = minimized, false = open.
                packet.WriteInteger(SearchResult.ViewMode == NavigatorViewMode.REGULAR ? 0 : SearchResult.ViewMode == NavigatorViewMode.THUMBNAIL ? 1 : 0);//View mode, 0 = tiny/regular, 1 = thumbnail

                NavigatorHandler.Search(packet, SearchResult, Data, Session, FetchLimit);
            }
        }
    }
}
