using StarBlue.HabboHotel.Navigator;
using System.Collections.Generic;


namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class UserFlatCatsComposer : MessageComposer
    {
        private ICollection<SearchResultList> Categories { get; }
        private int Rank { get; }

        public UserFlatCatsComposer(ICollection<SearchResultList> Categories, int Rank)
            : base(Composers.UserFlatCatsMessageComposer)
        {
            this.Categories = Categories;
            this.Rank = Rank;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Categories.Count);
            foreach (SearchResultList Cat in Categories)
            {
                packet.WriteInteger(Cat.Id);
                packet.WriteString(Cat.PublicName);
                packet.WriteBoolean(Cat.RequiredRank <= Rank);
                packet.WriteBoolean(false);
                packet.WriteString("");
                packet.WriteString("");
                packet.WriteBoolean(false);
            }
        }
    }
}
