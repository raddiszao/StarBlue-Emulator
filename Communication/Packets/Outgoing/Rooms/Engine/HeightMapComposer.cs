using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.PathFinding;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class HeightMapComposer : ServerPacket
    {
        public HeightMapComposer(Room Room)
            : base(ServerPacketHeader.HeightMapMessageComposer)
        {
            var map = Room.GetGameMap();
            base.WriteInteger(map.Model.MapSizeX);
            base.WriteInteger(map.Model.MapSizeX * map.Model.MapSizeY);
            for (var y = 0; y < map.Model.MapSizeY; y++)
            {
                for (var x = 0; x < map.Model.MapSizeX; x++)
                {
                    if (map.Model.SqState[x, y] == SquareState.BLOCKED)
                        WriteShort(16384);
                    else
                    {
                        List<Item> items = map.GetCoordinatedItems(new Point(x, y));
                        Item item = null;
                        bool itemOnMagicTile = false;
                        if (items.Any())
                            item = items.OrderByDescending(value => value.TotalHeight).FirstOrDefault();

                        if (item != null)
                        {
                            foreach (ThreeDCoord Point in item.GetAffectedTiles.Values.ToList())
                            {
                                if (map.HasStackTool(Point.X, Point.Y) && item.Data.InteractionType != InteractionType.STACKTOOL)
                                {
                                    base.WriteShort(0);
                                    itemOnMagicTile = true;
                                    break;
                                }
                            }
                        }

                        if (!itemOnMagicTile)
                        {
                            try
                            {
                                ushort Height = (ushort)(map.SqAbsoluteHeight(x, y) * 256);
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
    }
}
