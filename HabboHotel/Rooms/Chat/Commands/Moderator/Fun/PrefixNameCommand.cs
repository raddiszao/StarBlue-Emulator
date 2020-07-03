using Database_Manager.Database.Session_Details.Interfaces;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class PrefixNameCommand : IChatCommand
    {

        public string PermissionRequired => "user_1";
        public string Parameters => "[off]";
        public string Description => "Desativar prefixo.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Comando inválido.", 34);
                return;
            }

            if (Params[1].ToString().ToLower() == "off")
            {
                Session.GetHabbo()._tag = "";
                Session.SendWhisper("Desativaste seu prefixo com sucesso!");
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("UPDATE `users` SET `tag` = '' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                }
            }
        }
    }
}