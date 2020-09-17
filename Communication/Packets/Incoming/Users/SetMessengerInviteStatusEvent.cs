using StarBlue.Database.Interfaces;


namespace StarBlue.Communication.Packets.Incoming.Users
{
    internal class SetMessengerInviteStatusEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            bool Status = Packet.PopBoolean();

            Session.GetHabbo().AllowMessengerInvites = Status;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `ignore_invites` = @MessengerInvites WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("MessengerInvites", StarBlueServer.BoolToEnum(Status));
                dbClient.RunQuery();
            }
        }
    }
}
