using Database_Manager.Database.Session_Details.Interfaces;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class PrefixNameCommand : IChatCommand
    {

        public string PermissionRequired => "user_7";
        public string Parameters => "[PREFIXO]";
        public string Description => "off/red/green/blue/cyan/purple";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, digite por exemplo: [ADM]", 34);
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
            else
            {
                string PrefixName = CommandManager.MergeParams(Params, 1);
                Session.GetHabbo()._tag = PrefixName;
                Session.SendWhisper("Seu prefixo para o nome foi adicionado corretamente");
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `users` SET `tag` = @prefix WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    dbClient.AddParameter("prefix", PrefixName);
                    dbClient.RunQuery();
                }
            }
            return;
        }
    }
}