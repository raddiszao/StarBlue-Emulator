using StarBlue.Database.Interfaces;


namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator
{
    internal class DeleteGroupCommand : IChatCommand
    {
        public string PermissionRequired => "user_16";
        public string Parameters => "";
        public string Description => "Deleta um grupo do banco de dados e do cache.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            if (Room.RoomData.Group == null)
            {
                Session.SendWhisper("Oops, não há nenhum grupo aqui.", 34);
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("DELETE FROM `groups` WHERE `id` = '" + Room.RoomData.Group.Id + "'");
                dbClient.RunFastQuery("DELETE FROM `group_memberships` WHERE `group_id` = '" + Room.RoomData.Group.Id + "'");
                dbClient.RunFastQuery("DELETE FROM `group_requests` WHERE `group_id` = '" + Room.RoomData.Group.Id + "'");
                dbClient.RunFastQuery("UPDATE `rooms` SET `group_id` = '0' WHERE `group_id` = '" + Room.RoomData.Group.Id + "' LIMIT 1");
                dbClient.RunFastQuery("UPDATE `user_stats` SET `groupid` = '0' WHERE `groupid` = '" + Room.RoomData.Group.Id + "' LIMIT 1");
                dbClient.RunFastQuery("DELETE FROM `items_groups` WHERE `group_id` = '" + Room.RoomData.Group.Id + "'");
            }

            StarBlueServer.GetGame().GetGroupManager().DeleteGroup(Room.RoomData.Group.Id);

            Room.RoomData.Group = null;

            StarBlueServer.GetGame().GetRoomManager().UnloadRoom(Room.Id, true);

            Session.SendNotification("Grupo eliminado satisfatoriamente.");
            return;
        }
    }
}
