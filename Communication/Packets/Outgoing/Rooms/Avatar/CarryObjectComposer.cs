namespace StarBlue.Communication.Packets.Outgoing.Rooms.Avatar
{
    internal class CarryObjectComposer : ServerPacket
    {
        public CarryObjectComposer(int virtualID, int itemID)
            : base(ServerPacketHeader.CarryObjectMessageComposer)
        {
            base.WriteInteger(virtualID);
            base.WriteInteger(itemID);
        }
    }
}
