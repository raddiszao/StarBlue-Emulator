namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class AvatarAspectUpdateMessageComposer : MessageComposer
    {
        private string Figure { get; }
        private string Gender { get; }

        public AvatarAspectUpdateMessageComposer(string Figure, string Gender)
            : base(Composers.AvatarAspectUpdateMessageComposer)
        {
            this.Figure = Figure;
            this.Gender = Gender;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(Figure);
            packet.WriteString(Gender);

        }
    }
}