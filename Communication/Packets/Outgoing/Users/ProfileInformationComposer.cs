using StarBlue.HabboHotel.Groups;
using StarBlue.HabboHotel.Users;
using System;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Users
{
    internal class ProfileInformationComposer : MessageComposer
    {
        private Habbo TargetHabbo { get; }
        private Habbo MyHabbo { get; }

        public ProfileInformationComposer(Habbo Habbo, Habbo MyHabbo)
            : base(Composers.ProfileInformationMessageComposer)
        {
            this.TargetHabbo = Habbo;
            this.MyHabbo = MyHabbo;
        }

        public override void Compose(Composer packet)
        {
            string ColorName = "00000";
            if (TargetHabbo.Rank == 18)
            {
                ColorName = "6A0888";
            }

            if (TargetHabbo.Rank == 17)
            {
                ColorName = "DF0101";
            }
            else if (TargetHabbo.Rank == 16)
            {
                ColorName = "C53100";
            }
            else if (TargetHabbo.Rank == 15)
            {
                ColorName = "00A075";
            }
            else if (TargetHabbo.Rank == 12)
            {
                ColorName = "D49F00";
            }

            packet.WriteInteger(TargetHabbo.Id);
            packet.WriteString("<font color='#" + ColorName + "'>" + TargetHabbo.Username + "</font>");
            packet.WriteString(TargetHabbo.Look);
            packet.WriteString(TargetHabbo.Motto);
            packet.WriteString(new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(TargetHabbo.AccountCreated).ToString("dd/MM/yyyy"));
            packet.WriteInteger(TargetHabbo.GetStats().AchievementPoints);
            packet.WriteInteger(MyHabbo.GetMessenger().GetFriendsCount(TargetHabbo.Id));
            packet.WriteBoolean(TargetHabbo.Id != MyHabbo.Id && MyHabbo.GetMessenger().FriendshipExists(TargetHabbo.Id));  //Friend
            packet.WriteBoolean(TargetHabbo.Id != MyHabbo.Id && !MyHabbo.GetMessenger().FriendshipExists(TargetHabbo.Id) && MyHabbo.GetMessenger().RequestExists(TargetHabbo.Id));  //Friend Request Send
            packet.WriteBoolean((StarBlueServer.GetGame().GetClientManager().GetClientByUserID(TargetHabbo.Id)) != null);

            List<Group> Groups = StarBlueServer.GetGame().GetGroupManager().GetGroupsForUser(TargetHabbo.Id);
            packet.WriteInteger(Groups.Count);
            foreach (Group Group in Groups)
            {
                packet.WriteInteger(Group.Id);
                packet.WriteString(Group.Name);
                packet.WriteString(Group.Badge);
                packet.WriteString(StarBlueServer.GetGame().GetGroupManager().GetGroupColour(Group.Colour1, true));
                packet.WriteString(StarBlueServer.GetGame().GetGroupManager().GetGroupColour(Group.Colour2, false));
                packet.WriteBoolean(TargetHabbo.GetStats().FavouriteGroupId == Group.Id);
                packet.WriteInteger(Group.CreatorId);
                packet.WriteBoolean(Group != null ? Group.ForumEnabled : true);
            }

            packet.WriteInteger(Convert.ToInt32(StarBlueServer.GetUnixTimestamp() - TargetHabbo.LastOnline));
            packet.WriteBoolean(true);
        }
    }
}