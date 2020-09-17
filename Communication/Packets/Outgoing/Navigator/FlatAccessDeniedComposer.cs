namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class FlatAccessDeniedComposer : MessageComposer
    {
        private string Username { get; }

        public FlatAccessDeniedComposer(string Username)
            : base(Composers.FlatAccessDeniedMessageComposer)
        {
            this.Username = Username;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(Username);
        }
    }
}
