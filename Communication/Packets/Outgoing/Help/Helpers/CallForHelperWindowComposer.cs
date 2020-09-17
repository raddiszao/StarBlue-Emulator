using StarBlue.HabboHotel.Helpers;

namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    internal class CallForHelperWindowComposer : MessageComposer
    {
        private bool IsHelper { get; }
        private int Category { get; }
        private string Message { get; }
        private int WaitTime { get; }
        private HelperCase Case { get; }


        public CallForHelperWindowComposer(bool IsHelper, int Category, string Message, int WaitTime)
            : base(Composers.CallForHelperWindowMessageComposer)
        {
            this.IsHelper = IsHelper;
            this.Category = Category;
            this.Message = Message;
            this.WaitTime = WaitTime;
        }

        public CallForHelperWindowComposer(bool IsHelper, HelperCase Case)
            : base(Composers.CallForHelperWindowMessageComposer)
        {
            this.IsHelper = IsHelper;
            this.Case = Case;
        }

        public override void Compose(Composer packet)
        {
            if (Case == null)
            {
                packet.WriteBoolean(IsHelper);
                packet.WriteInteger(Category);
                packet.WriteString(Message);
                packet.WriteInteger(WaitTime);
            }
            else
            {
                packet.WriteBoolean(IsHelper);
                packet.WriteInteger((int)Case.Type);
                packet.WriteString(Case.Message);
                packet.WriteInteger(Case.ReamingToExpire);
            }
        }
    }
}
