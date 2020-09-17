namespace StarBlue.Communication.Packets.Outgoing.Users
{
    internal class RespectNotificationComposer : MessageComposer
    {
        private int userID { get; }
        private int Respect { get; }

        public RespectNotificationComposer(int userID, int Respect)
            : base(Composers.RespectNotificationMessageComposer)
        {
            this.userID = userID;
            this.Respect = Respect;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(userID);
            packet.WriteInteger(Respect);
        }
    }
}
