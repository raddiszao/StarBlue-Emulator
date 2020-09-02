using StarBlue.HabboHotel.Helpers;

namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    internal class CallForHelperWindowComposer : ServerPacket
    {
        public CallForHelperWindowComposer(bool IsHelper, int Category, string Message, int WaitTime)
            : base(ServerPacketHeader.CallForHelperWindowMessageComposer)
        {
            base.WriteBoolean(IsHelper);
            base.WriteInteger(Category);
            base.WriteString(Message);
            base.WriteInteger(WaitTime);
        }

        public CallForHelperWindowComposer(bool IsHelper, HelperCase Case)
            : base(ServerPacketHeader.CallForHelperWindowMessageComposer)
        {
            base.WriteBoolean(IsHelper);
            base.WriteInteger((int)Case.Type);
            base.WriteString(Case.Message);
            base.WriteInteger(Case.ReamingToExpire);
        }

    }
}
