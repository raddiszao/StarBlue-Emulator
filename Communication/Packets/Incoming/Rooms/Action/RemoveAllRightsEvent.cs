using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Communication.Packets.Outgoing.Rooms.Permissions;
using StarBlue.Communication.Packets.Outgoing.Rooms.Settings;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Generic;
using System.Linq;


namespace StarBlue.Communication.Packets.Incoming.Rooms.Action
{
    internal class RemoveAllRightsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }


            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Instance))
            {
                return;
            }

            if (!Instance.CheckRights(Session, true))
            {
                return;
            }

            foreach (int UserId in new List<int>(Instance.UsersWithRights))
            {
                RoomUser User = Instance.GetRoomUserManager().GetRoomUserByHabbo(UserId);
                if (User != null && !User.IsBot)
                {
                    User.RemoveStatus("flatctrl 1");
                    User.UpdateNeeded = true;

                    User.GetClient().SendMessage(new YouAreControllerComposer(0));
                }

                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("DELETE FROM `room_rights` WHERE `user_id` = @uid AND `room_id` = @rid LIMIT 1");
                    dbClient.AddParameter("uid", UserId);
                    dbClient.AddParameter("rid", Instance.Id);
                    dbClient.RunQuery();
                }

                Session.SendMessage(new FlatControllerRemovedComposer(Instance, UserId));
                Session.SendMessage(new RoomRightsListComposer(Instance));
                Session.SendMessage(new UserUpdateComposer(Instance.GetRoomUserManager().GetUserList().ToList()));
            }

            if (Instance.UsersWithRights.Count > 0)
            {
                Instance.UsersWithRights.Clear();
            }
        }
    }
}
