using System;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Poll
{
    class QuickPollMessageComposer : ServerPacket
    {
        public QuickPollMessageComposer(String question)
            : base(ServerPacketHeader.QuickPollMessageComposer)
        {
            base.WriteString("");
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(1);   //duration
            base.WriteInteger(-1);  //id
            base.WriteInteger(120); //number
            base.WriteInteger(3);
            base.WriteString(question);
        }
    }
}