using StarBlue.HabboHotel.Users;
using StarBlue.HabboHotel.Users.Badges;
using System;
using System.Data;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Users
{
    class HabboUserBadgesComposer : ServerPacket
    {
        public HabboUserBadgesComposer(Habbo Habbo, int UserId = 0)
            : base(ServerPacketHeader.HabboUserBadgesMessageComposer)
        {
            base.WriteInteger(Habbo.GetClient() == null ? UserId : Habbo.Id);

            if (Habbo.GetClient() != null)
            {
                base.WriteInteger(Habbo.GetBadgeComponent().EquippedCount);

                foreach (Badge Badge in Habbo.GetBadgeComponent().GetBadges().ToList())
                {
                    if (Badge.Slot <= 0)
                    {
                        continue;
                    }

                    base.WriteInteger(Badge.Slot);
                    base.WriteString(Badge.Code);
                }
            }
            else
            {
                DataTable badges = Habbo.GetBadgeComponent().GetBadgesOfflineUser(UserId);
                base.WriteInteger(badges.Rows.Count);
                if (badges.Rows.Count > 0)
                {
                    foreach (DataRow Row in badges.Rows)
                    {
                        base.WriteInteger(Convert.ToInt32(Row["badge_slot"]));
                        base.WriteString(Convert.ToString(Row["badge_id"]));
                    }
                }
            }
        }
    }
}
