using log4net;
using StarBlue.Database.Interfaces;
using System.Data;

namespace StarBlue.HabboHotel.LandingView.CommunityGoal
{
    public class CommunityGoalVS
    {
        private static readonly ILog log = LogManager.GetLogger("StarBlue.HabboHotel.LandingView.CommunityGoalVS");

        private int Id;
        private string Name;
        private int LeftVotes;
        private int RightVotes;

        public void LoadCommunityGoalVS()
        {
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `landing_communitygoalvs` ORDER BY `id` DESC LIMIT 1");
                DataRow dRow = dbClient.GetRow();

                if (dRow != null)
                {
                    Id = (int)dRow["id"];
                    Name = (string)dRow["name"];
                    LeftVotes = (int)dRow["left_votes"];
                    RightVotes = (int)dRow["right_votes"];
                }
            }
        }

        public int GetId()
        {
            return Id;
        }

        public string GetName()
        {
            return Name;
        }

        public int GetLeftVotes()
        {
            return LeftVotes;
        }

        public int GetRightVotes()
        {
            return RightVotes;
        }

        public void IncreaseLeftVotes()
        {
            LeftVotes++;
        }

        public void IncreaseRightVotes()
        {
            RightVotes++;
        }
    }
}
