using StarBlue.HabboHotel.Achievements;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Inventory.Achievements
{
    internal class BadgeDefinitionsComposer : ServerPacket
    {
        public BadgeDefinitionsComposer(Dictionary<string, Achievement> Achievements)
            : base(ServerPacketHeader.BadgeDefinitionsMessageComposer)
        {
            base.WriteInteger(Achievements.Count);

            foreach (Achievement Achievement in Achievements.Values)
            {
                base.WriteString(Achievement.GroupName.Replace("ACH_", ""));
                base.WriteInteger(Achievement.Levels.Count);
                foreach (AchievementLevel Level in Achievement.Levels.Values)
                {
                    base.WriteInteger(Level.Level);
                    base.WriteInteger(Level.Requirement);
                }
            }
        }
    }
}
