namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    internal class NewConsoleMessageComposer : MessageComposer
    {
        public int Sender { get; }
        public string Message { get; }
        public int Time { get; }

        public NewConsoleMessageComposer(int Sender, string Message, int Time = 0)
            : base(Composers.NewConsoleMessageMessageComposer)
        {
            this.Sender = Sender;
            this.Message = Message;
            this.Time = Time;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Sender);
            packet.WriteString(Message);
            packet.WriteInteger(Time);
        }
    }

    internal class FuckingConsoleMessageComposer : MessageComposer
    {
        public int Sender { get; }
        public string Message { get; }
        public string Data { get; }

        public FuckingConsoleMessageComposer(int Sender, string Message, string Data)
            : base(Composers.NewConsoleMessageMessageComposer)
        {
            this.Sender = Sender;
            this.Message = Message;
            this.Data = Data;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Sender);
            packet.WriteString(Message);
            packet.WriteInteger(0);
            packet.WriteString(Data);
        }
    }
}
