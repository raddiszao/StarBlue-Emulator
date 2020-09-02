namespace StarBlue.Communication.Packets.Outgoing.Handshake
{
    internal class VideoOffersRewardsComposer : ServerPacket
    {
        public VideoOffersRewardsComposer(/*int Id, string Type, string Message*/)
            : base(ServerPacketHeader.VideoOffersRewardsMessageComposer)
        {
            base.WriteString("start_video");
            base.WriteInteger(0);
            base.WriteString("");
            base.WriteString("");
        }
    }
}

