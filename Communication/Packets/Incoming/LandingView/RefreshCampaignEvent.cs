using StarBlue.Communication.Packets.Outgoing.LandingView;

namespace StarBlue.Communication.Packets.Incoming.LandingView
{
    internal class RefreshCampaignEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            try
            {
                string parseCampaings = Packet.PopString();
                Session.SendMessage(new HOFComposer());

                string campaingName = "";
                string[] parser = parseCampaings.Split(';');

                for (int i = 0; i < parser.Length; i++)
                {
                    if (string.IsNullOrEmpty(parser[i]) || parser[i].EndsWith(","))
                    {
                        continue;
                    }

                    string[] data = parser[i].Split(',');
                    campaingName = data[1];
                }
                Session.SendMessage(new CampaignComposer(parseCampaings, campaingName));

                Session.SendMessage(new LimitedCountdownExtendedComposer());

                if (campaingName.Contains("CommunityGoal"))
                {
                    Session.SendMessage(new CommunityGoalComposer());
                    Session.SendMessage(new DynamicPollLandingComposer(false)); // Si este campo está en false el usuario puede votar.
                }
            }
            catch
            {

            }
        }
    }
}