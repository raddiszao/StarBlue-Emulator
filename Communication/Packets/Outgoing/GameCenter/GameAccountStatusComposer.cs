namespace StarBlue.Communication.Packets.Outgoing.GameCenter
{
    internal class GameAccountStatusComposer : MessageComposer
    {
        private int GameId { get; }

        public GameAccountStatusComposer(int GameID)
            : base(Composers.GameAccountStatusMessageComposer)
        {
            this.GameId = GameID;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(GameId);
            packet.WriteInteger(100); // Games Left
            packet.WriteInteger(0);//Was 16?
        }
    }
}