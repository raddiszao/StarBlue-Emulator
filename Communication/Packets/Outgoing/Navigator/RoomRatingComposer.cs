namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class RoomRatingComposer : MessageComposer
    {
        private int Score { get; }
        private bool CanVote { get; }

        public RoomRatingComposer(int Score, bool CanVote)
            : base(Composers.RoomRatingMessageComposer)
        {
            this.Score = Score;
            this.CanVote = CanVote;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Score);
            packet.WriteBoolean(CanVote);
        }
    }
}
