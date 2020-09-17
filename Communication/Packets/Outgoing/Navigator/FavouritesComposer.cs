using System.Collections;

namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class FavouritesComposer : MessageComposer
    {
        private ArrayList favouriteIDs { get; }

        public FavouritesComposer(ArrayList favouriteIDs)
            : base(Composers.FavouritesMessageComposer)
        {
            this.favouriteIDs = favouriteIDs;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(50);
            packet.WriteInteger(favouriteIDs.Count);

            foreach (int Id in favouriteIDs.ToArray())
            {
                packet.WriteInteger(Id);
            }
        }
    }
}
