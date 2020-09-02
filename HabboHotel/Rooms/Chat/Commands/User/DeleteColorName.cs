using StarBlue.Database.Interfaces;
namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class DeleteColorName : IChatCommand
    {
        public string PermissionRequired => "user_vip";
        public string Parameters => "%remove%";
        public string Description => "Deletar a cor no nome.";
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Diga ':namecolor remove' remover a cor do seu nome", 34);
                return;
            }

            if (Session.GetHabbo() == null)
            {
                return;
            }

            if (Params[1].ToLower() == "remove")
            {
                Session.GetHabbo().chatHTMLColour = string.Empty;
                UpdateDatabase(Session);
            }

            Session.SendWhisper("Cor removida corretamente!", 34);
            return;
        }

        public void UpdateDatabase(GameClients.GameClient Session)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE `users` SET `namecolor` = '" + Session.GetHabbo().chatHTMLColour + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }
        }
    }
}