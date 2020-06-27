﻿namespace StarBlue.HabboHotel.Users.Messenger.FriendBar
{
    public static class FriendBarStateUtility
    {
        public static FriendBarState GetEnum(int State)
        {
            switch (State)
            {
                default:
                case 0:
                    return FriendBarState.CLOSED;

                case 1:
                    return FriendBarState.OPEN;

                case 3:
                    return FriendBarState.MIDDLE;
            }
        }

        public static int GetInt(FriendBarState State)
        {
            switch (State)
            {
                default:
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
