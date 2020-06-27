using StarBlue.Communication.Packets.Outgoing.LandingView;
using System;

namespace StarBlue.Communication.Packets.Incoming.LandingView
{
    class RefreshCampaignEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            try
            {
                String parseCampaings = Packet.PopString();
                Session.SendMessage(new HOFComposer());

                String campaingName = "";
                String[] parser = parseCampaings.Split(';');

                for (int i = 0; i < parser.Length; i++)
                {
                    if (String.IsNullOrEmpty(parser[i]) || parser[i].EndsWith(","))
                    {
                        continue;
                    }

                    String[] data = parser[i].Split(',');
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