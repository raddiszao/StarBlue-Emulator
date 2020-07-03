namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class OnlineCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Ver quantidade de usuários online.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            int OnlineUsers = StarBlueServer.GetGame().GetClientManager().Count;

            Session.SendWhisper("Agora mesmo tem " + OnlineUsers + " usuários conectados no " + StarBlueServer.HotelName + " :).", 34);
        }
    }
}

