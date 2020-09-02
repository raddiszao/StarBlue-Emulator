using StarBlue.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;


namespace StarBlue.HabboHotel.Catalog.Vouchers
{
    public class VoucherManager
    {
        private Dictionary<string, Voucher> _vouchers;

        public VoucherManager()
        {
            _vouchers = new Dictionary<string, Voucher>();
            Init();
        }

        public void Init()
        {
            if (_vouchers.Count > 0)
            {
                _vouchers.Clear();
            }

            DataTable GetVouchers = null;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `voucher`,`type`,`value`,`current_uses`,`max_uses` FROM `catalog_vouchers` WHERE `enabled` = '1'");
                GetVouchers = dbClient.GetTable();
            }

            if (GetVouchers != null)
            {
                foreach (DataRow Row in GetVouchers.Rows)
                {
                    _vouchers.Add(Convert.ToString(Row["voucher"]), new Voucher(Convert.ToString(Row["voucher"]), Convert.ToString(Row["type"]), Convert.ToInt32(Row["value"]), Convert.ToInt32(Row["current_uses"]), Convert.ToInt32(Row["max_uses"])));
                }
            }
        }

        public bool TryGetVoucher(string Code, out Voucher Voucher)
        {
            if (_vouchers.TryGetValue(Code, out Voucher))
            {
                return true;
            }

            return false;
        }

        public void AddVoucher(string Code, string Type, int Value, int Uses)
        {
            _vouchers.Add(Code, new Voucher(Code, Type, Value, 0, Uses));
        }
    }
}
