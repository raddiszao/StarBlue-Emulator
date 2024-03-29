﻿
using StarBlue.Communication.Packets.Outgoing.Marketplace;

namespace StarBlue.Communication.Packets.Incoming.Marketplace
{
    internal class GetOwnOffersEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            Session.SendMessage(new MarketPlaceOwnOffersComposer(Session.GetHabbo().Id));
        }
    }
}
