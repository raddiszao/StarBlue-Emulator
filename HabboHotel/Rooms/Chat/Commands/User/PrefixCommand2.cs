namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class PrefixCommand2 : IChatCommand
    {
        public string PermissionRequired => "user_1";
        public string Parameters => "%remove%";
        public string Description => "Deletar prefixo.";
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Diga ':tagremover remove' para deletar seu prefixo (a Tag)", 34);
                return;
            }

            if (Session.GetHabbo() == null)
            {
                return;
            }

            if (Params[1].ToLower() == "remove")
            {
                if (Session.GetHabbo().Rank >= 1)
                {
                    Session.GetHabbo()._tag = string.Empty;
                    UpdateDatabase(Session);
                }
            }

            Session.SendWhisper("Prefixo (a Tag) removido corretamente!", 34);
            return;
        }

        public void UpdateDatabase(GameClients.GameClient Session)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            using (var dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE `users` SET `tag` = '" + Session.GetHabbo()._tag + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");

            }
        }
    }
}