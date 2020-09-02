using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Cache;
using StarBlue.HabboHotel.Rooms;
using System;
using System.Data;

namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorUserChatlogComposer : ServerPacket
    {
        public ModeratorUserChatlogComposer(int UserId)
            : base(ServerPacketHeader.ModeratorUserChatlogMessageComposer)
        {
            base.WriteInteger(UserId);
            base.WriteString(StarBlueServer.GetGame().GetClientManager().GetNameById(UserId));
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT room_id,entry_timestamp,exit_timestamp FROM user_roomvisits WHERE `user_id` = " + UserId + " ORDER BY entry_timestamp DESC LIMIT 5");
                DataTable Visits = dbClient.GetTable();

                if (Visits != null)
                {
                    base.WriteInteger(Visits.Rows.Count);
                    foreach (DataRow Visit in Visits.Rows)
                    {
                        string RoomName = "Unknown";

                        Room Room = StarBlueServer.GetGame().GetRoomManager().LoadRoom(Convert.ToInt32(Visit["room_id"]));

                        if (Room != null)
                        {
                            RoomName = Room.RoomData.Name;
                        }

                        base.WriteByte(1);
                        base.WriteShort(2);//Count
                        base.WriteString("roomName");
                        base.WriteByte(2);
                        base.WriteString(RoomName); // room name
                        base.WriteString("roomId");
                        base.WriteByte(1);
                        base.WriteInteger(Convert.ToInt32(Visit["room_id"]));

                        DataTable Chatlogs = null;
                        if ((double)Visit["exit_timestamp"] <= 0)
                        {
                            Visit["exit_timestamp"] = StarBlueServer.GetUnixTimestamp();
                        }

                        dbClient.SetQuery("SELECT user_id,timestamp,message FROM `chatlogs` WHERE room_id = " + Convert.ToInt32(Visit["room_id"]) + " AND timestamp > " + (double)Visit["entry_timestamp"] + " AND timestamp < " + (double)Visit["exit_timestamp"] + " ORDER BY timestamp DESC LIMIT 150");
                        Chatlogs = dbClient.GetTable();

                        if (Chatlogs != null)
                        {
                            base.WriteShort(Chatlogs.Rows.Count);
                            foreach (DataRow Log in Chatlogs.Rows)
                            {
                                UserCache Habbo = StarBlueServer.GetGame().GetCacheManager().GenerateUser(Convert.ToInt32(Log["user_id"]));

                                if (Habbo == null)
                                {
                                    continue;
                                }

                                DateTime dDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                                dDateTime = dDateTime.AddSeconds(Convert.ToInt32(Log["timestamp"])).ToLocalTime();

                                base.WriteString(dDateTime.Hour + ":" + dDateTime.Minute);
                                base.WriteInteger(Habbo.Id);
                                base.WriteString(Habbo.Username);
                                base.WriteString(string.IsNullOrWhiteSpace(Convert.ToString(Log["message"])) ? "*stemen*" : Convert.ToString(Log["message"]));
                                base.WriteBoolean(false);
                            }
                        }
                        else
                        {
                            base.WriteInteger(0);
                        }
                    }
                }
                else
                {
                    base.WriteInteger(0);
                }
            }
        }
    }
}