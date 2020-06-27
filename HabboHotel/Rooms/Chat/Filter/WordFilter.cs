namespace StarBlue.HabboHotel.Rooms.Chat.Filter
{
    sealed class WordFilter
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

        public string Word
        {
            get { return _word; }
        }

        public string Replacement
        {
            get { return _replacement; }
        }
        public bool IsStrict
        {
            get { return _strict; }
        }
        public bool IsBannable
        {
            get { return _bannable; }
        }
    }
}
