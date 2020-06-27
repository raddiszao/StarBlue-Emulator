namespace StarBlue.Communication.Packets.Outgoing.Rooms.Camera
{
    public class CameraFinishParticipateCompetitionMessageComposer : ServerPacket
    {
        public CameraFinishParticipateCompetitionMessageComposer(bool UnknownBoolean, string UnknownString)
            : base(ServerPacketHeader.CameraFinishParticipateCompetitionMessageComposer)
        {
            WriteBoolean(UnknownBoolean);
            WriteString(UnknownString);
        }
    }
}