namespace StarBlue.Communication.Packets.Outgoing.Talents
{
    internal class TalentTrackLevelComposer : ServerPacket
    {
        public TalentTrackLevelComposer()
            : base(ServerPacketHeader.TalentTrackLevelMessageComposer)
        {
            base.WriteString("citizenship");
            base.WriteInteger(0);
            base.WriteInteger(4);
        }
    }
}