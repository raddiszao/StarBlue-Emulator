namespace StarBlue.Communication.Packets.Outgoing.Notifications
{
    internal class GraphicAlertComposer : MessageComposer
    {
        private string image { get; }

        public GraphicAlertComposer(string image) : base(Composers.GraphicAlertComposer)
        {
            this.image = image;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString("${image.library.url}" + image + ".png");
        }
    }
}
