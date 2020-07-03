namespace StarBlue.HabboHotel.Navigator
{
    public class SearchResultList
    {
        private int _id;
        private string _category;
        private string _categoryName;
        private string _customName;
        private bool _canDoActions;
        private int _colour;
        private int _requiredRank;
        private NavigatorViewMode _viewMode;
        private NavigatorCategoryType _categoryType;
        private NavigatorSearchAllowance _searchAllowance;
        private int _orderId;

        public SearchResultList(int Id, string Category, string CategoryIdentifier, string PublicName, bool CanDoActions, int Colour, int RequiredRank, NavigatorViewMode ViewMode, string CategoryType, string SearchAllowance, int OrderId)
        {
            _id = Id;
            _category = Category;
            _categoryName = CategoryIdentifier;
            _customName = PublicName;
            _canDoActions = CanDoActions;
            _colour = Colour;
            _requiredRank = RequiredRank;
            _viewMode = ViewMode;
            _categoryType = NavigatorCategoryTypeUtility.GetCategoryTypeByString(CategoryType);
            _searchAllowance = NavigatorSearchAllowanceUtility.GetSearchAllowanceByString(SearchAllowance);
            _orderId = OrderId;
        }

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        //TODO: Make an enum?
        public string Category
        {
            get => _category;
            set => _category = value;
        }

        public string CategoryIdentifier
        {
            get => _categoryName;
            set => _categoryName = value;
        }

        public string PublicName
        {
            get => _customName;
            set => _customName = value;
        }

        public bool CanDoActions
        {
            get => _canDoActions;
            set => _canDoActions = value;
        }

        public int Colour
        {
            get => _colour;
            set => _colour = value;
        }

        public int RequiredRank
        {
            get => _requiredRank;
            set => _requiredRank = value;
        }

        public NavigatorViewMode ViewMode
        {
            get => _viewMode;
            set => _viewMode = value;
        }

        public NavigatorCategoryType CategoryType
        {
            get => _categoryType;
            set => _categoryType = value;
        }

        public NavigatorSearchAllowance SearchAllowance
        {
            get => _searchAllowance;
            set => _searchAllowance = value;
        }

        public int OrderId
        {
            get => _orderId;
            set => _orderId = value;
        }
    }
}
