using StarBlue.HabboHotel.Catalog;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class OpenBoxTargetedOffer : MessageComposer
    {
        private bool Open { get; }

        private int ID { get; }
        private string Code { get; }
        private string[] Price { get; }
        private int MoneyType { get; }
        private int Limit { get; }
        private int Time { get; }
        private string Title { get; }
        private string Image { get; }
        private string Description { get; }
        private string Icon { get; }
        private List<TargetedItems> Products { get; }

        public OpenBoxTargetedOffer(bool Open, int Id, string Code, string[] Price, int MoneyType, int Limit, int Time, string Title, string Image, string Description, string Icon, List<TargetedItems> Products)
            : base(Composers.openBoxTargetedOffert)
        {
            this.Open = Open;
            this.ID = Id;
            this.Code = Code;
            this.Price = Price;
            this.MoneyType = MoneyType;
            this.Limit = Limit;
            this.Time = Time;
            this.Title = Title;
            this.Image = Image;
            this.Description = Description;
            this.Icon = Icon;
            this.Products = Products;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Open ? 4 : 1);
            packet.WriteInteger(ID);
            packet.WriteString(Code);
            packet.WriteString(Code);
            packet.WriteInteger(int.Parse(Price[0]));
            packet.WriteInteger(int.Parse(Price[1]));
            packet.WriteInteger(MoneyType);
            packet.WriteInteger(Limit);
            packet.WriteInteger(Time);
            packet.WriteString(Title);
            packet.WriteString(Description);
            packet.WriteString(Image);
            packet.WriteString(Icon);
            packet.WriteInteger(0);
            packet.WriteInteger(Products.Count);
            foreach (TargetedItems product in Products)
            {
                packet.WriteString(string.Empty);
            }
        }
    }
}
