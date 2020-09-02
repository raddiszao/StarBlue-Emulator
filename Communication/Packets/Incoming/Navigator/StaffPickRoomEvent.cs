using StarBlue.Communication.Packets.Outgoing.Navigator;
using StarBlue.Communication.Packets.Outgoing.Rooms.Settings;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Navigator;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Navigator
{
    internal class StaffPickRoomEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(session.GetHabbo().CurrentRoom.RoomData.OwnerName);

            if (!session.GetHabbo().GetPermissions().HasRight("room.staff_picks.management"))
            {
                return;
            }

            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(packet.PopInt(), out Room room))
            {
                return;
            }

            if (!StarBlueServer.GetGame().GetNavigator().TryGetStaffPickedRoom(room.Id, out StaffPick staffPick))
            {
                if (StarBlueServer.GetGame().GetNavigator().TryAddStaffPickedRoom(room.Id))
                {
                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("INSERT INTO `navigator_staff_picks` (`room_id`,`image`) VALUES (@roomId, null)");
                        dbClient.AddParameter("roomId", room.Id);
                        dbClient.RunQuery();
                    }
                    if (TargetClient != null)
                    {
                        StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(TargetClient, "ACH_Spr", 1, false);
                    }
                }
            }

            else
            {
                if (StarBlueServer.GetGame().GetNavigator().TryRemoveStaffPickedRoom(room.Id))
                {
                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("DELETE FROM `navigator_staff_picks` WHERE `room_id` = @roomId LIMIT 1");
                        dbClient.AddParameter("roomId", room.Id);
                        dbClient.RunQuery();
                    }
                }
            }

            room.SendMessage(new RoomSettingsSavedComposer(room.Id));
            room.SendMessage(new RoomInfoUpdatedComposer(room.Id));
        }
    }
}