namespace StarBlue.Communication.Packets.Outgoing.Rooms.Session
{
    internal class FlatAccessibleComposer : MessageComposer
    {
        private string Username { get; }

        public FlatAccessibleComposer(string Username)
            : base(Composers.FlatAccessibleMessageComposer)
        {
            this.Username = Username;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(Username);
        }
    }
}
