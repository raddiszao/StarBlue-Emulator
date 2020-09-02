
using StarBlue.HabboHotel.Users.Messenger;

namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    internal class InstantMessageErrorComposer : ServerPacket
    {
        public InstantMessageErrorComposer(MessengerMessageErrors Error, int Target)
            : base(ServerPacketHeader.InstantMessageErrorMessageComposer)
        {
            base.WriteInteger(MessengerMessageErrorsUtility.GetMessageErrorPacketNum(Error));
            base.WriteInteger(Target);
            base.WriteString("");
        }
    }
}
