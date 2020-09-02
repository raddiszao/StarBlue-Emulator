using log4net;
using StarBlue.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Polls
{
    public class PollManager
    {
        private static readonly ILog log = LogManager.GetLogger("StarBlue.HabboHotel.Polls.PollManager");

        private readonly Dictionary<int, RoomPoll> _polls;
        private readonly Dictionary<int, Dictionary<int, RoomPollQuestion>> _questions;

        public PollManager()
        {
            _polls = new Dictionary<int, RoomPoll>();
            _questions = new Dictionary<int, Dictionary<int, RoomPollQuestion>>();
        }

        public void Init()
        {
            if (_questions.Count > 0)
            {
                _questions.Clear();
            }

            int QuestionsLoaded = 0;

            DataTable GetQuestions = null;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `room_poll_questions`;");
                GetQuestions = dbClient.GetTable();

                if (GetQuestions != null)
                {
                    foreach (DataRow Row in GetQuestions.Rows)
                    {
                        int PollId = Convert.ToInt32(Row["poll_id"]);

                        if (!_questions.ContainsKey(PollId))
                        {
                            _questions[PollId] = new Dictionary<int, RoomPollQuestion>();
                        }

                        RoomPollQuestion CatalogItem = new RoomPollQuestion(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["poll_id"]), Convert.ToString(Row["question"]), Convert.ToString(Row["question_type"]), Convert.ToInt32(Row["series_order"]), Convert.ToInt32(Row["minimum_selections"]));

                        _questions[CatalogItem.PollId].Add(CatalogItem.Id, CatalogItem);

                        QuestionsLoaded++;
                    }
                }
            }

            if (_polls.Count > 0)
            {
                _polls.Clear();
            }

            DataTable GetPolls = null;

            int PollsLoaded = 0;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `room_polls` WHERE `enabled` = 'Y'");
                GetPolls = dbClient.GetTable();

                if (GetPolls != null)
                {
                    foreach (DataRow Row in GetPolls.Rows)
                    {
                        _polls.Add(Convert.ToInt32(Row["id"]), new RoomPoll(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["room_id"]),
                              Convert.ToString(Row["type"]), Convert.ToString(Row["headline"]), Convert.ToString(Row["summary"]), Convert.ToString(Row["completion_message"]),
                              Convert.ToInt32(Row["credit_reward"]), Convert.ToInt32(Row["pixel_reward"]), Convert.ToString(Row["badge_reward"]), Convert.ToDouble(Row["expiry"]),
                              _questions.ContainsKey(Convert.ToInt32(Row["id"])) ? _questions[Convert.ToInt32(Row["id"])] : new Dictionary<int, RoomPollQuestion>()));

                        PollsLoaded++;
                    }
                }

                //log.Info("Loaded " + PollsLoaded + " room polls & " + QuestionsLoaded + " poll questions");
                log.Info(">> Poll's Manager -> READY!");
            }
        }

        public bool TryGetPoll(int pollId, out RoomPoll roomPoll)
        {
            return _polls.TryGetValue(pollId, out roomPoll);
        }

        public bool TryGetPollForRoom(int roomId, out RoomPoll roomPoll)
        {
            roomPoll = null;
            if (_polls.Count(x => x.Value.RoomId == roomId) == 0)
            {
                return false;
            }

            return _polls.TryGetValue(_polls.FirstOrDefault(x => x.Value.RoomId == roomId).Value.Id, out roomPoll);
        }

        public bool TryGetPollForHotel(int id, out RoomPoll hotelPoll)
        {
            hotelPoll = null;
            if (_polls.Count(x => x.Value.Id == id) == 0)
            {
                return false;
            }

            return _polls.TryGetValue(_polls.FirstOrDefault(x => x.Value.Id == id).Value.Id, out hotelPoll);
        }
    }
}