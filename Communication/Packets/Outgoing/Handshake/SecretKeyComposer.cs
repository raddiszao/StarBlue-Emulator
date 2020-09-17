namespace StarBlue.Communication.Packets.Outgoing.Handshake
{
    public class SecretKeyComposer : MessageComposer
    {
        private string PublicKey { get; }

        public SecretKeyComposer(string PublicKey)
            : base(Composers.SecretKeyMessageComposer)
        {
            this.PublicKey = PublicKey;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(PublicKey);
        }
    }
}