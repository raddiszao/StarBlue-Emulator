using StarBlue.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace StarBlue.HabboHotel.Rooms.Polls
{
    public class RoomPollQuestion
    {
        public int Id { get; set; }
        public int PollId { get; set; }
        public string Question { get; set; }
        public RoomPollQuestionType Type { get; set; }
        public int SeriesOrder { get; set; }
        public int MinimumSlections { get; set; }

        private Dictionary<int, RoomPollQuestionSelection> _selections;


        public RoomPollQuestion(int id, int pollId, string question, string type, int seriesOrder, int minimumSlections)
        {
            Id = id;
            PollId = pollId;
            Question = question;
            Type = RoomPollQuestionTypeUtility.GetQuestionType(type);
            SeriesOrder = seriesOrder;
            MinimumSlections = minimumSlections;

            _selections = new Dictionary<int, RoomPollQuestionSelection>();

            DataTable GetSelections = null;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `room_poll_questions_selections` WHERE `question_id` = @QuestionId");
                dbClient.AddParameter("QuestionId", Id);
                GetSelections = dbClient.GetTable();

                if (GetSelections != null)
                {
                    foreach (DataRow Row in GetSelections.Rows)
                    {
                        if (!_selections.ContainsKey(Convert.ToInt32(Row["id"])))
                        {
                            _selections.Add(Convert.ToInt32(Row["id"]), new RoomPollQuestionSelection(Convert.ToInt32(Row["id"]), Id, Convert.ToString(Row["text"]), Convert.ToString(Row["value"])));
                        }
                    }
                }
            }
        }

        public Dictionary<int, RoomPollQuestionSelection> Selections
        {
            get => _selections;
            set => _selections = value;
        }
    }
}