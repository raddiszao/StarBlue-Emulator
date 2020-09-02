using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;


namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class UnmuteCommand : IChatCommand
    {
        public string PermissionRequired => "user_7";
        public string Parameters => "[USUARIO]";
        public string Description => "Desmutar usuario.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite o nome do usuário..", 34);
                return;
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null || TargetClient.GetHabbo() == null)
            {
                Session.SendWhisper("Ocorreu um erro.", 34);
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE `users` SET `time_muted` = '0' WHERE `id` = '" + TargetClient.GetHabbo().Id + "' LIMIT 1");
            }

            TargetClient.GetHabbo().TimeMuted = 0;
            TargetClient.SendNotification("Você foi desmutado por " + Session.GetHabbo().Username + "!");
            Session.SendWhisper("Você desmutou " + TargetClient.GetHabbo().Username + "!", 34);
        }
    }
}