using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Cache;
using StarBlue.HabboHotel.Rooms;
using System;
using System.Data;

namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorRoomChatlogComposer : MessageComposer
    {
        private Room Room { get; }

        public ModeratorRoomChatlogComposer(Room Room)
            : base(Composers.ModeratorRoomChatlogMessageComposer)
        {
            this.Room = Room;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteByte(1);
            packet.WriteShort(2);//Count
            packet.WriteString("roomName");
            packet.WriteByte(2);
            packet.WriteString(Room.RoomData.Name);
            packet.WriteString("roomId");
            packet.WriteByte(1);
            packet.WriteInteger(Room.Id);

            DataTable Table = null;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `chatlogs` WHERE `room_id` = @rid ORDER BY `id` DESC LIMIT 250");
                dbClient.AddParameter("rid", Room.Id);
                Table = dbClient.GetTable();
            }

            if (Table == null)
                return;

            packet.WriteShort(Table.Rows.Count);
            if (Table != null)
            {
                foreach (DataRow Row in Table.Rows)
                {
                    UserCache Habbo = StarBlueServer.GetGame().GetCacheManager().GenerateUser(Convert.ToInt32(Row["user_id"]));

                    if (Habbo == null)
                    {
                        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        dtDateTime = dtDateTime.AddSeconds(Convert.ToInt32(Row["timestamp"])).ToLocalTime();

                        packet.WriteString(dtDateTime.Hour + ":" + dtDateTime.Minute);
                        packet.WriteInteger(-1);
                        packet.WriteString("Unknown User");
                        packet.WriteString(string.IsNullOrWhiteSpace(Convert.ToString(Row["message"])) ? "*user sent a blank message*" : Convert.ToString(Row["message"]));
                        packet.WriteBoolean(false);
                    }
                    else
                    {
                        DateTime dDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        dDateTime = dDateTime.AddSeconds(Convert.ToInt32(Row["timestamp"])).ToLocalTime();

                        packet.WriteString(dDateTime.Hour + ":" + dDateTime.Minute);
                        packet.WriteInteger(Habbo.Id);
                        packet.WriteString(Habbo.Username);
                        packet.WriteString(string.IsNullOrWhiteSpace(Convert.ToString(Row["message"])) ? "*user sent a blank message*" : Convert.ToString(Row["message"]));
                        packet.WriteBoolean(false);
                    }
                }
            }
        }
    }
}
