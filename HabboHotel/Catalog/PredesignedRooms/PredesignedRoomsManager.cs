using StarBlue.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace StarBlue.HabboHotel.Catalog.PredesignedRooms
{
    internal class PredesignedRoomsManager
    {
        internal Dictionary<uint, PredesignedRooms> predesignedRoom;
        internal void Initialize()
        {
            predesignedRoom = new Dictionary<uint, PredesignedRooms>();
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM catalog_predesigned_rooms;");
                DataTable table = dbClient.GetTable();
                foreach (DataRow row in table.Rows)
                {
                    predesignedRoom.Add(Convert.ToUInt32(row["id"]), new PredesignedRooms(Convert.ToUInt32(row["id"]),
                        Convert.ToUInt32(row["id"]), (string)row["room_model"], row["flooritems"].ToString().TrimEnd(';'),
                        row["wallitems"].ToString().TrimEnd(';'), row["catalogitems"].ToString().TrimEnd(';'),
                        (string)row["room_decoration"]));
                }
            }
        }

        internal bool Exists(uint roomId)
        {
            return (predesignedRoom.Count > 0 && predesignedRoom.Select(predesigned => predesigned.Value).Where(predesigned => predesigned.RoomId == roomId) != null);
        }
    }
}