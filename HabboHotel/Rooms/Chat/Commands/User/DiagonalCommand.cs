namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class DiagonalCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Desativa a opção de andar na diagonal na sua sala."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oops, só o dono do quarto pode usar este comando!", 34);
                return;
            }

            Room.GetGameMap().DiagonalEnabled = !Room.GetGameMap().DiagonalEnabled;
            Session.SendWhisper(Room.GetGameMap().DiagonalEnabled ? "Todos podem caminhar em diagonal na sala." : "Ninguém pode caminhar na diagonal na sala.", 34);
        }
    }
}
