namespace StarBlue.Communication.Packets.Outgoing.Notifications
{
    internal class GraphicAlertComposer : ServerPacket
    {
        public GraphicAlertComposer(string image) : base(ServerPacketHeader.GraphicAlertComposer)
        { base.WriteString("${image.library.url}" + image + ".png"); }
    }
}
