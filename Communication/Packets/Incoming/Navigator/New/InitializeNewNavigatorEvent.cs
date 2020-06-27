using StarBlue.Communication.Packets.Outgoing.Navigator;
using StarBlue.HabboHotel.Navigator;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.Navigator
{
    class InitializeNewNavigatorEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            ICollection<TopLevelItem> TopLevelItems = StarBlueServer.GetGame().GetNavigator().GetTopLevelItems();
            ICollection<SearchResultList> SearchResultLists = StarBlueServer.GetGame().GetNavigator().GetSearchResultLists();

            Session.SendMessage(new NavigatorMetaDataParserComposer(TopLevelItems));
            Session.SendMessage(new NavigatorLiftedRoomsComposer());
            Session.SendMessage(new NavigatorCollapsedCategoriesComposer());
            Session.SendMessage(new NavigatorPreferencesComposer());
        }
    }
}
