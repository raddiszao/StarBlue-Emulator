namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorSupportTicketResponseComposer : ServerPacket
    {
        public ModeratorSupportTicketResponseComposer(int Result)
            : base(ServerPacketHeader.ModeratorSupportTicketResponseMessageComposer)
        {
            base.WriteInteger(Result);
            base.WriteString("");
        }
    }
}