using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Groups;
using StarBlue.HabboHotel.Users;
using System;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Users
{
    internal class ProfileInformationComposer : ServerPacket
    {
        public ProfileInformationComposer(Habbo Data, GameClient Session)
            : base(ServerPacketHeader.ProfileInformationMessageComposer)
        {
            string ColorName = "00000";
            if (Data.Rank == 18)
            {
                ColorName = "6A0888";
            }

            if (Data.Rank == 17)
            {
                ColorName = "DF0101";
            }
            else if (Data.Rank == 16)
            {
                ColorName = "C53100";
            }
            else if (Data.Rank == 15)
            {
                ColorName = "00A075";
            }
            else if (Data.Rank == 12)
            {
                ColorName = "D49F00";
            }

            base.WriteInteger(Data.Id);
            base.WriteString("<font color='#" + ColorName + "'>" + Data.Username + "</font>");
            base.WriteString(Data.Look);
            base.WriteString(Data.Motto);
            base.WriteString(new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Data.AccountCreated).ToString("dd/MM/yyyy"));
            base.WriteInteger(Data.GetStats().AchievementPoints);
            base.WriteInteger(Session.GetHabbo().GetMessenger().GetFriendsCount(Data.Id));
            base.WriteBoolean(Data.Id != Session.GetHabbo().Id && Session.GetHabbo().GetMessenger().FriendshipExists(Data.Id));  //Friend
            base.WriteBoolean(Data.Id != Session.GetHabbo().Id && !Session.GetHabbo().GetMessenger().FriendshipExists(Data.Id) && Session.GetHabbo().GetMessenger().RequestExists(Data.Id));  //Friend Request Send
            base.WriteBoolean((StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Data.Id)) != null);

            List<Group> Groups = StarBlueServer.GetGame().GetGroupManager().GetGroupsForUser(Data.Id);
            base.WriteInteger(Groups.Count);
            foreach (Group Group in Groups)
            {
                base.WriteInteger(Group.Id);
                base.WriteString(Group.Name);
                base.WriteString(Group.Badge);
                base.WriteString(StarBlueServer.GetGame().GetGroupManager().GetGroupColour(Group.Colour1, true));
                base.WriteString(StarBlueServer.GetGame().GetGroupManager().GetGroupColour(Group.Colour2, false));
                base.WriteBoolean(Data.GetStats().FavouriteGroupId == Group.Id);
                base.WriteInteger(Group.CreatorId);
                base.WriteBoolean(Group != null ? Group.ForumEnabled : true);
            }

            base.WriteInteger(Convert.ToInt32(StarBlueServer.GetUnixTimestamp() - Data.LastOnline));
            base.WriteBoolean(true);
        }
    }
}