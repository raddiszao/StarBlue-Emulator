namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class CheckGnomeNameComposer : MessageComposer
    {
        private string PetName { get; }
        private int ErrorId { get; }

        public CheckGnomeNameComposer(string PetName, int ErrorId)
            : base(Composers.CheckGnomeNameMessageComposer)
        {
            this.PetName = PetName;
            this.ErrorId = ErrorId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(0);
            packet.WriteInteger(ErrorId);
            packet.WriteString(PetName);
        }
    }
}
