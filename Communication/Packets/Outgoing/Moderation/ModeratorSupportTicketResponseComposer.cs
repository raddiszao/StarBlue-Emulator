namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorSupportTicketResponseComposer : MessageComposer
    {
        private int Result { get; }

        public ModeratorSupportTicketResponseComposer(int Result)
            : base(Composers.ModeratorSupportTicketResponseMessageComposer)
        {
            this.Result = Result;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Result);
            packet.WriteString("");
        }
    }
}