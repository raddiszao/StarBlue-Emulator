namespace StarBlue.Communication.Packets.Outgoing.Rooms.Chat
{
    public class UserTypingComposer : MessageComposer
    {
        private int VirtualId { get; }
        private bool Typing { get; }

        public UserTypingComposer(int VirtualId, bool Typing)
            : base(Composers.UserTypingMessageComposer)
        {
            this.VirtualId = VirtualId;
            this.Typing = Typing;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(VirtualId);
            packet.WriteInteger(Typing ? 1 : 0);
        }
    }
}