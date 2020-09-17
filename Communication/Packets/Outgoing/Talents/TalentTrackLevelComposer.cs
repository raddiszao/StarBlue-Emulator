namespace StarBlue.Communication.Packets.Outgoing.Talents
{
    internal class TalentTrackLevelComposer : MessageComposer
    {
        public TalentTrackLevelComposer()
            : base(Composers.TalentTrackLevelMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString("citizenship");
            packet.WriteInteger(0);
            packet.WriteInteger(4);
        }
    }
}