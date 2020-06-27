namespace StarBlue.HabboHotel.Catalog.Vouchers
{
    public static class VoucherUtility
    {
        public static VoucherType GetType(string Type)
        {
            switch (Type)
            {
                default:
                case "credit":
                    return VoucherType.CREDIT;
                case "ducket":
                    return VoucherType.DUCKET;
                case "diamond":
                    return VoucherType.DIAMOND;
                case "item":
                    return VoucherType.ITEM;
                case "honor":
                    return VoucherType.HONOR;
            }
        }

        public static string FromType(VoucherType Type)
        {
            switch (Type)
            {
                default:
                case VoucherType.CREDIT:
                    return "credit";
                case VoucherType.DUCKET:
                    return "ducket";
                case VoucherType.DIAMOND:
                    return "diamond";
                case VoucherType.ITEM:
                    return "item";
                case VoucherType.HONOR:
                    return "honor";
            }
        }
    }
}
