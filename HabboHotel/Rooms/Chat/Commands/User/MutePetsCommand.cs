using StarBlue.Database.Interfaces;


namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class MutePetsCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Silencie os mascotes.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowPetSpeech = !Session.GetHabbo().AllowPetSpeech;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE `users` SET `pets_muted` = '" + ((Session.GetHabbo().AllowPetSpeech) ? 1 : 0) + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            if (Session.GetHabbo().AllowPetSpeech)
            {
                Session.SendWhisper("Pronto, agora não podes escutar os Mascotes", 34);
            }
            else
            {
                Session.SendWhisper("Pronto, agora você pode ouvir os Mascotes", 34);
            }
        }
    }
}
