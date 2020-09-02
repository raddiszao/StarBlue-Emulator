using StarBlue.Communication.Packets.Outgoing.Navigator;
using StarBlue.HabboHotel.Navigator;
using StarBlue.Messages;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.Navigator
{
    internal class InitializeNewNavigatorEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            ICollection<TopLevelItem> TopLevelItems = StarBlueServer.GetGame().GetNavigator().GetTopLevelItems();
            ICollection<SearchResultList> SearchResultLists = StarBlueServer.GetGame().GetNavigator().GetSearchResultLists();

            QueuedServerMessage message = new QueuedServerMessage(Session.GetConnection());
            message.appendResponse(new NavigatorMetaDataParserComposer(TopLevelItems));
            message.appendResponse(new NavigatorLiftedRoomsComposer());
            message.appendResponse(new NavigatorCollapsedCategoriesComposer());
            message.appendResponse(new NavigatorPreferencesComposer());
            message.sendResponse();
        }
    }
}
