namespace StarBlue.Communication.Packets.Outgoing.Users
{
    internal class UpdateUsernameComposer : MessageComposer
    {
        private string User { get; }

        public UpdateUsernameComposer(string User)
            : base(Composers.UpdateUsernameMessageComposer)
        {
            this.User = User;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(0);
            packet.WriteString(User);
            packet.WriteInteger(0);
        }
    }
}