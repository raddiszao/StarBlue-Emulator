﻿using System.Collections.Generic;

namespace StarBlue.HabboHotel.Catalog.PredesignedRooms
{
    public class PredesignedContent
    {
        internal int CatalogId;
        internal Dictionary<int, int> Items;

        public PredesignedContent(int catalogId, Dictionary<int, int> items)
        {
            CatalogId = catalogId;
            Items = items;
        }
    }
}