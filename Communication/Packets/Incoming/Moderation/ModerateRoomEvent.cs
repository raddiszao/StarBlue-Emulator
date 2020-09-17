using StarBlue.Communication.Packets.Outgoing.Navigator;
using StarBlue.Communication.Packets.Outgoing.Rooms.Settings;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Rooms;
using System.Linq;


namespace StarBlue.Communication.Packets.Incoming.Moderation
{
    internal class ModerateRoomEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                return;
            }

            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Packet.PopInt(), out Room Room))
            {
                return;
            }

            bool SetLock = Packet.PopInt() == 1;
            bool SetName = Packet.PopInt() == 1;
            bool KickAll = Packet.PopInt() == 1;

            if (SetName)
            {
                Room.RoomData.Name = "Quarto inapropriado, infringiu as regras.";
                Room.RoomData.Description = "Quarto inapropriado, infringiu as regras do " + StarBlueServer.HotelName;
            }

            if (SetLock)
            {
                Room.RoomData.Access = RoomAccess.DOORBELL;
            }

            if (Room.RoomData.Tags.Count > 0)
            {
                Room.ClearTags();
            }

            if (Room.RoomData.HasActivePromotion)
            {
                Room.RoomData.EndPromotion();
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                if (SetName && SetLock)
                {
                    dbClient.RunFastQuery("UPDATE `rooms` SET `caption` = 'Inappropriate to Hotel Management', `description` = 'Inappropriate to Hotel Management', `tags` = '', `state` = '1' WHERE `id` = '" + Room.Id + "' LIMIT 1");
                }
                else if (SetName && !SetLock)
                {
                    dbClient.RunFastQuery("UPDATE `rooms` SET `caption` = 'Inappropriate to Hotel Management', `description` = 'Inappropriate to Hotel Management', `tags` = '' WHERE `id` = '" + Room.Id + "' LIMIT 1");
                }
                else if (!SetName && SetLock)
                {
                    dbClient.RunFastQuery("UPDATE `rooms` SET `state` = '1', `tags` = '' WHERE `id` = '" + Room.Id + "' LIMIT 1");
                }
            }

            Room.SendMessage(new RoomSettingsSavedComposer(Room.Id));
            Room.SendMessage(new RoomInfoUpdatedComposer(Room.Id));

            if (KickAll)
            {
                foreach (RoomUser RoomUser in Room.GetRoomUserManager().GetUserList().ToList())
                {
                    if (RoomUser == null || RoomUser.IsBot)
                    {
                        continue;
                    }

                    if (RoomUser.GetClient() == null || RoomUser.GetClient().GetHabbo() == null)
                    {
                        continue;
                    }

                    if (RoomUser.GetClient().GetHabbo().Rank >= Session.GetHabbo().Rank || RoomUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)
                    {
                        continue;
                    }

                    Room.GetRoomUserManager().RemoveUserFromRoom(RoomUser.GetClient(), true, false);
                }
            }
        }
    }
}
