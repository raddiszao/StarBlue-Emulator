using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Groups;
using StarBlue.HabboHotel.Users;
using System;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class GroupInfoComposer : MessageComposer
    {
        public Group Group { get; }
        public bool NewWindow { get; }
        public Habbo Habbo { get; }
        public DateTime Origin { get; }

        public GroupInfoComposer(Group Group, GameClient Session, bool NewWindow = false)
            : base(Composers.GroupInfoMessageComposer)
        {
            this.Group = Group;
            this.NewWindow = NewWindow;
            this.Habbo = Session.GetHabbo();
        }

        public override void Compose(Composer packet)
        {
            DateTime Origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Group.CreateTime);
            packet.WriteInteger(Group.Id);
            packet.WriteBoolean(true);
            packet.WriteInteger(Group.GroupType == GroupType.OPEN ? 0 : Group.GroupType == GroupType.LOCKED ? 1 : 2);
            packet.WriteString(Group.Name);
            packet.WriteString(Group.Description);
            packet.WriteString(Group.Badge);
            packet.WriteInteger(Group.RoomId);
            packet.WriteString((StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(Group.RoomId) == null) ? "No room found.." : StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(Group.RoomId).Name);    // room name
            packet.WriteInteger(Group.CreatorId == Habbo.Id ? 3 : Group.HasRequest(Habbo.Id) ? 2 : Group.IsMember(Habbo.Id) ? 1 : 0);
            packet.WriteInteger(Group.MemberCount); // Members
            packet.WriteBoolean(Habbo.GetStats().FavouriteGroupId == Group.Id);//?? CHANGED
            packet.WriteString(Origin.Day + "-" + Origin.Month + "-" + Origin.Year);
            packet.WriteBoolean(Group.CreatorId == Habbo.Id);
            packet.WriteBoolean(Group.IsAdmin(Habbo.Id)); // admin
            packet.WriteString(StarBlueServer.GetUsernameById(Group.CreatorId));
            packet.WriteBoolean(NewWindow); // Show group info
            packet.WriteBoolean(Group.AdminOnlyDeco == 0); // Any user can place furni in home room
            packet.WriteInteger((Group.CreatorId == Habbo.Id || Group.IsAdmin(Habbo.Id)) ? Group.RequestCount : 0); // Pending users
            packet.WriteBoolean(Group != null ? Group.ForumEnabled : true);//HabboTalk.

        }
    }
}