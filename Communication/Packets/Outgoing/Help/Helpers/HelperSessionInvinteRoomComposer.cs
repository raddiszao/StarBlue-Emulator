namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    internal class HelperSessionInvinteRoomComposer : MessageComposer
    {
        private int int1 { get; }
        private string str { get; }

        public HelperSessionInvinteRoomComposer(int int1, string str)
            : base(Composers.HelperSessionInvinteRoomMessageComposer)
        {
            this.int1 = int1;
            this.str = str;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(int1);
            packet.WriteString(str);
        }
    }
}
