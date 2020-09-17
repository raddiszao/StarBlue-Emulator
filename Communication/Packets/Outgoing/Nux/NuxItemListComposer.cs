using StarBlue.Database.Interfaces;
using System;
using System.Data;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Nux
{
    internal class NuxItemListComposer : MessageComposer
    {
        public NuxItemListComposer() : base(Composers.NuxItemListComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(1); // Número de páginas.

            packet.WriteInteger(1); // ELEMENTO 1
            packet.WriteInteger(3); // ELEMENTO 2
            packet.WriteInteger(3); // Número total de premios:

            using (IQueryAdapter dbQuery = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbQuery.SetQuery("SELECT * FROM `nux_gifts` LIMIT 3");
                DataTable gUsersTable = dbQuery.GetTable();

                foreach (DataRow Row in gUsersTable.Rows)
                {
                    packet.WriteString(Convert.ToString(Row["image"])); // image.library.url + string
                    packet.WriteInteger(1); // items:
                    packet.WriteString(Convert.ToString(Row["title"])); // item_name (product_x_name)
                    packet.WriteString(""); // can be null
                }
            }
        }
    }
}