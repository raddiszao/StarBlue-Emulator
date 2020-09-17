using StarBlue.Communication.Packets.Outgoing.Talents;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.Talents
{
    internal class CheckQuizType : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            string HabboType = Packet.PopString();
            if (HabboType == "HabboWay1")
            {
                Session.GetHabbo()._HabboQuizQuestions = new List<int>(5);
                Session.SendMessage(new QuizDataMessageComposer(Session.GetHabbo(), HabboType));
            }
            else if (HabboType == "SafetyQuiz1")
            {
                StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_SafetyQuizGraduate", 1);
            }
        }
    }
}
