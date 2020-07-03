using System.Collections.Generic;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Polls
{
    public class RoomPoll
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public RoomPollType Type { get; set; }
        public string Headline { get; set; }
        public string Summary { get; set; }
        public string CompletionMessage { get; set; }
        public int CreditReward { get; set; }
        public int PixelReward { get; set; }
        public string BadgeReward { get; set; }
        public double Expiry { get; set; }

        private Dictionary<int, RoomPollQuestion> _questions;
        public int LastQuestionId { get; set; }

        public RoomPoll(int id, int roomId, string type, string headline, string summary, string completionMessage, int creditReward, int pixelReward, string badgeReward, double expiry, Dictionary<int, RoomPollQuestion> questions)
        {
            Id = id;
            RoomId = roomId;
            Type = RoomPollTypeUtility.GetRoomPollType(type);
            Headline = headline;
            Summary = summary;
            CompletionMessage = completionMessage;
            CreditReward = creditReward;
            PixelReward = pixelReward;
            BadgeReward = badgeReward;
            Expiry = expiry;

            _questions = questions.Values.OrderBy(x => x.SeriesOrder).ToDictionary(t => t.Id);
            LastQuestionId = _questions.Count > 0 ? _questions.Values.OrderByDescending(x => x.SeriesOrder).FirstOrDefault().SeriesOrder : 0;
        }

        public Dictionary<int, RoomPollQuestion> Questions
        {
            get => _questions;
            set => _questions = value;
        }
    }
}