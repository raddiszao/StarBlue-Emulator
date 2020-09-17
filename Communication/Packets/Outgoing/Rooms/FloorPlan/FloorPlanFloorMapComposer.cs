using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms.PathFinding;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.FloorPlan
{
    internal class FloorPlanFloorMapComposer : MessageComposer
    {
        private ICollection<Item> Items { get; }

        public FloorPlanFloorMapComposer(ICollection<Item> Items)
            : base(Composers.FloorPlanFloorMapMessageComposer)
        {
            this.Items = Items;
        }

        public override void Compose(Composer packet)
        {
            List<ThreeDCoord> Tiles = new List<ThreeDCoord>();
            foreach (Item Item in Items.ToList())
            {
                foreach (ThreeDCoord Tile in Item.GetAffectedTiles.Values.ToList())
                {
                    Tiles.Add(Tile);
                }
            }

            packet.WriteInteger(Tiles.Count);
            foreach (ThreeDCoord Tile in Tiles)
            {
                packet.WriteInteger(Tile.X);
                packet.WriteInteger(Tile.Y);
            }
        }
    }
}
