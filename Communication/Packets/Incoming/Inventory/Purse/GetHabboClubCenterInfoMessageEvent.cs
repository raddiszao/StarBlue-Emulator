using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;

namespace StarBlue.Communication.Packets.Incoming.Inventory.Purse
{
    internal class GetHabboClubCenterInfoMessageEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new GetHabboClubCenterInfoMessageComposer(Session));
        }
    }
}