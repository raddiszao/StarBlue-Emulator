using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Catalog;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Outgoing.Users
{
    internal class GetClubComposer : MessageComposer
    {
        private CatalogPage Page { get; }
        private MessageEvent ClientPacket { get; }
        private GameClient Session { get; }

        public GetClubComposer(CatalogPage Page, MessageEvent ClientPacket, GameClient Session) : base(Composers.GetClubComposer)
        {
            this.Page = Page;
            this.ClientPacket = ClientPacket;
            this.Session = Session;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Page.Items.Values.Count);

            foreach (CatalogItem catalogItem in Page.Items.Values)
            {
                catalogItem.SerializeClub(packet, Session);
            }

            packet.WriteInteger(ClientPacket.PopInt());
        }
    }
}