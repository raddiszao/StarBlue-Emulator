namespace StarBlue.HabboHotel.Users.Messenger.FriendBar
{
    public static class FriendBarStateUtility
    {
        public static FriendBarState GetEnum(int State)
        {
            switch (State)
            {
                case 0:
                    return FriendBarState.CLOSED;

                case 1:
                    return FriendBarState.OPEN;

                default:
                    return FriendBarState.OPEN;
            }
        }

        public static int GetInt(FriendBarState State)
        {
            switch (State)
            {
                default:
                    return 1;

                case FriendBarState.CLOSED:
                    return 0;

                case FriendBarState.OPEN:
                    return 1;

                case FriendBarState.MIDDLE:
                    return 3;
            }
        }
    }
}
