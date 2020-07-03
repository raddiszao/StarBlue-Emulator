using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class SetMaxCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "[NUMERO]";

        public string Description => "Aumenta ou reduz o numero maximo em seu quarto.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
            {
                return;
            }

            if (Params.Length == 1)
            {
                Session.SendWhisper("Insira uma quantia inteira (em números) de quantos usuários podem entrar no seu quarto.", 34);
                return;
            }

            if (int.TryParse(Params[1], out int MaxAmount))
            {
                if (MaxAmount <= 0)
                {
                    MaxAmount = 10;
                    Session.SendWhisper("Quantidade de usuários muito baixa, a quantidade de usuários foi colocada para 10.", 34);
                }
                else if (MaxAmount > 250 && !Session.GetHabbo().GetPermissions().HasRight("override_command_setmax_limit"))
                {
                    MaxAmount = 250;
                    Session.SendWhisper("Quantidade de usuários muito alta, a quantidade de usuários foi colocada para 250.", 34);
                }
                else
                {
                    Room.UsersMax = MaxAmount;
                }

                Room.RoomData.UsersMax = MaxAmount;
                Room.SendMessage(RoomNotificationComposer.SendBubble("setmax", "" + Session.GetHabbo().Username + " estabeleceu um limite de usuários para " + MaxAmount + ".", ""));

                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("UPDATE `rooms` SET `users_max` = " + MaxAmount + " WHERE `id` = '" + Room.Id + "' LIMIT 1");
                }
            }
            else
            {
                Session.SendWhisper("Quantidade invalida, só é permitido numeros.", 34);
            }
        }
    }
}
