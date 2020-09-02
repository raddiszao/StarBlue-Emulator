using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms.PathFinding;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.FloorPlan
{
    internal class FloorPlanFloorMapComposer : ServerPacket
    {
        public FloorPlanFloorMapComposer(ICollection<Item> Items)
            : base(ServerPacketHeader.FloorPlanFloorMapMessageComposer)
        {
            List<ThreeDCoord> Tiles = new List<ThreeDCoord>();
            foreach (Item Item in Items.ToList())
            {
                foreach (ThreeDCoord Tile in Item.GetAffectedTiles.Values.ToList())
                {
                    Tiles.Add(Tile);
                }
            }

            base.WriteInteger(Tiles.Count);
            foreach (ThreeDCoord Tile in Tiles)
            {
                base.WriteInteger(Tile.X);
                base.WriteInteger(Tile.Y);
            }
        }
    }
}
