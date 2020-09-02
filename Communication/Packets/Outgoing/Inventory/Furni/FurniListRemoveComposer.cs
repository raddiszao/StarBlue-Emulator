namespace StarBlue.Communication.Packets.Outgoing.Inventory.Furni
{
    internal class FurniListRemoveComposer : ServerPacket
    {
        public FurniListRemoveComposer(int Id)
            : base(ServerPacketHeader.FurniListRemoveMessageComposer)
        {
            base.WriteInteger(Id);
        }
    }
}
