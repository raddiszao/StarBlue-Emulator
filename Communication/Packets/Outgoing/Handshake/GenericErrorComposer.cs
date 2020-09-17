namespace StarBlue.Communication.Packets.Outgoing.Handshake
{
    internal class GenericErrorComposer : MessageComposer
    {
        private int errorId { get; }

        public GenericErrorComposer(int errorId)
            : base(Composers.GenericErrorMessageComposer)
        {
            this.errorId = errorId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(errorId);
        }
    }
}
