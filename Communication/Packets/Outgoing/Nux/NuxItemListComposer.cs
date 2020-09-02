using StarBlue.Database.Interfaces;
using System;
using System.Data;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Nux
{
    internal class NuxItemListComposer : ServerPacket
    {
        public NuxItemListComposer() : base(ServerPacketHeader.NuxItemListComposer)
        {
            base.WriteInteger(1); // Número de páginas.

            base.WriteInteger(1); // ELEMENTO 1
            base.WriteInteger(3); // ELEMENTO 2
            base.WriteInteger(3); // Número total de premios:

            using (IQueryAdapter dbQuery = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbQuery.SetQuery("SELECT * FROM `nux_gifts` LIMIT 3");
                DataTable gUsersTable = dbQuery.GetTable();

                foreach (DataRow Row in gUsersTable.Rows)
                {
                    base.WriteString(Convert.ToString(Row["image"])); // image.library.url + string
                    base.WriteInteger(1); // items:
                    base.WriteString(Convert.ToString(Row["title"])); // item_name (product_x_name)
                    base.WriteString(""); // can be null
                }
            }
        }
    }
}