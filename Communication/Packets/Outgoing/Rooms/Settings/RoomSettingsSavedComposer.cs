namespace StarBlue.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class RoomSettingsSavedComposer : MessageComposer
    {
        public int RoomId { get; }
        public RoomSettingsSavedComposer(int roomID)
            : base(Composers.RoomSettingsSavedMessageComposer)
        {
            this.RoomId = roomID;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(RoomId);
        }
    }
}