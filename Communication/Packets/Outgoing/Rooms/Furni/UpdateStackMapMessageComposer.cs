using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.PathFinding;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    internal class UpdateStackMapMessageComposer : ServerPacket
    {
        public UpdateStackMapMessageComposer(Room Room, Dictionary<int, ThreeDCoord> Tiles, bool HeightIsZero = false)
            : base(ServerPacketHeader.UpdateStackMapMessageComposer)
        {
            if (Tiles.Count > 127)
            {
                base.WriteByte(127);
                for (int i = 0; i < 127; i++)
                {
                    Tiles.TryGetValue(i, out ThreeDCoord Tile);
                    base.WriteByte(Tile.X);
                    base.WriteByte(Tile.Y);
                    try
                    {
                        short Height = (short)(Room.GetGameMap().SqAbsoluteHeight(Tile.X, Tile.Y) * 256);
                        base.WriteShort((short)(HeightIsZero ? 0 : (Height > short.MaxValue ? short.MaxValue : Height)));
                    }
                    catch
                    {
                        base.WriteShort(0);
                    }
                }
            }
            else
            {
                base.WriteByte(Tiles.Count);
                foreach (ThreeDCoord Tile in Tiles.Values)
                {
                    base.WriteByte(Tile.X);
                    base.WriteByte(Tile.Y);
                    try
                    {
                        ushort Height = (ushort)(Room.GetGameMap().SqAbsoluteHeight(Tile.X, Tile.Y) * 256);
                        base.WriteUnsignedShort((ushort)(Height > ushort.MaxValue ? ushort.MaxValue : Height));
                    }
                    catch
                    {
                        base.WriteShort(0);
                    }
                }
            }
        }

    }
}
