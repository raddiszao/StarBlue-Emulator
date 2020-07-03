namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class ForceStandCommand : IChatCommand
    {
        public string PermissionRequired => "user_7";

        public string Parameters => "[USUARIO]";

        public string Description => "Levantar outro usuário.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Uau, você esqueceu de escolher um usuário!", 34);
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Params[1]);
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