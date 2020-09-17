namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class FlatCreatedComposer : MessageComposer
    {
        private int roomID { get; }
        private string roomName { get; }

        public FlatCreatedComposer(int roomID, string roomName)
            : base(Composers.FlatCreatedMessageComposer)
        {
            this.roomID = roomID;
            this.roomName = roomName;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(roomID);
            packet.WriteString(roomName);
        }
    }
}
