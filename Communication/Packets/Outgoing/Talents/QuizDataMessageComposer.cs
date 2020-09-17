using StarBlue.HabboHotel.Users;
using System;

namespace StarBlue.Communication.Packets.Outgoing.Talents
{
    internal class QuizDataMessageComposer : MessageComposer
    {
        private Habbo Habbo { get; }
        private string HabboType { get; }

        public QuizDataMessageComposer(Habbo Habbo, string HabboType)
            : base(Composers.QuizDataMessageComposer)
        {
            this.Habbo = Habbo;
            this.HabboType = HabboType;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(HabboType);
            packet.WriteInteger(5); // longitud.                
            for (int i = 0; i < 5; i++)
            {
                int rndNumber = new Random().Next(10);
                if (Habbo._HabboQuizQuestions.Contains(rndNumber))
                {
                    for (int ii = 0; ii < 10; ii++)
                    {
                        if (!Habbo._HabboQuizQuestions.Contains(ii))
                        {
                            rndNumber = ii;
                            break;
                        }
                    }
                }
                Habbo._HabboQuizQuestions.Add(rndNumber);
                packet.WriteInteger(rndNumber);
            }
        }
    }
}
