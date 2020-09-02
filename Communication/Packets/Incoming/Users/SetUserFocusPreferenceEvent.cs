using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;


namespace StarBlue.Communication.Packets.Incoming.Users
{
    internal class SetUserFocusPreferenceEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            bool FocusPreference = Packet.PopBoolean();

            Session.GetHabbo().FocusPreference = FocusPreference;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `focus_preference` = @focusPreference WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("focusPreference", StarBlueServer.BoolToEnum(FocusPreference));
                dbClient.RunQuery();
            }
        }
    }
}
