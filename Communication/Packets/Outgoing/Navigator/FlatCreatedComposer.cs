namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    class FlatCreatedComposer : ServerPacket
    {
        public FlatCreatedComposer(int roomID, string roomName)
            : base(ServerPacketHeader.FlatCreatedMessageComposer)
        {
            base.WriteInteger(roomID);
            base.WriteString(roomName);
        }
    }
}
