using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;


namespace StarBlue.Communication.Packets.Incoming.Moderation
{
    internal class ModerationCautionEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_caution"))
            {
                return;
            }

            int UserId = Packet.PopInt();
            string Message = Packet.PopString();

            GameClient Client = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(UserId);
            if (Client == null || Client.GetHabbo() == null)
            {
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE `user_info` SET `cautions` = `cautions` + '1' WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1");
            }

            Client.SendNotification(Message);
        }
    }
}