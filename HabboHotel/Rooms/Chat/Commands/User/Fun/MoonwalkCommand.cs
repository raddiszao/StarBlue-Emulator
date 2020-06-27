namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class MoonwalkCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_vip"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Faça o passo do Michael Jackson."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            User.moonwalkEnabled = !User.moonwalkEnabled;

            if (User.moonwalkEnabled)
            {
                Session.SendWhisper("Moonwalk Ativado!");
            }
            else
            {
                Session.SendWhisper("Moonwalk Desativado!");
            }
        }
    }
}
