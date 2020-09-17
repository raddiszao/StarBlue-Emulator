namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class FurniMaticNoRoomError : MessageComposer
    {
        public FurniMaticNoRoomError()
            : base(Composers.FurniMaticNoRoomError)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(1);
            packet.WriteInteger(0);
        }
    }
}
