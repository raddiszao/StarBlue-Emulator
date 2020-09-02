using StarBlue.Communication.Packets.Outgoing.Inventory.Bots;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Users.Inventory.Bots;
using System;
using System.Linq;


namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class KickBotsCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Expulsa todos os BOTS dentro do quarto.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, false, true))
            {
                Session.SendWhisper("Lamento, somente pessoas com direitos podem usar esse comando!", 34);
                return;
            }

            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (User == null || User.IsPet || !User.IsBot)
                {
                    continue;
                }

                if (!Room.GetRoomUserManager().TryGetBot(User.BotData.Id, out RoomUser BotUser))
                {
                    return;
                }

                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `bots` SET `room_id` = '0' WHERE `id` = @id LIMIT 1");
                    dbClient.AddParameter("id", User.BotData.Id);
                    dbClient.RunQuery();
                }

                Session.GetHabbo().GetInventoryComponent().TryAddBot(new Bot(Convert.ToInt32(BotUser.BotData.Id), Convert.ToInt32(BotUser.BotData.ownerID), BotUser.BotData.Name, BotUser.BotData.Motto, BotUser.BotData.Look, BotUser.BotData.Gender));
                Session.SendMessage(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
                Room.GetRoomUserManager().RemoveBot(BotUser.VirtualId, false);
            }

            Session.SendWhisper("Todos os bots foram removidos.");
        }
    }
}
