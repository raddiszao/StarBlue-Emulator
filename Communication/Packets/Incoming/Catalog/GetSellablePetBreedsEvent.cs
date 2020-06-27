using StarBlue.Communication.Packets.Outgoing.Catalog;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    public class GetSellablePetBreedsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Type = Packet.PopString();
            int PetId = StarBlueServer.GetGame().GetCatalog().GetPetRaceManager().GetPetId(Type, out string PacketType);

            Session.SendMessage(new SellablePetBreedsComposer(PacketType, PetId, StarBlueServer.GetGame().GetCatalog().GetPetRaceManager().GetRacesForRaceId(PetId)));
        }
    }
}