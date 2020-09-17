using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms.Trading;
using System.Linq;


namespace StarBlue.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingUpdateComposer : MessageComposer
    {
        public Trade Trade { get; }

        public TradingUpdateComposer(Trade trade)
            : base(Composers.TradingUpdateMessageComposer)
        {
            this.Trade = trade;
        }

        public override void Compose(Composer packet)
        {
            if (Trade.Users.Count() < 2)
            {
                return;
            }

            foreach (TradeUser user in Trade.Users)
            {
                packet.WriteInteger(user.GetClient().GetHabbo().Id);
                packet.WriteInteger(user.OfferedItems.Count);

                foreach (Item Item in user.OfferedItems.ToList())
                {
                    packet.WriteInteger(Item.Id);
                    packet.WriteString(Item.Data.Type.ToString().ToUpper());
                    packet.WriteInteger(Item.Id);
                    packet.WriteInteger(Item.Data.SpriteId);
                    packet.WriteInteger(0);//Not sure.
                    if (Item.LimitedNo > 0)
                    {
                        packet.WriteBoolean(false);//Stackable
                        packet.WriteInteger(256);
                        packet.WriteString("");
                        packet.WriteInteger(Item.LimitedNo);
                        packet.WriteInteger(Item.LimitedTot);
                    }
                    else
                    {
                        packet.WriteBoolean(true);//Stackable
                        packet.WriteInteger(0);
                        packet.WriteString("");
                    }

                    packet.WriteInteger(0);
                    packet.WriteInteger(0);
                    packet.WriteInteger(0);
                    if (Item.Data.Type.ToString().ToUpper() == "S")
                    {
                        packet.WriteInteger(0);
                    }
                }

                packet.WriteInteger(user.OfferedItems.Count);
                packet.WriteInteger(0);

            }
        }
    }
}