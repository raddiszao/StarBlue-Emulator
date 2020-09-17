namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class FloorHeightMapComposer : MessageComposer
    {
        public string Map { get; }
        public int WallHeight { get; }

        public FloorHeightMapComposer(string Map, int WallHeight)
            : base(Composers.FloorHeightMapMessageComposer)
        {
            this.Map = Map;
            this.WallHeight = WallHeight;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(WallHeight > 0);
            packet.WriteInteger(WallHeight);
            packet.WriteString(Map);
        }
    }
}
