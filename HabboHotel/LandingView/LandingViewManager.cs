using log4net;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.LandingView.Promotions;
using System;
using System.Collections.Generic;
using System.Data;

namespace StarBlue.HabboHotel.LandingView
{
    public class LandingViewManager
    {
        private static readonly ILog log = LogManager.GetLogger("StarBlue.HabboHotel.LandingView.LandingViewManager");

        private Dictionary<int, Promotion> _promotionItems;

        public LandingViewManager()
        {
            _promotionItems = new Dictionary<int, Promotion>();

            LoadPromotions();
        }

        public void LoadPromotions()
        {
            if (_promotionItems.Count > 0)
            {
                _promotionItems.Clear();
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `server_landing` ORDER BY `id` DESC");
                DataTable GetData = dbClient.GetTable();

                if (GetData != null)
                {
                    foreach (DataRow Row in GetData.Rows)
                    {
                        _promotionItems.Add(Convert.ToInt32(Row[0]), new Promotion((int)Row[0], Row[1].ToString(), Row[2].ToString(), Row[3].ToString(), Convert.ToInt32(Row[4]), Row[5].ToString(), Row[6].ToString()));
                    }
                }
            }

            log.Info(">> LandingView Manager -> READY! ");
        }

        public ICollection<Promotion> GetPromotionItems()
        {
            return _promotionItems.Values;
        }
    }
}