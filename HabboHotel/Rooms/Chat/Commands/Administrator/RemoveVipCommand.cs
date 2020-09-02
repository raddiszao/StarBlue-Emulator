using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Communication.Packets.Outgoing.Users;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class RemoveVipCommand : IChatCommand
    {
        public string PermissionRequired => "user_17";
        public string Parameters => "[USUARIO]";
        public string Description => "Remover rank VIP a um usuário.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduza o nome do usuário.");
                return;
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Não foi possível localizar o usuário.");
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM user_subscriptions WHERE user_id = @id");
                dbClient.AddParameter("id", TargetClient.GetHabbo().Id);
                dbClient.RunQuery();
                dbClient.RunFastQuery("UPDATE `users` SET `rank` = '1' WHERE `id` = '" + TargetClient.GetHabbo().Id + "'");
                dbClient.RunFastQuery("UPDATE `users` SET `rank_vip` = '0' WHERE `id` = '" + TargetClient.GetHabbo().Id + "'");
                dbClient.RunFastQuery("UPDATE `users` SET `respetos` = '3' WHERE `id` = '" + TargetClient.GetHabbo().Id + "'");
                TargetClient.GetHabbo().Rank = 1;
                TargetClient.GetHabbo().VIPRank = 0;
            }

            if (!TargetClient.GetHabbo().GetBadgeComponent().HasBadge("VIP"))
            {
                TargetClient.GetHabbo().GetClient().GetHabbo().GetBadgeComponent().RemoveBadge("VIP");
            }

            if (!TargetClient.GetHabbo().GetBadgeComponent().HasBadge("DVIP"))
            {
                TargetClient.GetHabbo().GetBadgeComponent().RemoveBadge("DVIP");
            }

            TargetClient.GetHabbo().GetClubManager().ReloadSubscription(TargetClient.GetHabbo().GetClient());
            TargetClient.SendMessage(new ScrSendUserInfoComposer(TargetClient.GetHabbo()));
            TargetClient.SendMessage(new SendHotelAlertLinkEventComposer("Infelizmente o seu VIP acabou, esperamos que tenha aproveitado bastante!"));
            Session.SendWhisper("VIP removido com sucesso!", 34);
        }
    }
}