using StarBlue.Communication.Packets.Outgoing;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.Talents
{
    class PostQuizAnswersMessage : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            string HabboType = Packet.PopString();
            if (HabboType != "HabboWay1")
            {
                return;
            }

            int HabboQuestions = Packet.PopInt();
            List<int> errors = new List<int>(5);

            var answer = new ServerPacket(ServerPacketHeader.PostQuizAnswersMessageComposer);
            answer.WriteString(HabboType);
            for (int i = 0; i < HabboQuestions; i++)
            {
                int QuestionId = Session.GetHabbo()._HabboQuizQuestions[i];
                int respuesta = Packet.PopInt();
                if (!Quiz.CorrectAnswer(QuestionId, respuesta))
                {
                    errors.Add(QuestionId);
                }
            }
            answer.WriteInteger(errors.Count);
            foreach (int error in errors)
            {
                answer.WriteInteger(error);
            }
            Session.SendMessage(answer);

            if (errors.Count == 0)
            {
                StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_HabboWayGraduate", 1);
            }
        }
    }
}
