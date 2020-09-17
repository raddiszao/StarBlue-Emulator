namespace StarBlue.Communication.Packets.Outgoing.Rooms.AI.Pets
{
    internal class AddExperiencePointsComposer : MessageComposer
    {
        private int PetId { get; }
        private int VirtualId { get; }
        private int Amount { get; }

        public AddExperiencePointsComposer(int PetId, int VirtualId, int Amount)
            : base(Composers.AddExperiencePointsMessageComposer)
        {
            this.PetId = PetId;
            this.VirtualId = VirtualId;
            this.Amount = Amount;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(PetId);
            packet.WriteInteger(VirtualId);
            packet.WriteInteger(Amount);
        }
    }
}