namespace StarBlue.Communication.Packets.Outgoing.Rooms.FloorPlan
{
    internal class FloorPlanSendDoorComposer : ServerPacket
    {
        public FloorPlanSendDoorComposer(int DoorX, int DoorY, int DoorDirection)
            : base(ServerPacketHeader.FloorPlanSendDoorMessageComposer)
        {
            base.WriteInteger(DoorX);
            base.WriteInteger(DoorY);
            base.WriteInteger(DoorDirection);
        }
    }
}
