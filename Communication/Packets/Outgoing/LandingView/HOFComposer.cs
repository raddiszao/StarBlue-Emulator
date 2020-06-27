using StarBlue.Communication.Packets.Incoming.LandingView;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.LandingView
{
    class HOFComposer : ServerPacket
    {
        public HOFComposer()
            : base(ServerPacketHeader.HOFComposer)
        {
            base.WriteString("halloffame");

            List<UserCompetition> usersHof = GetHallOfFame.getHofUsers();
            base.WriteInteger(usersHof.Count);
            foreach (UserCompetition user in usersHof)
            {
                base.WriteInteger(user.userId); //userID
                base.WriteString(user.userName);//userName
                base.WriteString(user.Look);//Look
                base.WriteInteger(user.Rank); //rank
                base.WriteInteger(user.Score);//?
            }
        }
    }
}
