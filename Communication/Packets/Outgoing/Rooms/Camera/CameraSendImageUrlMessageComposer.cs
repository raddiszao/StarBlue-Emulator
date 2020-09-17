namespace StarBlue.Communication.Packets.Outgoing.Rooms.Camera
{
    public class CameraSendImageUrlMessageComposer : MessageComposer
    {
        private string ImageUrl { get; }

        public CameraSendImageUrlMessageComposer(string ImageUrl)
            : base(Composers.CameraSendImageUrlMessageComposer)
        {
            this.ImageUrl = ImageUrl;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(ImageUrl);
        }
    }
}