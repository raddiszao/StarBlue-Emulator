using StarBlue.HabboHotel.Achievements;
using StarBlue.HabboHotel.GameClients;
using System;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Talents
{
    internal class TalentTrackComposer : MessageComposer
    {
        private GameClient session { get; }
        private string trackType { get; }
        private List<Talent> talents { get; }

        public TalentTrackComposer(GameClient session, string trackType, List<Talent> talents)
            : base(Composers.TalentTrackMessageComposer)
        {
            this.session = session;
            this.trackType = trackType;
            this.talents = talents;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(trackType);
            packet.WriteInteger(talents.Count);

            int failLevel = -1;

            foreach (Talent current in talents)
            {
                packet.WriteInteger(current.Level);
                int nm = failLevel == -1 ? 1 : 0; // TODO What does this mean?
                packet.WriteInteger(nm);

                List<Talent> children = StarBlueServer.GetGame().GetTalentManager().GetTalents(trackType, current.Id);

                packet.WriteInteger(children.Count);

                foreach (Talent child in children)
                {
                    UserAchievement achievment = session.GetHabbo().GetAchievementData(child.AchievementGroup);
                    if (child.GetAchievement() == null)
                    {
                        throw new NullReferenceException(
                            string.Format("The following talent achievement can't be found: {0}",
                                child.AchievementGroup));
                    }

                    // TODO Refactor What does num mean?!
                    int num = (failLevel != -1 && failLevel < child.Level)
                       ? 0
                       : (session.GetHabbo().GetAchievementData(child.AchievementGroup) == null)
                           ? 1
                           : (session.GetHabbo().GetAchievementData(child.AchievementGroup).Level >=
                              child.AchievementLevel)
                               ? 2
                               : 1;

                    packet.WriteInteger(child.GetAchievement().Id);
                    packet.WriteInteger(0); // TODO Magic constant

                    packet.WriteString(child.AchievementGroup + child.AchievementLevel);
                    packet.WriteInteger(num);

                    packet.WriteInteger(achievment != null ? achievment.Progress : 0);
                    packet.WriteInteger(child.GetAchievement() == null ? 0
                        : child.GetAchievement().Levels[child.AchievementLevel].Requirement);

                    if (num != 2 && failLevel == -1)
                    {
                        failLevel = child.Level;
                    }
                }

                packet.WriteInteger(0); // TODO Magic constant

                // TODO Type should be enum?
                if (current.Type == "citizenship" && current.Level == 4)
                {
                    packet.WriteInteger(2);
                    packet.WriteString("HABBO_CLUB_VIP_7_DAYS");
                    packet.WriteInteger(7);
                    packet.WriteString(current.Prize); // TODO Hardcoded stuff
                    packet.WriteInteger(0);
                }
                else
                {
                    packet.WriteInteger(1);
                    packet.WriteString(current.Prize);
                    packet.WriteInteger(0);
                }
            }
        }
    }
}
