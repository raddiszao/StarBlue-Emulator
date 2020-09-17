using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Groups;
using StarBlue.HabboHotel.Users.Messenger;
using StarBlue.HabboHotel.Users.Relationships;
using System;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    internal class FriendListUpdateComposer : MessageComposer
    {
        public int FriendId { get; }

        public GameClient Session { get; }
        public MessengerBuddy Buddy { get; }

        public Group Group { get; }

        public int State { get; }
        public bool GroupUpdateState { get; }
        public bool UpdateFriend { get; }
        public bool SerializeBuddy { get; }

        public override void Compose(Composer packet)
        {
            if (Group != null && !GroupUpdateState)
            {
                packet.WriteInteger(1);//Category Count
                packet.WriteInteger(1);
                packet.WriteString("Grupos");
                packet.WriteInteger(1);//Updates Count
                packet.WriteInteger(-1);//Update
                packet.WriteInteger(Group.Id);
            }
            else if (Group != null && GroupUpdateState)
            {
                packet.WriteInteger(1);//Category Count
                packet.WriteInteger(1);
                packet.WriteString("Grupos");
                packet.WriteInteger(1);//Updates Count

                packet.WriteInteger(State);//Update
                packet.WriteInteger(-Group.Id);
                packet.WriteString(Group.Name);
                packet.WriteInteger(0);

                packet.WriteBoolean(true);
                packet.WriteBoolean(false);

                packet.WriteString(Group.Badge);//Habbo.IsOnline ? Habbo.Look : "");
                packet.WriteInteger(1); // categoryid
                packet.WriteString("");
                packet.WriteString(string.Empty); // Facebook username
                packet.WriteString(string.Empty);
                packet.WriteBoolean(true); // Allows offline messaging
                packet.WriteBoolean(false); // ?
                packet.WriteBoolean(false); // Uses phone
                packet.WriteShort(0);
            }
            else if (Session != null && Buddy != null && !SerializeBuddy)
            {
                packet.WriteInteger(1);//Category Count
                packet.WriteInteger(1);
                packet.WriteString("Grupos");
                packet.WriteInteger(1);//Updates Count
                packet.WriteInteger(0);//Update

                Relationship Relationship = Session.GetHabbo().Relationships.FirstOrDefault(x => x.Value.UserId == Convert.ToInt32(Buddy.UserId)).Value;
                int y = Relationship == null ? 0 : Relationship.Type;

                packet.WriteInteger(Buddy.UserId);
                packet.WriteString(Buddy.mUsername);
                packet.WriteInteger(1);
                if (!Buddy.mAppearOffline || Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                {
                    packet.WriteBoolean(Buddy.IsOnline);
                }
                else
                {
                    packet.WriteBoolean(false);
                }

                if (!Buddy.mHideInroom || Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                {
                    packet.WriteBoolean(Buddy.InRoom);
                }
                else
                {
                    packet.WriteBoolean(false);
                }

                packet.WriteString("");//Habbo.IsOnline ? Habbo.Look : "");
                packet.WriteInteger(0); // categoryid
                packet.WriteString(string.Empty);
                packet.WriteString(string.Empty); // Facebook username
                packet.WriteString(string.Empty);
                packet.WriteBoolean(true); // Allows offline messaging
                packet.WriteBoolean(false); // ?
                packet.WriteBoolean(false); // Uses phone
                packet.WriteShort(y);
            }
            else if (UpdateFriend)
            {
                packet.WriteInteger(1);//Category Count
                packet.WriteInteger(1);
                packet.WriteString("Grupos");
                packet.WriteInteger(1);//Updates Count
                packet.WriteInteger(-1);//Update
                packet.WriteInteger(FriendId);
            }
            else
            {
                packet.WriteInteger(1); // category count
                packet.WriteInteger(1);
                packet.WriteString("Grupos");
                packet.WriteInteger(1); // number of updates
                packet.WriteInteger(0); // don't know
                if (Buddy != null && Session != null && SerializeBuddy)
                {
                    Buddy.Serialize(packet, Session);
                }
            }
        }

        public FriendListUpdateComposer(Group Group)
            : base(Composers.FriendListUpdateMessageComposer)
        {
            this.Group = Group;
        }

        public FriendListUpdateComposer(int FriendId)
            : base(Composers.FriendListUpdateMessageComposer)
        {
            this.FriendId = FriendId;
            this.UpdateFriend = true;
        }

        public FriendListUpdateComposer(Group Group, int State)
           : base(Composers.FriendListUpdateMessageComposer)
        {
            this.Group = Group;
            this.State = State;
            this.GroupUpdateState = true;
        }

        public FriendListUpdateComposer(GameClient Session, MessengerBuddy Buddy)
            : base(Composers.FriendListUpdateMessageComposer)
        {
            this.Session = Session;
            this.Buddy = Buddy;
        }

        public FriendListUpdateComposer(GameClient Session, MessengerBuddy Buddy, bool Serialize)
            : base(Composers.FriendListUpdateMessageComposer)
        {
            this.Session = Session;
            this.Buddy = Buddy;
            this.SerializeBuddy = Serialize;
        }
    }
}