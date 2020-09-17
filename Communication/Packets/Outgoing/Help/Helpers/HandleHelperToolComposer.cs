using StarBlue.HabboHotel.Helpers;

namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    internal class HandleHelperToolComposer : MessageComposer
    {
        private bool onDuty { get; }

        public HandleHelperToolComposer(bool onDuty)
            : base(Composers.HandleHelperToolMessageComposer)
        {
            this.onDuty = onDuty;

        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(onDuty);
            packet.WriteInteger(HelperToolsManager.GuideCount);
            packet.WriteInteger(HelperToolsManager.HelperCount);
            packet.WriteInteger(HelperToolsManager.GuardianCount);
        }
    }
}
