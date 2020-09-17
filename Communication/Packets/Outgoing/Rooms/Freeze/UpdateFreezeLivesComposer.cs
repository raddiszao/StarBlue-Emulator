namespace StarBlue.Communication.Packets.Outgoing.Rooms.Freeze
{
    internal class UpdateFreezeLivesComposer : MessageComposer
    {
        private int UserId { get; }
        private int FreezeLives { get; }

        public UpdateFreezeLivesComposer(int UserId, int FreezeLives)
            : base(Composers.UpdateFreezeLivesMessageComposer)
        {
            this.UserId = UserId;
            this.FreezeLives = FreezeLives;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(UserId);
            packet.WriteInteger(FreezeLives);
        }
    }
}
