using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class MakePrivateCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";
        public string Parameters => "";
        public string Description => "Converter este quarto em privado.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            var room = Session.GetHabbo().CurrentRoom;
            using (var queryReactor = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.RunFastQuery(string.Format("UPDATE rooms SET roomtype = 'private' WHERE id = {0}",
                    room.RoomId));
            }

            var roomId = Session.GetHabbo().CurrentRoom.RoomId;
            var users = new List<RoomUser>(Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUsers().ToList());

            StarBlueServer.GetGame().GetRoomManager().UnloadRoom(Session.GetHabbo().CurrentRoom.Id);

            RoomData Data = StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(roomId);
            Session.GetHabbo().PrepareRoom(Session.GetHabbo().CurrentRoom.RoomId, "");

            StarBlueServer.GetGame().GetRoomManager().LoadRoom(roomId);

            var data = new RoomForwardComposer(roomId);

            foreach (var user in users.Where(user => user != null && user.GetClient() != null))
            {
                user.GetClient().SendMessage(data);
            }
        }
    }
}
