namespace StarBlue.HabboHotel.Rooms.Polls
{
    public class RoomPollQuestionSelection
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }

        public RoomPollQuestionSelection(int id, int questionId, string text, string value)
        {
            Id = id;
            QuestionId = questionId;
            Text = text;
            Value = value;
        }
    }
}