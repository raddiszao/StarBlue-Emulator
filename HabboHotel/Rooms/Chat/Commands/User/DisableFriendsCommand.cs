using Database_Manager.Database.Session_Details.Interfaces;



namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableFriendsCommand : IChatCommand
    {
        public string PermissionRequired => "command_disable_friends";

        public string Parameters => "";

        public string Description => "Desativa as solicitações de amigos.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowFriendRequests = !Session.GetHabbo().AllowFriendRequests;
            Session.SendWhisper("Você " + (Session.GetHabbo().AllowFriendRequests == true ? "agora" : "agora não") + "é mais adicionavel.");

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `block_newfriends` = '1' WHERE `id` = '" + Session.GetHabbo().Id + "'");
                dbClient.RunQuery();
            }
        }
    }
}