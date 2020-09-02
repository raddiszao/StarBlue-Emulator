namespace StarBlue.Communication.Packets.Outgoing.WebSocket
{
    internal class YoutubeVideoComposer : ServerPacket
    {
        public YoutubeVideoComposer(string VideoId, string VideoBy) : base(2)
        {
            base.WriteString(VideoId);
            base.WriteString(VideoBy);
        }
    }
}