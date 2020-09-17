namespace StarBlue.Communication.Packets.Outgoing.Rooms.Poll
{
    internal class QuickPollMessageComposer : MessageComposer
    {
        private string question { get; }

        public QuickPollMessageComposer(string question)
            : base(Composers.QuickPollMessageComposer)
        {
            this.question = question;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString("");
            packet.WriteInteger(0);
            packet.WriteInteger(0);
            packet.WriteInteger(1);   //duration
            packet.WriteInteger(-1);  //id
            packet.WriteInteger(120); //number
            packet.WriteInteger(3);
            packet.WriteString(question);
        }
    }
}