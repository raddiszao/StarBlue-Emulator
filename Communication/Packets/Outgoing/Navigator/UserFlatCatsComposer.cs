using StarBlue.HabboHotel.Navigator;
using System.Collections.Generic;


namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class UserFlatCatsComposer : ServerPacket
    {
        public UserFlatCatsComposer(ICollection<SearchResultList> Categories, int Rank)
            : base(ServerPacketHeader.UserFlatCatsMessageComposer)
        {
            base.WriteInteger(Categories.Count);
            foreach (SearchResultList Cat in Categories)
            {
                base.WriteInteger(Cat.Id);
                base.WriteString(Cat.PublicName);
                base.WriteBoolean(Cat.RequiredRank <= Rank);
                base.WriteBoolean(false);
                base.WriteString("");
                base.WriteString("");
                base.WriteBoolean(false);
            }
        }
    }
}
