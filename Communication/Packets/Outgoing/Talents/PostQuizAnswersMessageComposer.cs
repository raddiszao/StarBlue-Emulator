using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Incoming.Talents;
using StarBlue.HabboHotel.Users;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Talents
{
    internal class PostQuizAnswersMessageComposer : MessageComposer
    {
        private Habbo Habbo { get; }
        private string HabboType { get; }
        private MessageEvent ClientPacket { get; }
        private int HabboQuestions { get; }

        public PostQuizAnswersMessageComposer(Habbo Habbo, string HabboType, MessageEvent ClientPacket, int HabboQuestions)
            : base(Composers.PostQuizAnswersMessageComposer)
        {
            this.Habbo = Habbo;
            this.HabboType = HabboType;
            this.ClientPacket = ClientPacket;
            this.HabboQuestions = HabboQuestions;
        }

        public override void Compose(Composer packet)
        {
            List<int> errors = new List<int>(5);
            packet.WriteString(HabboType);
            for (int i = 0; i < HabboQuestions; i++)
            {
                int QuestionId = Habbo._HabboQuizQuestions[i];
                int respuesta = ClientPacket.PopInt();
                if (!Quiz.CorrectAnswer(QuestionId, respuesta))
                {
                    errors.Add(QuestionId);
                }
            }

            packet.WriteInteger(errors.Count);
            foreach (int error in errors)
            {
                packet.WriteInteger(error);
            }

            if (errors.Count == 0)
            {
                StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Habbo.GetClient(), "ACH_HabboWayGraduate", 1);
            }
        }
    }
}
