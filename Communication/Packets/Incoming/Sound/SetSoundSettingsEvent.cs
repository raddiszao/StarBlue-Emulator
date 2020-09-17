using StarBlue.Database.Interfaces;


namespace StarBlue.Communication.Packets.Incoming.Sound
{
    internal class SetSoundSettingsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            string Volume = "";
            for (int i = 0; i < 3; i++)
            {
                int Vol = Packet.PopInt();
                if (Vol < 0 || Vol > 100)
                {
                    Vol = 100;
                }

                if (i < 2)
                {
                    Volume += Vol + ",";
                }
                else
                {
                    Volume += Vol;
                }
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET volume = @volume WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("volume", Volume);
                dbClient.RunQuery();
            }
        }
    }
}
