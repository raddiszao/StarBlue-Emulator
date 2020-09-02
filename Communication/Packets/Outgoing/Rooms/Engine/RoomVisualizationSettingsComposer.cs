namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class RoomVisualizationSettingsComposer : ServerPacket
    {
        public RoomVisualizationSettingsComposer(int Walls, int Floor, bool HideWalls)
            : base(ServerPacketHeader.RoomVisualizationSettingsMessageComposer)
        {
            base.WriteBoolean(HideWalls);
            base.WriteInteger(Walls);
            base.WriteInteger(Floor);
        }
    }
}