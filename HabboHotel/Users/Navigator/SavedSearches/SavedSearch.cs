namespace StarBlue.HabboHotel.Users.Navigator.SavedSearches
{
    public class SavedSearch
    {
        private int _id;
        private string _filter;
        private string _search;

        public SavedSearch(int Id, string Filter, string Search)
        {
            _id = Id;
            _filter = Filter;
            _search = Search;
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        public string Search
        {
            get { return _search; }
            set { _search = value; }
        }
    }
}
