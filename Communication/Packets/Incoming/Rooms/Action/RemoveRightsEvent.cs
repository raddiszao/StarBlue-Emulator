
using StarBlue.Communication.Packets.Outgoing.Rooms.Permissions;
using StarBlue.Communication.Packets.Outgoing.Rooms.Settings;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Rooms;


namespace StarBlue.Communication.Packets.Incoming.Rooms.Action
{
    internal class RemoveRightsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }


            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            if (!Room.CheckRights(Session, true))
            {
                return;
            }

            int Amount = Packet.PopInt();
            for (int i = 0; (i < Amount && i <= 100); i++)
            {
                int UserId = Packet.PopInt();
                if (UserId > 0 && Room.UsersWithRights.Contains(UserId))
                {
                    RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(UserId);
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
                        dbClient.AddParameter("rid", Room.Id);
                        dbClient.RunQuery();
                    }

                    if (Room.UsersWithRights.Contains(UserId))
                    {
                        Room.UsersWithRights.Remove(UserId);
                    }

                    Session.SendMessage(new FlatControllerRemovedComposer(Room.Id, UserId));
                }
            }
        }
    }
}
