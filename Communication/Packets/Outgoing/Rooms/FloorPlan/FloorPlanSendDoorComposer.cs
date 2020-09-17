namespace StarBlue.Communication.Packets.Outgoing.Rooms.FloorPlan
{
    internal class FloorPlanSendDoorComposer : MessageComposer
    {
        private int DoorX { get; }
        private int DoorY { get; }
        private int DoorDirection { get; }

        public FloorPlanSendDoorComposer(int DoorX, int DoorY, int DoorDirection)
            : base(Composers.FloorPlanSendDoorMessageComposer)
        {
            this.DoorX = DoorX;
            this.DoorY = DoorY;
            this.DoorDirection = DoorDirection;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(DoorX);
            packet.WriteInteger(DoorY);
            packet.WriteInteger(DoorDirection);
        }
    }
}
