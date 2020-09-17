using StarBlue.HabboHotel.Users;
using StarBlue.HabboHotel.Users.Messenger;
using StarBlue.HabboHotel.Users.Relationships;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    internal class BuddyListComposer : MessageComposer
    {
        public ICollection<MessengerBuddy> Friends { get; }
        public Habbo Player { get; }
        public int Pages { get; }
        public int Page { get; }

        public BuddyListComposer(ICollection<MessengerBuddy> friends, Habbo player, int pages = 1, int page = 0)
            : base(Composers.BuddyListMessageComposer)
        {
            this.Friends = friends;
            this.Pages = pages;
            this.Player = player;
            this.Page = page;
        }

        public override void Compose(Composer packet)
        {
            int friendCount = Friends.Count;
            if (Player._guidelevel >= 1)
            {
                friendCount++;
            }

            if (Player.Rank >= 5)
            {
                friendCount++;
            }

            packet.WriteInteger(this.Pages);
            packet.WriteInteger(this.Page);
            List<HabboHotel.Groups.Group> groups = StarBlueServer.GetGame().GetGroupManager().GetGroupsForUser(Player.Id).Where(c => c.HasChat).ToList();
            packet.WriteInteger(friendCount + groups.Count);

            foreach (HabboHotel.Groups.Group gp in groups.ToList())
            {
                packet.WriteInteger(int.MinValue + gp.Id);
                packet.WriteString(gp.Name);
                packet.WriteInteger(1);//Gender.
                packet.WriteBoolean(true);
                packet.WriteBoolean(false);
                packet.WriteString(gp.Badge);
                packet.WriteInteger(1); // category id
                packet.WriteString(string.Empty);
                packet.WriteString("Chat de Grupo");//Alternative name?
                packet.WriteString(string.Empty);
                packet.WriteBoolean(true);
                packet.WriteBoolean(false);
                packet.WriteBoolean(false);//Pocket Habbo user.
                packet.WriteShort(0);
                Player.GetClient().SendMessage(new FriendListUpdateComposer(gp));
            }

            foreach (MessengerBuddy Friend in Friends.ToList())
            {
                Relationship Relationship = Player.Relationships.FirstOrDefault(x => x.Value.UserId == Convert.ToInt32(Friend.UserId)).Value;

                packet.WriteInteger(Friend.Id);
                packet.WriteString(Friend.mUsername);
                packet.WriteInteger(1);//Gender.
                packet.WriteBoolean(Friend.IsOnline);
                packet.WriteBoolean(Friend.IsOnline && Friend.InRoom);
                packet.WriteString(Friend.IsOnline ? Friend.mLook : string.Empty);
                packet.WriteInteger(0); // category id
                packet.WriteString(string.Empty);
                packet.WriteString(string.Empty);//Alternative name?
                packet.WriteString(string.Empty);
                packet.WriteBoolean(true);
                packet.WriteBoolean(false);
                packet.WriteBoolean(false);//Pocket Habbo user.
                packet.WriteShort(Relationship == null ? 0 : Relationship.Type);
            }

            #region Custom Chats
            if (Player.Rank >= 5)
            {
                packet.WriteInteger(int.MinValue);
                packet.WriteString("Chat Staff");
                packet.WriteInteger(1);
                packet.WriteBoolean(true);
                packet.WriteBoolean(false);
                packet.WriteString("chatstaff");
                packet.WriteInteger(1);
                packet.WriteString(string.Empty);
                packet.WriteString("Chat Staff");
                packet.WriteString(string.Empty);
                packet.WriteBoolean(true);
                packet.WriteBoolean(false);
                packet.WriteBoolean(false);
                packet.WriteShort(0);
            }
            #endregion

        }
    }
}
