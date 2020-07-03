using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class PrefixCommand : IChatCommand
    {
        public string PermissionRequired => "command_prefix";

        public string Parameters => "%prefix%";

        public string Description => "Borra tu prefijo.";

        public void Execute(GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, escreva \":tagremover\" para desativar seu prefixo.");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);

            if (Message == "off")
            {
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("UPDATE `users` SET `tag` = NULL WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                }
                Session.GetHabbo()._tag = string.Empty;
                Session.SendWhisper("Prefijo borrado correctamente.", 34);
            }
        }
    }
}
