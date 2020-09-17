using StarBlue.Communication.Packets.Outgoing.Rooms.Camera;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Camera
{
    public class PublishCameraPictureMessageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            bool isOk = false;
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            Room Room = Session.GetHabbo().CurrentRoom;

            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (User == null || Session.GetHabbo().lastPhotoPreview.Equals(""))
            {
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT IGNORE INTO camera_photos VALUES (@id, '" + Session.GetHabbo().Id + "', @creator_name, '" + Session.GetRoomUser().RoomId + "','" + StarBlueServer.GetUnixTimestamp() + "')");
                dbClient.AddParameter("id", URLPost.GetMD5(Session.GetHabbo().lastPhotoPreview));
                dbClient.AddParameter("creator_name", Session.GetHabbo().Username);
                dbClient.RunQuery();
                isOk = true;
            }

            Session.SendMessage(new CameraFinishPublishMessageComposer(isOk, 4, URLPost.GetMD5(Session.GetHabbo().lastPhotoPreview)));
        }
    }
}