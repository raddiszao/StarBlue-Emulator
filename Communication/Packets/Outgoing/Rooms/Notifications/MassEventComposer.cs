namespace StarBlue.Communication.Packets.Outgoing.Rooms.Notifications
{
    class MassEventComposer : ServerPacket
    {
        public MassEventComposer(string Message)
            : base(ServerPacketHeader.MassEventComposer)

        {
            base.WriteString(Message);
        }
    }
}