using StarBlue.Database.Interfaces;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class DiagonalCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Desativa a opção de andar na diagonal na sua sala.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oops, só o dono do quarto pode usar este comando!", 34);
                return;
            }

            Room.RoomData.DiagonalEnabled = !Room.RoomData.DiagonalEnabled;

            using (IQueryAdapter con = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                con.SetQuery("UPDATE `rooms` SET `diagonal_enabled` = @enum WHERE `id` = @id LIMIT 1");
                con.AddParameter("enum", StarBlueServer.BoolToEnum(Room.RoomData.DiagonalEnabled));
                con.AddParameter("id", Room.Id);
                con.RunQuery();
            }

            Session.SendWhisper(Room.RoomData.DiagonalEnabled ? "Todos podem caminhar em diagonal na sala." : "Ninguém pode caminhar na diagonal na sala.", 34);
        }
    }
}
