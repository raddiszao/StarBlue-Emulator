using StarBlue.HabboHotel.Achievements;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Inventory.Achievements
{
    internal class BadgeDefinitionsComposer : MessageComposer
    {
        public Dictionary<string, Achievement> Achievements { get; }

        public BadgeDefinitionsComposer(Dictionary<string, Achievement> Achievements)
            : base(Composers.BadgeDefinitionsMessageComposer)
        {
            this.Achievements = Achievements;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Achievements.Count);

            foreach (Achievement Achievement in Achievements.Values)
            {
                packet.WriteString(Achievement.GroupName.Replace("ACH_", ""));
                packet.WriteInteger(Achievement.Levels.Count);
                foreach (AchievementLevel Level in Achievement.Levels.Values)
                {
                    packet.WriteInteger(Level.Level);
                    packet.WriteInteger(Level.Requirement);
                }
            }
        }
    }
}
