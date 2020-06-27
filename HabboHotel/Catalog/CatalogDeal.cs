using StarBlue.HabboHotel.Items;
using System.Collections.Generic;

namespace StarBlue.HabboHotel.Catalog
{
    public class CatalogDeal
    {
        public int Id { get; set; }
        public int PageId { get; set; }
        public List<CatalogItem> ItemDataList { get; private set; }
        public string DisplayName { get; set; }
        public int CostCredits { get; set; }
        public int CostPixels { get; set; }

        public CatalogDeal(int Id, int PageId, string Items, string DisplayName, int Credits, int Pixels, ItemDataManager ItemDataManager)
        {
            this.Id = Id;
            this.PageId = PageId;
            this.DisplayName = DisplayName;
            ItemDataList = new List<CatalogItem>();

            string[] SplitItems = Items.Split(';');
            foreach (string Split in SplitItems)
            {
                string[] Item = Split.Split('*');
                if (!int.TryParse(Item[0], out int ItemId) || !int.TryParse(Item[1], out int Amount))
                {
                    continue;
                }

                if (!ItemDataManager.GetItem(ItemId, out ItemData Data))
                {
                    continue;
                }

                ItemDataList.Add(new CatalogItem(0, ItemId, Data, string.Empty, PageId, CostCredits, CostPixels, 0, Amount, 0, 0, false, "", "", 0, 0, 0));
            }

            CostCredits = Credits;
            CostPixels = Pixels;
        }
    }
}
