using StarBlue.Communication.WebSocket;

namespace StarBlue.Communication.Packets.Outgoing.WebSocket
{
    internal class YoutubeVideoComposer : WebComposer
    {
        public YoutubeVideoComposer(string VideoId, string VideoBy) : base(2)
        {
            base.WriteString(VideoId);
            base.WriteString(VideoBy);
        }
    }
}