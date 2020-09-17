namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class ClubGiftsComposer : MessageComposer
    {
        public ClubGiftsComposer()
            : base(Composers.ClubGiftsMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(-1);//Days until next gift.
            packet.WriteInteger(0);//Gifts available
            packet.WriteInteger(12);//Count?
            {
                packet.WriteInteger(12701);
                packet.WriteString("hc16_1");
                packet.WriteBoolean(false);
                packet.WriteInteger(1);
                packet.WriteInteger(0);
                packet.WriteInteger(0);
                packet.WriteBoolean(true);
                packet.WriteInteger(1);//Count for some reason
                {
                    packet.WriteString("s");
                    packet.WriteInteger(8228);
                    packet.WriteString("");
                    packet.WriteInteger(1);
                    packet.WriteBoolean(false);
                }
                //  packet.WriteInteger(0);
                //packet.WriteBoolean(true);
            }

            packet.WriteInteger(0);//Count
            {
                //int, bool, int, bool
                packet.WriteInteger(3253248);//Maybe the item id?

                packet.WriteBoolean(false);//Can we get?
                packet.WriteInteger(256);//idk
                packet.WriteBoolean(false);//idk
                packet.WriteInteger(0);
                packet.WriteBoolean(true);

            }
        }
    }
}
