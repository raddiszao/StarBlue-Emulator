using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class HideWiredCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Esconde os Wireds do seu quarto."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            if (!Room.CheckRights(Session, false, false))
            {
                Session.SendWhisper("Não tens permissões nesta sala.", 34);
                return;
            }

            Room.RoomData.HideWired = !Room.RoomData.HideWired;
            if (Room.RoomData.HideWired)
            {
                Session.SendWhisper("Você escondeu todos os Wireds da sala.", 34);
            }
            else
            {
                Session.SendWhisper("Você mostrou todos os Wireds da sala.", 34);
            }

            using (IQueryAdapter con = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                con.SetQuery("UPDATE `rooms` SET `hide_wired` = @enum WHERE `id` = @id LIMIT 1");
                con.AddParameter("enum", StarBlueServer.BoolToEnum(Room.RoomData.HideWired));
                con.AddParameter("id", Room.Id);
                con.RunQuery();
            }

            Room.SendMessage(Room.HideWiredMessages(Room.RoomData.HideWired));
        }
    }
}
