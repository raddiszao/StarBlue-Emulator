
using StarBlue.Communication.Packets.Outgoing.Groups;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Groups
{
    internal class RemoveGroupFavouriteEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            Session.GetHabbo().GetStats().FavouriteGroupId = 0;

            if (Session.GetHabbo().InRoom)
            {
                RoomUser User = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (User != null)
                {
                    Session.GetHabbo().CurrentRoom.SendMessage(new UpdateFavouriteGroupComposer(null, User.VirtualId));
                }

                Session.GetHabbo().CurrentRoom.SendMessage(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
            }
            else
            {
                Session.SendMessage(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
            }
        }
    }
}
