namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    internal class RoomInviteComposer : MessageComposer
    {
        public int SenderId { get; }
        public string Text { get; }

        public RoomInviteComposer(int SenderId, string Text)
            : base(Composers.RoomInviteMessageComposer)
        {
            this.SenderId = SenderId;
            this.Text = Text;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(SenderId);
            packet.WriteString(Text);
        }
    }
}
