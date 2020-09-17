namespace StarBlue.Communication.Packets.Outgoing.GameCenter
{
    internal class PlayableGamesComposer : MessageComposer
    {
        private int GameID { get; }

        public PlayableGamesComposer(int GameID)
            : base(Composers.PlayableGamesMessageComposer)
        {
            this.GameID = GameID;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(GameID);
            packet.WriteInteger(0);
        }
    }
}
