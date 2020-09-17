namespace StarBlue.Communication.Packets.Outgoing.Help
{
    internal class SendBullyReportComposer : MessageComposer
    {
        public SendBullyReportComposer()
            : base(Composers.SendBullyReportMessageComposer)
        {

        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(0);//0-3, sends 0 on Habbo for this purpose.
        }
    }
}
