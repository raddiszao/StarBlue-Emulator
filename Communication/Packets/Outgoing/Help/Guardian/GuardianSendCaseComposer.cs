using StarBlue.HabboHotel.Users;

namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    internal class GuardianSendCaseComposer : MessageComposer
    {
        private int seconds { get; }
        private Habbo reported { get; }

        public GuardianSendCaseComposer(int seconds, Habbo reported)
            : base(Composers.GuardianSendCaseMessageComposer)
        {
            this.seconds = seconds;
            this.reported = reported;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(1); // length
            packet.WriteInteger(3); // type: Bully
            packet.WriteInteger(15); // timer sec
            packet.WriteBoolean(false); // false = usuario, true = null
            //if (user != null)
            //{
            packet.WriteString(reported.Username);
            packet.WriteString(reported.Look);
            packet.WriteString((reported.CurrentRoom == null) ? "" : reported.CurrentRoom.RoomData.Name);
            //}
        }
    }
}
