using StarBlue.Database.Interfaces;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class CloseRoomCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Fecha o quarto atual.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session))
            {
                Session.SendWhisper("Oops, somente pessoas com direitos podem usar este comando!", 34);
                return;
            }

            Room.RoomData.Access = RoomAccess.DOORBELL;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE rooms SET state = 'locked' WHERE `id` = '" + Room.Id + "' LIMIT 1");
            }

            Session.SendWhisper("O quarto foi trancado.", 34);
        }
    }
}
