using StarBlue.HabboHotel.Calendar;

namespace StarBlue.Communication.Packets.Outgoing.Campaigns
{
    internal class CalendarPrizesComposer : MessageComposer
    {
        private CalendarDay cday { get; }

        public CalendarPrizesComposer(CalendarDay cday)
            : base(Composers.CalendarPrizesMessageComposer)
        {
            this.cday = cday;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(true); // enable
            packet.WriteString(cday.ProductName); // productName: getProductData(x)
            packet.WriteString(cday.ImageLink);  // customImage: //habboo-a.akamaihd.net/c_images/ + x
            packet.WriteString(cday.ItemName); // getFloorItemDataByName(x)
        }
    }
}