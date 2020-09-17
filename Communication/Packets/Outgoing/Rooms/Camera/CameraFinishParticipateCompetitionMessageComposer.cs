namespace StarBlue.Communication.Packets.Outgoing.Rooms.Camera
{
    public class CameraFinishParticipateCompetitionMessageComposer : MessageComposer
    {
        private bool UnknownBoolean { get; }
        private string UnknownString { get; }

        public CameraFinishParticipateCompetitionMessageComposer(bool UnknownBoolean, string UnknownString)
            : base(Composers.CameraFinishParticipateCompetitionMessageComposer)
        {
            this.UnknownBoolean = UnknownBoolean;
            this.UnknownString = UnknownString;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(UnknownBoolean);
            packet.WriteString(UnknownString);
        }
    }
}