namespace StarBlue.Communication.Packets.Outgoing.Inventory.Achievements
{
    internal class AchievementScoreComposer : MessageComposer
    {
        public int AchievementScore { get; }

        public AchievementScoreComposer(int achScore)
            : base(Composers.AchievementScoreMessageComposer)
        {
            this.AchievementScore = achScore;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(AchievementScore);
        }
    }
}
