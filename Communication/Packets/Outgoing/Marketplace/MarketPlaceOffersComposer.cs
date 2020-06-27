﻿using StarBlue.HabboHotel.Catalog.Marketplace;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Marketplace
{
    class MarketPlaceOffersComposer : ServerPacket
    {
        public MarketPlaceOffersComposer(int MinCost, int MaxCost, Dictionary<int, MarketOffer> dictionary, Dictionary<int, int> dictionary2)
            : base(ServerPacketHeader.MarketPlaceOffersMessageComposer)
        {
            base.WriteInteger(dictionary.Count);
            if (dictionary.Count > 0)
            {
                foreach (KeyValuePair<int, MarketOffer> pair in dictionary)
                {
                    base.WriteInteger(pair.Value.OfferID);
                    base.WriteInteger(1);//State
                    base.WriteInteger(1);
                    base.WriteInteger(pair.Value.SpriteId);

                    base.WriteInteger(256);
                    base.WriteString("");
                    base.WriteInteger(pair.Value.LimitedNumber);
                    base.WriteInteger(pair.Value.LimitedStack);

                    base.WriteInteger(pair.Value.TotalPrice);
                    base.WriteInteger(0);
                    base.WriteInteger(StarBlueServer.GetGame().GetCatalog().GetMarketplace().AvgPriceForSprite(pair.Value.SpriteId));
                    base.WriteInteger(dictionary2[pair.Value.SpriteId]);
                }
            }
            base.WriteInteger(dictionary.Count);//Item count to show how many were found.
        }
    }
}
