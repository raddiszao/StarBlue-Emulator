using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;


namespace StarBlue.Communication.Packets.Incoming.Users
{
    internal class SetChatPreferenceEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            bool ChatPreference = Packet.PopBoolean();

            Session.GetHabbo().ChatPreference = ChatPreference;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `chat_preference` = @chatPreference WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("chatPreference", StarBlueServer.BoolToEnum(ChatPreference));
                dbClient.RunQuery();
            }
        }
    }
}
