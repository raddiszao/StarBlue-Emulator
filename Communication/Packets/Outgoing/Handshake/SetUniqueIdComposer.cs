namespace StarBlue.Communication.Packets.Outgoing.Handshake
{
    internal class SetUniqueIdComposer : MessageComposer
    {
        private string UserId { get; }

        public SetUniqueIdComposer(string Id)
            : base(Composers.SetUniqueIdMessageComposer)
        {
            this.UserId = Id;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(UserId);
        }
    }
}
