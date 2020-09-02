namespace StarBlue.Communication.Packets.Outgoing.Rooms.Notifications
{
    internal class MassEventComposer : ServerPacket
    {
        public MassEventComposer(string Message)
            : base(ServerPacketHeader.MassEventComposer)

        {
            base.WriteString(Message);
        }
    }
}