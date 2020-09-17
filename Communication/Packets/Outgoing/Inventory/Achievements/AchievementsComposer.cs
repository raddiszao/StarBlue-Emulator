using StarBlue.HabboHotel.Achievements;
using StarBlue.HabboHotel.Users;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Inventory.Achievements
{
    internal class AchievementsComposer : MessageComposer
    {
        public List<Achievement> Achievements { get; }
        public Habbo Habbo { get; }

        public AchievementsComposer(Habbo habbo, List<Achievement> Achievements)
            : base(Composers.AchievementsMessageComposer)
        {
            this.Achievements = Achievements;
            this.Habbo = habbo;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Achievements.Count);
            foreach (Achievement Achievement in Achievements)
            {
                UserAchievement UserData = Habbo.GetAchievementData(Achievement.GroupName);
                int TargetLevel = (UserData != null ? UserData.Level + 1 : 1);
                int TotalLevels = Achievement.Levels.Count;

                TargetLevel = (TargetLevel > TotalLevels ? TotalLevels : TargetLevel);

                AchievementLevel TargetLevelData = Achievement.Levels[TargetLevel];
                packet.WriteInteger(Achievement.Id); // Unknown (ID?)
                packet.WriteInteger(TargetLevel); // Target level
                packet.WriteString(Achievement.GroupName + TargetLevel); // Target name/desc/badge

                packet.WriteInteger(1);
                packet.WriteInteger(TargetLevelData.Requirement); // Progress req/target          
                packet.WriteInteger(TargetLevelData.RewardPixels);

                packet.WriteInteger(0); // Type of reward
                packet.WriteInteger(UserData != null ? UserData.Progress : 0); // Current progress

                packet.WriteBoolean(UserData != null ? (UserData.Level >= TotalLevels) : false);// Set 100% completed(??)
                packet.WriteString(Achievement.Category); // Category
                packet.WriteString(string.Empty);
                packet.WriteInteger(TotalLevels); // Total amount of levels 
                packet.WriteInteger(0);
            }
            packet.WriteString("");
        }
    }
}