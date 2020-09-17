namespace StarBlue.Communication.Packets.Outgoing.GameCenter
{
    internal class JoinQueueComposer : MessageComposer
    {
        private int GameId { get; }

        public JoinQueueComposer(int GameId)
            : base(Composers.JoinQueueMessageComposer)
        {
            this.GameId = GameId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(GameId);
        }
    }
}
