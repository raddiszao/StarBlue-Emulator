namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class PresentDeliverErrorMessageComposer : ServerPacket
    {
        public PresentDeliverErrorMessageComposer(bool CreditError, bool DucketError)
            : base(ServerPacketHeader.PresentDeliverErrorMessageComposer)
        {
            base.WriteBoolean(CreditError);//Do we have enough credits?
            base.WriteBoolean(DucketError);//Do we have enough duckets?
        }
    }
}
