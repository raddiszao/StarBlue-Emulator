namespace StarBlue.Communication.Packets.Outgoing.Handshake
{
    public class AuthenticationOKComposer : MessageComposer
    {
        public AuthenticationOKComposer()
            : base(Composers.AuthenticationOKMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
        }
    }
}