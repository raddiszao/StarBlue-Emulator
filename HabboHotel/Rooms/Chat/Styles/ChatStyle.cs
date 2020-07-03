namespace StarBlue.HabboHotel.Rooms.Chat.Styles
{
    public sealed class ChatStyle
    {
        private int _id;
        private string _name;
        private string _requiredRight;

        public ChatStyle(int Id, string Name, string RequiredRight)
        {
            _id = Id;
            _name = Name;
            _requiredRight = RequiredRight;
        }

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string RequiredRight
        {
            get => _requiredRight;
            set => _requiredRight = value;
        }
    }
}