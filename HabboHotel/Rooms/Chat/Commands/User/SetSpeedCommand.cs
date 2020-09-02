using StarBlue.Database.Interfaces;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class SetSpeedCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "[NUMERO]";

        public string Description => "Escolha a velocidade dos Rollers de 0 a 10.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
            {
                return;
            }

            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite a velocidade dos rollers.", 34);
                return;
            }

            if (int.TryParse(Params[1], out int Speed))
            {
                if (Speed < 100)
                {
                    Session.GetHabbo().CurrentRoom.GetRoomItemHandler().SetSpeed(Speed);
                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunFastQuery("UPDATE `rooms` SET `roller_speed` = " + Speed + " WHERE `id` = '" + Room.Id + "' LIMIT 1");
                        Room.RoomData.RollerSpeed = Speed;
                        Session.SendWhisper("Sucesso, velocidade dos rollers definida para " + Speed + ".");
                    }
                }
                else
                {
                    Session.SendWhisper("Velocidade não permitida.");
                }
            }
            else
            {
                Session.SendWhisper("Quantidade invalida, só é permitido números.", 34);
            }
        }
    }
}