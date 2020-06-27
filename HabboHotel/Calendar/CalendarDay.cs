namespace StarBlue.HabboHotel.Calendar
{
    public class CalendarDay
    {
        public int Day;
        public string Gift;
        public string ProductName;
        public string ImageLink;
        public string ItemName;

        public CalendarDay(int day, string gift, string productname, string imagelink, string itemname)
        {
            Day = day;
            Gift = gift;
            ProductName = productname;
            ImageLink = imagelink;
            ItemName = itemname;
        }
    }
}
