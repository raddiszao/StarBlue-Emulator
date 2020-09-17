using StarBlue.HabboHotel.Achievements;
using StarBlue.HabboHotel.GameClients;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.GameCenter
{
    internal class GameAchievementListComposer : MessageComposer
    {
        private GameClient Session { get; }
        private ICollection<Achievement> Achievements { get; }
        private int GameId { get; }

        public GameAchievementListComposer(GameClient Session, ICollection<Achievement> Achievements, int GameId)
            : base(Composers.GameAchievementListMessageComposer)
        {
            this.Session = Session;
            this.Achievements = Achievements;
            this.GameId = GameId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(GameId);
            packet.WriteInteger(Achievements.Count);
            foreach (Achievement Ach in Achievements.ToList())
            {
                UserAchievement UserData = Session.GetHabbo().GetAchievementData(Ach.GroupName);
                int TargetLevel = (UserData != null ? UserData.Level + 1 : 1);

                AchievementLevel TargetLevelData = Ach.Levels[TargetLevel];

                packet.WriteInteger(Ach.Id); // ach id
                packet.WriteInteger(TargetLevel); // target level
                packet.WriteString(Ach.GroupName + TargetLevel); // badge
                packet.WriteInteger(TargetLevelData.Requirement); // requirement
                packet.WriteInteger(TargetLevelData.Requirement); // requirement
                packet.WriteInteger(TargetLevelData.RewardPixels); // pixels
                packet.WriteInteger(0); // ach score
                packet.WriteInteger(UserData != null ? UserData.Progress : 0); // Current progress
                packet.WriteBoolean(UserData != null ? (UserData.Level >= Ach.Levels.Count) : false); // Set 100% completed(??)
                packet.WriteString(Ach.Category);
                packet.WriteString("basejump");
                packet.WriteInteger(0); // total levels
                packet.WriteInteger(0);
            }
            packet.WriteString("");
        }
    }
}
