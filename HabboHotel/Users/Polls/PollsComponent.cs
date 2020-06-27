using Database_Manager.Database.Session_Details.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace StarBlue.HabboHotel.Users.Polls
{
    public sealed class PollsComponent
    {
        private List<int> _completedPolls;

        public PollsComponent()
        {
            _completedPolls = new List<int>();
        }

        public bool Init(Habbo habbo)
        {
            if (_completedPolls.Count > 0)
            {
                return false;
            }

            DataTable GetPolls = null;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `poll_id` FROM `user_room_poll_results` WHERE `user_id` = @uid GROUP BY `poll_id`;");
                dbClient.AddParameter("uid", habbo.Id);
                GetPolls = dbClient.GetTable();

                if (GetPolls != null)
                {
                    foreach (DataRow Row in GetPolls.Rows)
                    {
                        if (!_completedPolls.Contains(Convert.ToInt32(Row["poll_id"])))
                        {
                            _completedPolls.Add(Convert.ToInt32(Row["poll_id"]));
                        }
                    }
                }
            }
            return true;
        }

        public bool TryAdd(int PollId)
        {
            if (_completedPolls.Contains(PollId))
            {
                return false;
            }

            _completedPolls.Add(PollId);
            return true;
        }

        public ICollection<int> CompletedPolls
        {
            get { return _completedPolls; }
        }

        public void Dispose()
        {
            _completedPolls.Clear();
        }
    }
}