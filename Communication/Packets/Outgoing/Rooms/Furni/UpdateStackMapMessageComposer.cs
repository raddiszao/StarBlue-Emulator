using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.PathFinding;
using System;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    class UpdateStackMapMessageComposer : ServerPacket
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
                    int Height = (int)(Room.GetGameMap().SqAbsoluteHeight(Tile.X, Tile.Y) * 256);
                    if (Height > UInt16.MaxValue)
                    {
                        Height = UInt16.MaxValue;
                    }

                    base.WriteUnsignedShort((UInt16)(HeightIsZero ? 0 : Height));
                }
            }
            else
            {
                base.WriteByte(Tiles.Count);
                foreach (ThreeDCoord Tile in Tiles.Values)
                {
                    base.WriteByte(Tile.X);
                    base.WriteByte(Tile.Y);
                    int Height = (int)(Room.GetGameMap().SqAbsoluteHeight(Tile.X, Tile.Y) * 256);
                    if (Height > UInt16.MaxValue)
                    {
                        Height = UInt16.MaxValue;
                    }

                    base.WriteUnsignedShort((UInt16)(HeightIsZero ? 0 : Height));
                }
            }
        }

    }
}
