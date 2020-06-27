using StarBlue.HabboHotel.Items;
using System;

namespace StarBlue.HabboHotel.Catalog.FurniMatic
{
    public class FurniMaticRewards
    {
        public Int32 DisplayId;
        public Int32 BaseId;
        public Int32 Level;

        public FurniMaticRewards(int displayId, int baseId, int level)
        {
            DisplayId = displayId;
            BaseId = baseId;
            Level = level;
        }

        public ItemData GetBaseItem()
        {
            if (StarBlueServer.GetGame().GetItemManager().GetItem(BaseId, out ItemData data))
            {
                return data;
            }

            return null;
        }
    }
}