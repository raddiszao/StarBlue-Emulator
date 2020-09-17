namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class DoorbellComposer : MessageComposer
    {
        private string Username { get; }

        public DoorbellComposer(string Username)
            : base(Composers.DoorbellMessageComposer)
        {
            this.Username = Username;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(Username);
        }
    }
}
