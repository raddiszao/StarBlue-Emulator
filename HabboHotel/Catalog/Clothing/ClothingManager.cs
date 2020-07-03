using Database_Manager.Database.Session_Details.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace StarBlue.HabboHotel.Catalog.Clothing
{
    public class ClothingManager
    {
        private Dictionary<int, ClothingItem> _clothing;

        public ClothingManager()
        {
            _clothing = new Dictionary<int, ClothingItem>();

            Init();
        }

        public void Init()
        {
            if (_clothing.Count > 0)
            {
                _clothing.Clear();
            }

            DataTable GetClothing = null;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`clothing_name`,`clothing_parts` FROM `catalog_clothing`");
                GetClothing = dbClient.GetTable();
            }

            if (GetClothing != null)
            {
                foreach (DataRow Row in GetClothing.Rows)
                {
                    _clothing.Add(Convert.ToInt32(Row["id"]), new ClothingItem(Convert.ToInt32(Row["id"]), Convert.ToString(Row["clothing_name"]), Convert.ToString(Row["clothing_parts"])));
                }
            }
        }

        public bool TryGetClothing(int ItemId, out ClothingItem Clothing)
        {
            if (_clothing.TryGetValue(ItemId, out Clothing))
            {
                return true;
            }

            return false;
        }

        public ICollection<ClothingItem> GetClothingAllParts => _clothing.Values;
    }
}
