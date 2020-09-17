namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    internal class HelperSessionVisiteRoomComposer : MessageComposer
    {
        private int roomId { get; }

        public HelperSessionVisiteRoomComposer(int roomId)
            : base(Composers.HelperSessionVisiteRoomMessageComposer)
        {
            this.roomId = roomId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(roomId);
        }
    }
}
