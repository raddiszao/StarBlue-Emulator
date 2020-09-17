using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.PathFinding;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    internal class UpdateStackMapMessageComposer : MessageComposer
    {
        private Room Room { get; }
        private Dictionary<int, ThreeDCoord> Tiles { get; }
        private bool HeightIsZero { get; }

        public UpdateStackMapMessageComposer(Room Room, Dictionary<int, ThreeDCoord> Tiles, bool HeightIsZero = false)
            : base(Composers.UpdateStackMapMessageComposer)
        {
            this.Room = Room;
            this.Tiles = Tiles;
            this.HeightIsZero = HeightIsZero;
        }

        public override void Compose(Composer packet)
        {
            if (Tiles.Count > 127)
            {
                packet.WriteByte(127);
                for (int i = 0; i < 127; i++)
                {
                    Tiles.TryGetValue(i, out ThreeDCoord Tile);
                    packet.WriteByte(Tile.X);
                    packet.WriteByte(Tile.Y);
                    try
                    {
                        short Height = (short)(Room.GetGameMap().SqAbsoluteHeight(Tile.X, Tile.Y) * 256);
                        packet.WriteShort((short)(HeightIsZero ? 0 : (Height > short.MaxValue ? short.MaxValue : Height)));
                    }
                    catch
                    {
                        packet.WriteShort(0);
                    }
                }
            }
            else
            {
                packet.WriteByte(Tiles.Count);
                foreach (ThreeDCoord Tile in Tiles.Values)
                {
                    packet.WriteByte(Tile.X);
                    packet.WriteByte(Tile.Y);
                    try
                    {
                        ushort Height = (ushort)(Room.GetGameMap().SqAbsoluteHeight(Tile.X, Tile.Y) * 256);
                        packet.WriteShort(Height);
                    }
                    catch
                    {
                        packet.WriteShort(0);
                    }
                }
            }
        }

    }
}
