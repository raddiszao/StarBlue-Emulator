namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class FurniMaticRewardsComposer : MessageComposer
    {
        public FurniMaticRewardsComposer()
            : base(Composers.FurniMaticRewardsComposer)
        {

        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(5);
            for (int i = 5; i >= 1; i--)
            {
                packet.WriteInteger(i);
                if (i <= 1)
                {
                    packet.WriteInteger(1);
                }
                else if (i == 2)
                {
                    packet.WriteInteger(5);
                }
                else if (i == 3)
                {
                    packet.WriteInteger(20);
                }
                else if (i == 4)
                {
                    packet.WriteInteger(50);
                }
                else if (i == 5)
                {
                    packet.WriteInteger(100);
                }

                System.Collections.Generic.List<HabboHotel.Catalog.FurniMatic.FurniMaticRewards> rewards = StarBlueServer.GetGame().GetFurniMaticRewardsMnager().GetRewardsByLevel(i);
                packet.WriteInteger(rewards.Count);
                foreach (HabboHotel.Catalog.FurniMatic.FurniMaticRewards reward in rewards)
                {
                    if (reward.GetBaseItem() == null)
                        continue;

                    packet.WriteString(reward.GetBaseItem().ItemName);
                    packet.WriteInteger(reward.DisplayId);
                    packet.WriteString(reward.GetBaseItem().Type.ToString().ToLower());
                    packet.WriteInteger(reward.GetBaseItem().SpriteId);
                }
            }
        }
    }
}
