
using StarBlue.Communication.Packets.Outgoing.Groups;
using StarBlue.Communication.Packets.Outgoing.Rooms.Permissions;
using StarBlue.HabboHotel.Groups;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;



namespace StarBlue.Communication.Packets.Incoming.Groups
{
    internal class TakeAdminRightsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            if (!StarBlueServer.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if (!Group.IsMember(UserId) || !Group.IsAdmin(Session.GetHabbo().Id))
            {
                return;
            }

            Habbo Habbo = StarBlueServer.GetHabboById(UserId);
            if (Habbo == null)
            {
                Session.SendNotification("Oops, ocurrio un error mientras se realizaba la busqueda de este usuario.");
                return;
            }

            Group.TakeAdmin(UserId);

            if (StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room Room))
            {
                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(UserId);
                if (User != null)
                {
                    if (User.Statusses.ContainsKey("flatctrl 3"))
                    {
                        User.RemoveStatus("flatctrl 3");
                    }

                    User.UpdateNeeded = true;
                    if (User.GetClient() != null)
                    {
                        User.GetClient().SendMessage(new YouAreControllerComposer(0));
                    }
                }
            }

            Session.SendMessage(new GroupMemberUpdatedComposer(GroupId, Habbo, 2));
        }
    }
}
