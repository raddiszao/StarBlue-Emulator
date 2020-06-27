﻿using Database_Manager.Database.Session_Details.Interfaces;

namespace StarBlue.Communication.Packets.Incoming.LandingView
{
    class VoteCommunityGoalVS : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int VoteType = Packet.PopInt(); // 1 izq, 2 der

            if (VoteType == 1)
            {
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("UPDATE landing_communitygoalvs SET left_votes = left_votes + 1 WHERE id = " + StarBlueServer.GetGame().GetCommunityGoalVS().GetId());
                }

                StarBlueServer.GetGame().GetCommunityGoalVS().IncreaseLeftVotes();
            }
            else if (VoteType == 2)
            {
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("UPDATE landing_communitygoalvs SET right_votes = right_votes + 1 WHERE id = " + StarBlueServer.GetGame().GetCommunityGoalVS().GetId());
                }

                StarBlueServer.GetGame().GetCommunityGoalVS().IncreaseRightVotes();
            }
            StarBlueServer.GetGame().GetCommunityGoalVS().LoadCommunityGoalVS();
        }
    }
}