namespace StarBlue.Communication.Packets.Outgoing.LandingView
{
    internal class CampaignComposer : MessageComposer
    {
        private string campaignString { get; }
        private string campaignName { get; }

        public CampaignComposer(string campaignString, string campaignName)
            : base(Composers.CampaignMessageComposer)
        {
            this.campaignName = campaignName;
            this.campaignString = campaignString;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(campaignString);
            packet.WriteString(campaignName);
        }
    }
}
