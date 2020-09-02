using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Users;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class UnBanCommand : IChatCommand
    {

        public string PermissionRequired => "user_14";
        public string Parameters => "[USUARIO]";
        public string Description => "Desbanir usuário.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, digite o nome de usuário do usuário.", 34);
                return;
            }

            Habbo Habbo = StarBlueServer.GetHabboByUsername(Params[1]);
            if (Habbo == null)
            {
                Session.SendWhisper("Ocorreu um erro enquanto procurava o usuário no banco de dados.", 34);
                return;
            }

            if (Habbo.GetPermissions().HasRight("mod_soft_ban") && !Session.GetHabbo().GetPermissions().HasRight("mod_ban_any"))
            {
                Session.SendWhisper("Oops, você não pode desbanir esse usuário.", 34);
                return;
            }

            string Username = Habbo.Username;
            string IPAddress = "";
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `ip_last` FROM `users` WHERE `id` = '" + Habbo.Id + "' LIMIT 1");
                IPAddress = dbClient.GetString();

                dbClient.RunFastQuery("DELETE FROM `bans` WHERE `value` = '" + Habbo.Username + "' or `value` =  '" + IPAddress + "' LIMIT 1");
            }

            Session.SendWhisper("Sucesso, você desbaniu '" + Username + "'!", 34);
        }
    }
}