using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Users;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class SetVipCommand : IChatCommand
    {
        public string PermissionRequired => "user_16";
        public string Parameters => "[USUARIO]";
        public string Description => "Dar rank VIP a um usuário.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduza o nome do usuário.");
                return;
            }

            if (Params.Length == 2)
            {
                Session.SendWhisper("Por favor, introduza a quantidade de dias.");
                return;
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Não foi possível localizar o usuário.");
                return;
            }

            int Days = int.Parse(CommandManager.MergeParams(Params, 2));

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE `users` SET `rank` = '2' WHERE `id` = '" + TargetClient.GetHabbo().Id + "'");
                dbClient.RunFastQuery("UPDATE `users` SET `rank_vip` = '1' WHERE `id` = '" + TargetClient.GetHabbo().Id + "'");
                TargetClient.GetHabbo().Rank = 2;
                TargetClient.GetHabbo().VIPRank = 1;
            }

            TargetClient.GetHabbo().GetClubManager().AddOrExtendSubscription("club_vip", Days * 24 * 3600, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("DVIP", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("ACH_VipClub12", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("ES28A", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("ES551", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("BR967", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("DE720", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("BR415", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("shop", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("PT054", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("PX4", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("PX3", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("UK277", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("THI95", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("NL185", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("NL537", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("ES720", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("ES78A", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("HST27", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("ROOMP", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("ES800", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("ROOMP", true, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("ES679", true, Session);

            StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_VipClub", 1);
            TargetClient.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
            TargetClient.SendNotification("Agora você é um usuário VIP!");

            Session.SendWhisper("VIP dado com sucesso!");
        }
    }
}