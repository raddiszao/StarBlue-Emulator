using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using StarBlue.Database.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class MakePrivateCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";
        public string Parameters => "";
        public string Description => "Converter este quarto em privado.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Room room = Session.GetHabbo().CurrentRoom;
            if (Session.GetHabbo() == null || room == null)
            {
                return;
            }

            using (IQueryAdapter queryReactor = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.RunFastQuery(string.Format("UPDATE rooms SET roomtype = 'private' WHERE id = {0}",
                    room.Id));
            }

            room.RoomData.Type = "private";

            int roomId = room.Id;
            List<RoomUser> users = new List<RoomUser>(room.GetRoomUserManager().GetRoomUsers().ToList());

            StarBlueServer.GetGame().GetRoomManager().UnloadRoom(room.Id);

            RoomData Data = StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(roomId);
            Session.GetHabbo().PrepareRoom(room.Id, "");

            StarBlueServer.GetGame().GetRoomManager().LoadRoom(roomId);

            RoomForwardComposer data = new RoomForwardComposer(roomId);

            foreach (RoomUser user in users.Where(user => user != null && user.GetClient() != null))
            {
                user.GetClient().SendMessage(data);
            }
        }
    }
}
