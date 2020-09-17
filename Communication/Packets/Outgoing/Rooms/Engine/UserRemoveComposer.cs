namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class UserRemoveComposer : MessageComposer
    {
        public int UserId { get; }

        public UserRemoveComposer(int Id)
            : base(Composers.UserRemoveMessageComposer)
        {
            this.UserId = Id;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(UserId.ToString());
        }
    }
}
