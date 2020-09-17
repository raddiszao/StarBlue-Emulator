
using StarBlue.Communication.Packets.Outgoing.Inventory.Badges;

namespace StarBlue.Communication.Packets.Incoming.Inventory.Badges
{
    internal class GetBadgesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            Session.SendMessage(new BadgesComposer(Session.GetHabbo().GetBadgeComponent().GetBadges()));
        }
    }
}
