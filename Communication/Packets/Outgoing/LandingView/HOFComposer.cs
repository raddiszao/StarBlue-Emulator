using StarBlue.Communication.Packets.Incoming.LandingView;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.LandingView
{
    internal class HOFComposer : MessageComposer
    {
        public HOFComposer()
            : base(Composers.HOFComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString("halloffame");

            List<UserCompetition> usersHof = GetHallOfFame.getHofUsers();
            packet.WriteInteger(usersHof.Count);
            foreach (UserCompetition user in usersHof)
            {
                packet.WriteInteger(user.userId); //userID
                packet.WriteString(user.userName);//userName
                packet.WriteString(user.Look);//Look
                packet.WriteInteger(user.Rank); //rank
                packet.WriteInteger(user.Score);//?
            }
        }
    }
}
