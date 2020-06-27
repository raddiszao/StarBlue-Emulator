using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.PathFinding;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    class UpdateStackMapMessageComposer : ServerPacket
    {
        public UpdateStackMapMessageComposer(Room Room, Dictionary<int, ThreeDCoord> Tiles, double Height = -1)
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
                    base.WriteShort((int)((Height == -1 ? Room.GetGameMap().SqAbsoluteHeight(Tile.X, Tile.Y) : Height) * 256));
                }
            }
            else
            {
                base.WriteByte(Tiles.Count);
                foreach (ThreeDCoord Tile in Tiles.Values)
                {
                    base.WriteByte(Tile.X);
                    base.WriteByte(Tile.Y);
                    base.WriteShort((int)((Height == -1 ? Room.GetGameMap().SqAbsoluteHeight(Tile.X, Tile.Y) : Height) * 256));
                }
            }
        }

    }
}
