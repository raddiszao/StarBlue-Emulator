using StarBlue.Communication.Packets.Outgoing.Catalog;
using StarBlue.Communication.Packets.Outgoing.Groups;
using StarBlue.Communication.Packets.Outgoing.Messenger;
using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Groups;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Groups

{
    internal class JoinGroupEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            if (!StarBlueServer.GetGame().GetGroupManager().TryGetGroup(Packet.PopInt(), out Group Group))
            {
                return;
            }

            if (Group.IsMember(Session.GetHabbo().Id) || Group.IsAdmin(Session.GetHabbo().Id) || (Group.HasRequest(Session.GetHabbo().Id) && Group.GroupType == GroupType.PRIVATE))
            {
                return;
            }

            List<Group> Groups = StarBlueServer.GetGame().GetGroupManager().GetGroupsForUser(Session.GetHabbo().Id);
            if (Groups.Count >= 1500)
            {
                Session.SendMessage(new BroadcastMessageAlertComposer("Oops, parece que has alcanzado el limite de pertenencia en un grupo, solo te puedes inscribir en 1500 grupos."));
                return;
            }

            Group.AddMember(Session.GetHabbo().Id);

            if (Group.GroupType == GroupType.LOCKED)
            {
                List<GameClient> GroupAdmins = (from Client in StarBlueServer.GetGame().GetClientManager().GetClients.ToList() where Client != null && Client.GetHabbo() != null && Group.IsAdmin(Client.GetHabbo().Id) select Client).ToList();
                foreach (GameClient Client in GroupAdmins)
                {
                    Client.SendMessage(new GroupMembershipRequestedComposer(Group.Id, Session.GetHabbo(), 3));
                }

                Session.SendMessage(new GroupInfoComposer(Group, Session));
            }
            else
            {
                Session.SendMessage(new GroupFurniConfigComposer(StarBlueServer.GetGame().GetGroupManager().GetGroupsForUser(Session.GetHabbo().Id)));
                Session.SendMessage(new GroupInfoComposer(Group, Session));

                if (Session.GetHabbo().CurrentRoom != null)
                {
                    Session.GetHabbo().CurrentRoom.SendMessage(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
                }
                else
                {
                    Session.SendMessage(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
                }

                if (Group.HasChat)
                {
                    Session.SendMessage(new FriendListUpdateComposer(Group, 1));
                }
            }

        }
    }
}
