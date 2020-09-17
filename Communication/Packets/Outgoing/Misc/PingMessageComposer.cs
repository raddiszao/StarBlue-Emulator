namespace StarBlue.Communication.Packets.Outgoing.Misc
{
    internal class PingMessageComposer : MessageComposer
    {

        public PingMessageComposer()
            : base(Composers.LatencyResponseMessageComposer)
        {

        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(0);
        }
    }
}
