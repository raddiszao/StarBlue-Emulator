namespace StarBlue.Communication.Packets.Outgoing.LandingView
{
    internal class CampaignComposer : ServerPacket
    {
        public CampaignComposer(string campaignString, string campaignName)
            : base(ServerPacketHeader.CampaignMessageComposer)
        {
            base.WriteString(campaignString);
            base.WriteString(campaignName);
        }
    }
}
