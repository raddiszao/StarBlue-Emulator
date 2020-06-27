namespace StarBlue.Communication.Packets.Outgoing.Rooms.Settings
{
    class RoomSettingsSavedComposer : ServerPacket
    {
        public RoomSettingsSavedComposer(int roomID)
            : base(ServerPacketHeader.RoomSettingsSavedMessageComposer)
        {
            base.WriteInteger(roomID);
        }
    }
}