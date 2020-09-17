
using StarBlue.HabboHotel.Users.Messenger;

namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    internal class InstantMessageErrorComposer : MessageComposer
    {
        public MessengerMessageErrors Error { get; }
        public int Target { get; }

        public InstantMessageErrorComposer(MessengerMessageErrors Error, int Target)
            : base(Composers.InstantMessageErrorMessageComposer)
        {
            this.Error = Error;
            this.Target = Target;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(MessengerMessageErrorsUtility.GetMessageErrorPacketNum(Error));
            packet.WriteInteger(Target);
            packet.WriteString("");
        }
    }
}
