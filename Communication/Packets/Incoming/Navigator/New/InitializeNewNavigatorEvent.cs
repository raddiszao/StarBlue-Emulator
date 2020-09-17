using StarBlue.Communication.Packets.Outgoing.Navigator;
using StarBlue.HabboHotel.Navigator;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.Navigator
{
    internal class InitializeNewNavigatorEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            ICollection<TopLevelItem> TopLevelItems = StarBlueServer.GetGame().GetNavigator().GetTopLevelItems();
            ICollection<SearchResultList> SearchResultLists = StarBlueServer.GetGame().GetNavigator().GetSearchResultLists();

            Session.SendQueue(new NavigatorMetaDataParserComposer(TopLevelItems));
            Session.SendQueue(new NavigatorLiftedRoomsComposer());
            Session.SendQueue(new NavigatorCollapsedCategoriesComposer());
            Session.SendQueue(new NavigatorPreferencesComposer());
            Session.Flush();
        }
    }
}
