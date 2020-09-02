namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    internal class TeleportCommand : IChatCommand
    {
        public string PermissionRequired => "user_7";
        public string Parameters => "";
        public string Description => "Obter a habilidade de se teletransportar.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            User.TeleportEnabled = !User.TeleportEnabled;
            Room.GetGameMap().GenerateMaps();
        }
    }
}
