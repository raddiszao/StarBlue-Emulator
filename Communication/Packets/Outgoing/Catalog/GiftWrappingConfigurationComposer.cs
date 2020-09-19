namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    public class GiftWrappingConfigurationComposer : MessageComposer
    {
        private int[] BoxTypes = new int[] {
            0, 1, 2, 3, 4, 5, 6, 8
        };

        private int[] RibbonTypes = new int[]
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10
        };

        public GiftWrappingConfigurationComposer()
            : base(Composers.GiftWrappingConfigurationMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(true);
            packet.WriteInteger(1);
            packet.WriteInteger(10);
            for (int i = 3372; i < 3382;)
            {
                packet.WriteInteger(i);
                i++;
            }

            packet.WriteInteger(BoxTypes.Length);
            foreach (int Box in BoxTypes)
                packet.WriteInteger(Box);

            packet.WriteInteger(RibbonTypes.Length);
            foreach (int Ribbon in RibbonTypes)
                packet.WriteInteger(Ribbon);

            packet.WriteInteger(7);
            for (int i = 187; i < 194;)
            {
                packet.WriteInteger(i);
                i++;
            }
        }
    }
}