namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class RoomVisualizationSettingsComposer : MessageComposer
    {
        public int Walls { get; }
        public int Floor { get; }
        public bool HideWalls { get; }

        public RoomVisualizationSettingsComposer(int Walls, int Floor, bool HideWalls)
            : base(Composers.RoomVisualizationSettingsMessageComposer)
        {
            this.Walls = Walls;
            this.Floor = Floor;
            this.HideWalls = HideWalls;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(HideWalls);
            packet.WriteInteger(Walls);
            packet.WriteInteger(Floor);
        }
    }
}