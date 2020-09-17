using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Cache;
using StarBlue.HabboHotel.Rooms;
using System;
using System.Data;

namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorUserChatlogComposer : MessageComposer
    {
        private int UserId { get; }

        public ModeratorUserChatlogComposer(int UserId)
            : base(Composers.ModeratorUserChatlogMessageComposer)
        {
            this.UserId = UserId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(UserId);
            packet.WriteString(StarBlueServer.GetGame().GetClientManager().GetNameById(UserId));
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT room_id,entry_timestamp,exit_timestamp FROM user_roomvisits WHERE `user_id` = " + UserId + " ORDER BY entry_timestamp DESC LIMIT 5");
                DataTable Visits = dbClient.GetTable();

                if (Visits != null)
                {
                    packet.WriteInteger(Visits.Rows.Count);
                    foreach (DataRow Visit in Visits.Rows)
                    {
                        string RoomName = "Unknown";

                        Room Room = StarBlueServer.GetGame().GetRoomManager().LoadRoom(Convert.ToInt32(Visit["room_id"]));

                        if (Room != null)
                        {
                            RoomName = Room.RoomData.Name;
                        }

                        packet.WriteByte(1);
                        packet.WriteShort(2);//Count
                        packet.WriteString("roomName");
                        packet.WriteByte(2);
                        packet.WriteString(RoomName); // room name
                        packet.WriteString("roomId");
                        packet.WriteByte(1);
                        packet.WriteInteger(Convert.ToInt32(Visit["room_id"]));

                        DataTable Chatlogs = null;
                        if ((double)Visit["exit_timestamp"] <= 0)
                        {
                            Visit["exit_timestamp"] = StarBlueServer.GetUnixTimestamp();
                        }

                        dbClient.SetQuery("SELECT user_id,timestamp,message FROM `chatlogs` WHERE room_id = " + Convert.ToInt32(Visit["room_id"]) + " AND timestamp > " + (double)Visit["entry_timestamp"] + " AND timestamp < " + (double)Visit["exit_timestamp"] + " ORDER BY timestamp DESC LIMIT 150");
                        Chatlogs = dbClient.GetTable();

                        if (Chatlogs != null)
                        {
                            packet.WriteShort(Chatlogs.Rows.Count);
                            foreach (DataRow Log in Chatlogs.Rows)
                            {
                                UserCache Habbo = StarBlueServer.GetGame().GetCacheManager().GenerateUser(Convert.ToInt32(Log["user_id"]));

                                if (Habbo == null)
                                {
                                    continue;
                                }

                                DateTime dDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                                dDateTime = dDateTime.AddSeconds(Convert.ToInt32(Log["timestamp"])).ToLocalTime();

                                packet.WriteString(dDateTime.Hour + ":" + dDateTime.Minute);
                                packet.WriteInteger(Habbo.Id);
                                packet.WriteString(Habbo.Username);
                                packet.WriteString(string.IsNullOrWhiteSpace(Convert.ToString(Log["message"])) ? "*stemen*" : Convert.ToString(Log["message"]));
                                packet.WriteBoolean(false);
                            }
                        }
                        else
                        {
                            packet.WriteInteger(0);
                        }
                    }
                }
                else
                {
                    packet.WriteInteger(0);
                }
            }
        }
    }
}