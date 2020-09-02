using StarBlue.Database.Interfaces;
using System;
using System.Data;


namespace StarBlue.Communication.Packets.Outgoing.Avatar
{
    internal class WardrobeComposer : ServerPacket
    {
        public WardrobeComposer(int UserId)
            : base(ServerPacketHeader.WardrobeMessageComposer)
        {
            WriteInteger(1);
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `slot_id`,`look`,`gender` FROM `user_wardrobe` WHERE `user_id` = '" + UserId + "'");
                DataTable WardrobeData = dbClient.GetTable();

                if (WardrobeData == null)
                {
                    WriteInteger(0);
                }
                else
                {
                    WriteInteger(WardrobeData.Rows.Count);
                    foreach (DataRow Row in WardrobeData.Rows)
                    {
                        WriteInteger(Convert.ToInt32(Row["slot_id"]));
                        WriteString(Convert.ToString(Row["look"]));
                        WriteString(Row["gender"].ToString().ToUpper());
                    }
                }
            }
        }
    }
}
