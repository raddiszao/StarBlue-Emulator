namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    public class RecyclerRewardsComposer : MessageComposer
    {
        public RecyclerRewardsComposer()
            : base(Composers.RecyclerRewardsMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(0);// Count of items
        }
    }
}