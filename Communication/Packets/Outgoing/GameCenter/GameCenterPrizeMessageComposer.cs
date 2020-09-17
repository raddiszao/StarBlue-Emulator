namespace StarBlue.Communication.Packets.Outgoing.GameCenter
{
    public class GameCenterPrizeMessageComposer : MessageComposer
    {
        private int GameId { get; }

        public GameCenterPrizeMessageComposer(int GameId)
            : base(Composers.GameCenterPrizeMessageComposer)
        {
            this.GameId = GameId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(GameId);
            packet.WriteInteger(1);
            packet.WriteString("s");
            packet.WriteInteger(230); // SpriteID
            packet.WriteString("throne");
            packet.WriteInteger(3);
            packet.WriteBoolean(false);
            packet.WriteInteger(10000);
            packet.WriteBoolean(true);
        }
    }
}
