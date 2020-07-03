namespace StarBlue.HabboHotel.Users.Messenger
{
    public class MessengerRequest
    {
        private int _toUser;
        private int _fromUser;
        private string _username;

        public MessengerRequest(int ToUser, int FromUser, string Username)
        {
            _toUser = ToUser;
            _fromUser = FromUser;
            _username = Username;
        }

        public string Username => _username;

        public int To => _toUser;

        public int From => _fromUser;
    }
}