using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Groups;
using System;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class GroupInfoComposer : ServerPacket
    {
        public GroupInfoComposer(Group Group, GameClient Session, bool NewWindow = false)
            : base(ServerPacketHeader.GroupInfoMessageComposer)
        {
            DateTime Origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Group.CreateTime);

            base.WriteInteger(Group.Id);
            base.WriteBoolean(true);
            base.WriteInteger(Group.GroupType == GroupType.OPEN ? 0 : Group.GroupType == GroupType.LOCKED ? 1 : 2);
            base.WriteString(Group.Name);
            base.WriteString(Group.Description);
            base.WriteString(Group.Badge);
            base.WriteInteger(Group.RoomId);
            base.WriteString((StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(Group.RoomId) == null) ? "No room found.." : StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(Group.RoomId).Name);    // room name
            base.WriteInteger(Group.CreatorId == Session.GetHabbo().Id ? 3 : Group.HasRequest(Session.GetHabbo().Id) ? 2 : Group.IsMember(Session.GetHabbo().Id) ? 1 : 0);
            base.WriteInteger(Group.MemberCount); // Members
            base.WriteBoolean(Session.GetHabbo().GetStats().FavouriteGroupId == Group.Id);//?? CHANGED
            base.WriteString(Origin.Day + "-" + Origin.Month + "-" + Origin.Year);
            base.WriteBoolean(Group.CreatorId == Session.GetHabbo().Id);
            base.WriteBoolean(Group.IsAdmin(Session.GetHabbo().Id)); // admin
            base.WriteString(StarBlueServer.GetUsernameById(Group.CreatorId));
            base.WriteBoolean(NewWindow); // Show group info
            base.WriteBoolean(Group.AdminOnlyDeco == 0); // Any user can place furni in home room
            base.WriteInteger((Group.CreatorId == Session.GetHabbo().Id || Group.IsAdmin(Session.GetHabbo().Id)) ? Group.RequestCount : 0); // Pending users
            base.WriteBoolean(Group != null ? Group.ForumEnabled : true);//HabboTalk.

        }
    }
}