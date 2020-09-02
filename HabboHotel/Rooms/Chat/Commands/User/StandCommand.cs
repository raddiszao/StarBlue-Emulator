namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class StandCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters
        {
            get { return ""; ; }
        }

        public string Description => "Levanta-se.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
            if (User == null)
            {
                return;
            }

            if (User.isSitting)
            {
                User.Statusses.Remove("sit");
                User.Z += 0.35;
                User.isSitting = false;
                User.UpdateNeeded = true;
            }
            else if (User.isLying)
            {
                User.Statusses.Remove("lay");
                User.Z += 0.35;
                User.isLying = false;
                User.UpdateNeeded = true;
            }
        }
    }
}
