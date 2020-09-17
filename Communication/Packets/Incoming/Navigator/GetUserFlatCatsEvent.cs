using StarBlue.Communication.Packets.Outgoing.Navigator;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Navigator;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.Navigator
{
    public class GetUserFlatCatsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            if (Session == null)
            {
                return;
            }

            ICollection<SearchResultList> Categories = StarBlueServer.GetGame().GetNavigator().GetFlatCategories();

            Session.SendMessage(new UserFlatCatsComposer(Categories, Session.GetHabbo().Rank));
        }
    }
}