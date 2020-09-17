namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class PresentDeliverErrorMessageComposer : MessageComposer
    {
        private bool CreditError { get; }

        private bool DucketError { get; }

        public PresentDeliverErrorMessageComposer(bool CreditError, bool DucketError)
            : base(Composers.PresentDeliverErrorMessageComposer)
        {
            this.CreditError = CreditError;
            this.DucketError = DucketError;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(CreditError);//Do we have enough credits?
            packet.WriteBoolean(DucketError);//Do we have enough duckets?
        }
    }
}
