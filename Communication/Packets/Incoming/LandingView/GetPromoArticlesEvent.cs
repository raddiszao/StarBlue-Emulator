using StarBlue.Communication.Packets.Outgoing.LandingView;
using StarBlue.HabboHotel.LandingView.Promotions;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.LandingView
{
    class GetPromoArticlesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            ICollection<Promotion> LandingPromotions = StarBlueServer.GetGame().GetLandingManager().GetPromotionItems();

            Session.SendMessage(new PromoArticlesComposer(LandingPromotions));
        }
    }
}
