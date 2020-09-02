namespace StarBlue.Communication.Packets.Outgoing.Inventory.Achievements
{
    internal class AchievementScoreComposer : ServerPacket
    {
        public AchievementScoreComposer(int achScore)
            : base(ServerPacketHeader.AchievementScoreMessageComposer)
        {
            base.WriteInteger(achScore);
        }
    }
}
