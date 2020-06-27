namespace StarBlue.Communication.Packets.Outgoing.Notifications
{
    class GraphicAlertComposer : ServerPacket
    {
        public GraphicAlertComposer(string image) : base(ServerPacketHeader.GraphicAlertComposer)
        { base.WriteString("${image.library.url}" + image + ".png"); }
    }
}
