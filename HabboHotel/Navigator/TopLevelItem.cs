namespace StarBlue.HabboHotel.Navigator
{
    public class TopLevelItem
    {
        private int _id;
        private string _searchCode;
        private string _filter;
        private string _localization;

        public TopLevelItem(int Id, string SearchCode, string Filter, string Localization)
        {
            _id = Id;
            _searchCode = SearchCode;
            _filter = Filter;
            _localization = Localization;
        }

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string SearchCode
        {
            get => _searchCode;
            set => _searchCode = value;
        }

        public string Filter
        {
            get => _filter;
            set => _filter = value;
        }

        public string Localization
        {
            get => _localization;
            set => _localization = value;
        }
    }
}