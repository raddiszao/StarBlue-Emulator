using StarBlue.Communication.Packets.Outgoing.Navigator;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Navigator;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.Navigator
{
    internal class GetNavigatorFlatsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            ICollection<SearchResultList> Categories = StarBlueServer.GetGame().GetNavigator().GetEventCategories();

            Session.SendMessage(new NavigatorFlatCatsComposer(Categories, Session.GetHabbo().Rank));
        }
    }
}