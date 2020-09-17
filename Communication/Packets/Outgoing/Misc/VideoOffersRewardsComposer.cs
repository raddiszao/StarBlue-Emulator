namespace StarBlue.Communication.Packets.Outgoing.Handshake
{
    internal class VideoOffersRewardsComposer : MessageComposer
    {
        public VideoOffersRewardsComposer(/*int Id, string Type, string Message*/)
            : base(Composers.VideoOffersRewardsMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString("start_video");
            packet.WriteInteger(0);
            packet.WriteString("");
            packet.WriteString("");
        }
    }
}

