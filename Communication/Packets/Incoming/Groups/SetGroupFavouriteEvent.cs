
using StarBlue.Communication.Packets.Outgoing.Groups;
using StarBlue.Communication.Packets.Outgoing.Users;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Groups;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Groups
{
    internal class SetGroupFavouriteEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (Session == null)
            {
                return;
            }

            int GroupId = Packet.PopInt();
            if (GroupId == 0)
            {
                return;
            }

            if (!StarBlueServer.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            Session.GetHabbo().GetStats().FavouriteGroupId = Group.Id;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE `user_stats` SET `groupid` = " + Session.GetHabbo().GetStats().FavouriteGroupId + " WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            if (Session.GetHabbo().InRoom && Session.GetHabbo().CurrentRoom != null)
            {
                Session.GetHabbo().CurrentRoom.SendMessage(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
                if (Group != null)
                {
                    Session.GetHabbo().CurrentRoom.SendMessage(new HabboGroupBadgesComposer(Group));

                    RoomUser User = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                    if (User != null)
                    {
                        Session.GetHabbo().CurrentRoom.SendMessage(new UpdateFavouriteGroupComposer(Group, User.VirtualId));
                    }
                }
            }
            else
            {
                Session.SendMessage(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
            }
        }
    }
}
