using StarBlue.HabboHotel.Users;
using StarBlue.HabboHotel.Users.Badges;
using System;
using System.Data;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Users
{
    internal class HabboUserBadgesComposer : MessageComposer
    {
        private Habbo Habbo { get; }
        private int UserId { get; }

        public HabboUserBadgesComposer(Habbo Habbo, int UserId = 0)
            : base(Composers.HabboUserBadgesMessageComposer)
        {
            this.Habbo = Habbo;
            this.UserId = UserId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Habbo.GetClient() == null ? UserId : Habbo.Id);

            if (Habbo.GetClient() != null)
            {
                packet.WriteInteger(Habbo.GetBadgeComponent().EquippedCount);

                foreach (Badge Badge in Habbo.GetBadgeComponent().GetBadges().ToList())
                {
                    if (Badge.Slot <= 0)
                    {
                        continue;
                    }

                    packet.WriteInteger(Badge.Slot);
                    packet.WriteString(Badge.Code);
                }
            }
            else
            {
                DataTable badges = Habbo.GetBadgeComponent().GetBadgesOfflineUser(UserId);
                packet.WriteInteger(badges.Rows.Count);
                if (badges.Rows.Count > 0)
                {
                    foreach (DataRow Row in badges.Rows)
                    {
                        packet.WriteInteger(Convert.ToInt32(Row["badge_slot"]));
                        packet.WriteString(Convert.ToString(Row["badge_id"]));
                    }
                }
            }
        }
    }
}
