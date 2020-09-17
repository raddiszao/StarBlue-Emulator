using StarBlue.HabboHotel.Navigator;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class NavigatorFlatCatsComposer : MessageComposer
    {
        private ICollection<SearchResultList> Categories { get; }
        private int Rank { get; }

        public NavigatorFlatCatsComposer(ICollection<SearchResultList> Categories, int Rank)
            : base(Composers.NavigatorFlatCatsMessageComposer)
        {
            this.Categories = Categories;
            this.Rank = Rank;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Categories.Count);
            foreach (SearchResultList Category in Categories.ToList())
            {
                packet.WriteInteger(Category.Id);
                packet.WriteString(Category.PublicName);
                packet.WriteBoolean(true);//TODO
            }
        }
    }
}