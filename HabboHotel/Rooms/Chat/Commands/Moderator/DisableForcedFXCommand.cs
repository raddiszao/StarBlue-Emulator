using Database_Manager.Database.Session_Details.Interfaces;


namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class DisableForcedFXCommand : IChatCommand
    {
        public string PermissionRequired => "user_2";

        public string Parameters => "";

        public string Description => "Possibilidade de ignorar ou permitir efeitos forçados.";

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            Session.GetHabbo().DisableForcedEffects = !Session.GetHabbo().DisableForcedEffects;
            Session.SendWhisper("Modo FX Forçado está " + (Session.GetHabbo().DisableForcedEffects == true ? "desativado!" : "ativado!"), 34);

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `disable_forced_effects` = @DisableForcedEffects WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("DisableForcedEffects", StarBlueServer.BoolToEnum(Session.GetHabbo().DisableForcedEffects));
                dbClient.RunQuery();
            }

        }
    }
}
