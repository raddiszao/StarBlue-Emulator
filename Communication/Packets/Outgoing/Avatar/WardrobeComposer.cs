using StarBlue.Database.Interfaces;
using System;
using System.Data;


namespace StarBlue.Communication.Packets.Outgoing.Avatar
{
    internal class WardrobeComposer : MessageComposer
    {
        private int UserId { get; }

        public WardrobeComposer(int UserId)
            : base(Composers.WardrobeMessageComposer)
        {
            this.UserId = UserId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(1);
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `slot_id`,`look`,`gender` FROM `user_wardrobe` WHERE `user_id` = '" + UserId + "'");
                DataTable WardrobeData = dbClient.GetTable();

                if (WardrobeData == null)
                {
                    packet.WriteInteger(0);
                }
                else
                {
                    packet.WriteInteger(WardrobeData.Rows.Count);
                    foreach (DataRow Row in WardrobeData.Rows)
                    {
                        packet.WriteInteger(Convert.ToInt32(Row["slot_id"]));
                        packet.WriteString(Convert.ToString(Row["look"]));
                        packet.WriteString(Row["gender"].ToString().ToUpper());
                    }
                }
            }
        }
    }
}
