namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    public class CheckPetNameComposer : MessageComposer
    {
        private int Error { get; }
        private string ExtraData { get; }

        public CheckPetNameComposer(int Error, string ExtraData)
            : base(Composers.CheckPetNameMessageComposer)
        {
            this.Error = Error;
            this.ExtraData = ExtraData;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Error);//0 = nothing, 1 = too long, 2 = too short, 3 = invalid characters
            packet.WriteString(ExtraData);
        }
    }
}