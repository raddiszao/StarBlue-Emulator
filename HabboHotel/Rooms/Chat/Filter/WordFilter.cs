namespace StarBlue.HabboHotel.Rooms.Chat.Filter
{
    internal sealed class WordFilter
    {
        private string _word;
        private string _replacement;
        private bool _strict;
        private bool _bannable;

        public WordFilter(string Word, string Replacement, bool Strict, bool Bannable)
        {
            _word = Word;
            _replacement = Replacement;
            _strict = Strict;
            _bannable = Bannable;
        }

        public string Word => _word;

        public string Replacement => _replacement;
        public bool IsStrict => _strict;
        public bool IsBannable => _bannable;
    }
}
