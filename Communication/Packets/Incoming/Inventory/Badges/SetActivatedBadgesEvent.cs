
using StarBlue.Communication.Packets.Outgoing.Users;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Quests;
using StarBlue.HabboHotel.Rooms;


namespace StarBlue.Communication.Packets.Incoming.Inventory.Badges
{
    internal class SetActivatedBadgesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.GetHabbo().GetBadgeComponent().ResetSlots();

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE `user_badges` SET `badge_slot` = '0' WHERE `user_id` = '" + Session.GetHabbo().Id + "'");
            }

            for (int i = 0; i < 5; i++)
            {
                int Slot = Packet.PopInt();
                string Badge = Packet.PopString();

                if (Badge.Length == 0)
                {
                    continue;
                }

                if (!Session.GetHabbo().GetBadgeComponent().HasBadge(Badge) || Slot < 1 || Slot > 5)
                {
                    return;
                }

                Session.GetHabbo().GetBadgeComponent().GetBadge(Badge).Slot = Slot;

                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `user_badges` SET `badge_slot` = " + Slot + " WHERE `badge_id` = @badge AND `user_id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    dbClient.AddParameter("badge", Badge);
                    dbClient.RunQuery();
                }
            }

            StarBlueServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.PROFILE_BADGE);


            if (Session.GetHabbo().InRoom && StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                Session.GetHabbo().CurrentRoom.SendMessage(new HabboUserBadgesComposer(Session.GetHabbo()));
            }
            else
            {
                Session.SendMessage(new HabboUserBadgesComposer(Session.GetHabbo()));
            }
        }
    }
}
