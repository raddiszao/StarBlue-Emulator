namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class UserNameChangeComposer : ServerPacket
    {
        public UserNameChangeComposer(int RoomId, int VirtualId, string Username)
            : base(ServerPacketHeader.UserNameChangeMessageComposer)
        {
            base.WriteInteger(RoomId);
            base.WriteInteger(VirtualId);
            base.WriteString(Username);
        }
    }
}
