using StarBlue.HabboHotel.Users;

namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    internal class InitHelperSessionChatComposer : MessageComposer
    {
        private Habbo Habbo1 { get; }
        private Habbo Habbo2 { get; }

        public InitHelperSessionChatComposer(Habbo Habbo1, Habbo Habbo2)
            : base(Composers.InitHelperSessionChatMessageComposer)
        {
            this.Habbo1 = Habbo1;
            this.Habbo2 = Habbo2;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Habbo1.Id);
            packet.WriteString(Habbo1.Username);
            packet.WriteString(Habbo1.Look);

            packet.WriteInteger(Habbo2.Id);
            packet.WriteString(Habbo2.Username);
            packet.WriteString(Habbo2.Look);
        }
    }
}
