namespace StarBlue.Communication.Packets.Outgoing.Inventory.Furni
{
    internal class FurniListUpdateComposer : ServerPacket
    {
        public FurniListUpdateComposer()
            : base(ServerPacketHeader.FurniListUpdateMessageComposer)
        {

        }
    }
}
