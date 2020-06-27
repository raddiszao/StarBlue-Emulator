namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    class ModeratorSupportTicketResponseComposer : ServerPacket
    {
        public ModeratorSupportTicketResponseComposer(int Result)
            : base(ServerPacketHeader.ModeratorSupportTicketResponseMessageComposer)
        {
            base.WriteInteger(Result);
            base.WriteString("");
        }
    }
}