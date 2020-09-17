namespace StarBlue.Communication.Packets.Outgoing.Handshake
{
    public class InitCryptoComposer : MessageComposer
    {
        private string Prime { get; }
        private string Generator { get; }

        public InitCryptoComposer(string Prime, string Generator)
            : base(Composers.InitCryptoMessageComposer)
        {
            this.Prime = Prime;
            this.Generator = Generator;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(Prime);
            packet.WriteString(Generator);
        }
    }
}