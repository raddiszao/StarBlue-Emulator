using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class BubbleBotCommand : IChatCommand
    {
        public string PermissionRequired => "user_vip";
        public string Parameters => "[BOTNOME] [BOLHAID]";
        public string Description => "Troque a bolha do seu bot.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            string BotName = CommandManager.MergeParams(Params, 1);
            string Bubble = CommandManager.MergeParams(Params, 2);

            long nowTime = StarBlueServer.CurrentTimeMillis();
            long timeBetween = nowTime - Session.GetHabbo()._lastTimeUsedHelpCommand;
            if (timeBetween < 60000 && Session.GetHabbo().Rank == 1)
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("abuse", "Espera 1 minuto para voltar a mudar o balão do seu bot.\n\nAdquira o vip do " + StarBlueServer.HotelName + " para evitar a espera.", "catalog/open/clubVIP"));
                return;
            }

            Session.GetHabbo()._lastTimeUsedHelpCommand = nowTime;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Você não colocou o nome do bot válido.", 34);
                return;
            }

            RoomUser Bot = Room.GetRoomUserManager().GetBotByName(Params[1]);
            if (Bot == null)
            {
                Session.SendWhisper("Não há nenhum boto chamado " + Params[1] + " no quarto.", 34);
                return;
            }

            if (Bot.BotData.ownerID != Session.GetHabbo().Id)
            {
                Session.SendWhisper("Você está mudando a bolha de um bot que não é seu, crack, máquina, figura.", 34);
                return;
            }

            if (Bubble == "1" || Bubble == "23" || Bubble == "34" || Bubble == "37")
            {
                Session.SendWhisper("Você está colocando uma bolha proibida.");
                return;
            }

            if (Params.Length == 2)
            {
                Session.SendWhisper("Uy, Você esqueceu de inserir uma ID de bolha.", 34);
                return;
            }

            if (int.TryParse(Bubble, out int BubbleID))
            {
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("UPDATE `bots` SET `chat_bubble` =  '" + BubbleID + "' WHERE `name` =  '" + Bot.BotData.Name + "' AND  `room_id` =  '" + Session.GetHabbo().CurrentRoomId + "'");
                    Bot.Chat("Você colocou uma bolha " + BubbleID + " no BOT.", true, BubbleID);
                    Bot.BotData.ChatBubble = BubbleID;
                }
            }

            return;
        }
    }
}