using StarBlue.HabboHotel.Users.Badges;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Inventory.Badges
{
    internal class BadgesComposer : MessageComposer
    {
        public ICollection<Badge> Badges { get; }

        public BadgesComposer(ICollection<Badge> Badges)
            : base(Composers.BadgesMessageComposer)
        {
            this.Badges = Badges;
        }

        public override void Compose(Composer packet)
        {
            List<Badge> EquippedBadges = new List<Badge>();

            packet.WriteInteger(Badges.Count);
            foreach (Badge Badge in Badges)
            {
                packet.WriteInteger(1);
                packet.WriteString(Badge.Code);

                if (Badge.Slot > 0)
                    EquippedBadges.Add(Badge);
            }

            packet.WriteInteger(EquippedBadges.Count);
            foreach (Badge Badge in EquippedBadges)
            {
                packet.WriteInteger(Badge.Slot);
                packet.WriteString(Badge.Code);
            }
        }
    }
}
