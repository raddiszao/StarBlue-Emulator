namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    internal class OpenHelpToolComposer : MessageComposer
    {
        public OpenHelpToolComposer()
            : base(Composers.OpenHelpToolMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(0);
        }
    }
}
