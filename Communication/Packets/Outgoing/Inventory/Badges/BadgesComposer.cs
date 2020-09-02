using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Users.Badges;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Inventory.Badges
{
    internal class BadgesComposer : ServerPacket
    {
        public BadgesComposer(GameClient Session)
            : base(ServerPacketHeader.BadgesMessageComposer)
        {
            List<Badge> EquippedBadges = new List<Badge>();

            base.WriteInteger(Session.GetHabbo().GetBadgeComponent().Count);
            foreach (Badge Badge in Session.GetHabbo().GetBadgeComponent().GetBadges().ToList())
            {
                base.WriteInteger(1);
                base.WriteString(Badge.Code);

                if (Badge.Slot > 0)
                {
                    EquippedBadges.Add(Badge);
                }
            }

            base.WriteInteger(EquippedBadges.Count);
            foreach (Badge Badge in EquippedBadges)
            {
                base.WriteInteger(Badge.Slot);
                base.WriteString(Badge.Code);
            }
        }
    }
}
