using Database_Manager.Database.Session_Details.Interfaces;



namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class EnableFriendsCommand : IChatCommand
    {
        public string PermissionRequired => "command_enable_friends";

        public string Parameters => "";

        public string Description => "Ativa as solicitações de amizade.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowFriendRequests = !Session.GetHabbo().AllowFriendRequests;
            Session.SendWhisper("Você" + (Session.GetHabbo().AllowFriendRequests == true ? "agora" : "agora não ") + " pode ter amigos.");

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `block_newfriends` = '0' WHERE `id` = '" + Session.GetHabbo().Id + "'");

                dbClient.RunQuery();
            }
        }
    }
}