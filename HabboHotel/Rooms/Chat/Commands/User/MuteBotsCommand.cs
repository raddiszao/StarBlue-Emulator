using StarBlue.Database.Interfaces;


namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class MuteBotsCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Silencia todos os BOTs.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowBotSpeech = !Session.GetHabbo().AllowBotSpeech;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE `users` SET `bots_muted` = '" + ((Session.GetHabbo().AllowBotSpeech) ? 1 : 0) + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            if (Session.GetHabbo().AllowBotSpeech)
            {
                Session.SendWhisper("Pronto, agora você não pode ouvir mais os BOTS", 34);
            }
            else
            {
                Session.SendWhisper("Pronto, agora você pode ouvir os BOTS.", 34);
            }
        }
    }
}
