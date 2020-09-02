using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using StarBlue.Database.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class MakePublicCommand : IChatCommand
    {
        public string PermissionRequired => "user_16";
        public string Parameters => "";
        public string Description => "Converter este quarto em público.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Room room = Session.GetHabbo().CurrentRoom;
            if (room == null)
            {
                return;
            }

            using (IQueryAdapter queryReactor = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.RunFastQuery(string.Format("UPDATE rooms SET roomtype = 'public' WHERE id = {0}", room.Id));
            }

            room.RoomData.Type = "public";

            int roomId = Session.GetHabbo().CurrentRoom.Id;
            List<RoomUser> users = new List<RoomUser>(Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUsers().ToList());

            StarBlueServer.GetGame().GetRoomManager().UnloadRoom(Session.GetHabbo().CurrentRoom.Id);

            RoomData Data = StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(roomId);
            Session.GetHabbo().PrepareRoom(roomId, "");

            StarBlueServer.GetGame().GetRoomManager().LoadRoom(roomId);

            RoomForwardComposer data = new RoomForwardComposer(roomId);

            foreach (RoomUser user in users.Where(user => user != null && user.GetClient() != null))
            {
                user.GetClient().SendMessage(data);
            }
        }
    }
}
