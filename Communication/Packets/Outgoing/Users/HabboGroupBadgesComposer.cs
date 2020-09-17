using StarBlue.HabboHotel.Groups;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Users
{
    internal class HabboGroupBadgesComposer : MessageComposer
    {
        private Dictionary<int, string> Badges { get; }
        private Group Group { get; }

        public HabboGroupBadgesComposer(Dictionary<int, string> Badges)
            : base(Composers.HabboGroupBadgesMessageComposer)
        {
            this.Badges = Badges;
        }

        public override void Compose(Composer packet)
        {
            if (Badges != null)
            {
                packet.WriteInteger(Badges.Count);
                foreach (KeyValuePair<int, string> badge in Badges)
                {
                    packet.WriteInteger(badge.Key);
                    packet.WriteString(badge.Value);
                }
            }
            else if (Group != null)
            {
                packet.WriteInteger(1); //count

                packet.WriteInteger(Group.Id);
                packet.WriteString(Group.Badge);
            }
            else
            {
                packet.WriteInteger(0);
            }
        }

        public HabboGroupBadgesComposer(Group Group)
            : base(Composers.HabboGroupBadgesMessageComposer)
        {
            this.Group = Group;
        }
    }
}
