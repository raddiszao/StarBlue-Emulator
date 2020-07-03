using StarBlue.Communication.Packets.Outgoing;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users.Relationships;
using System;
using System.Linq;

namespace StarBlue.HabboHotel.Users.Messenger
{
    public class MessengerBuddy
    {
        #region Fields

        public int UserId;
        public bool mAppearOffline;
        public bool mHideInroom;
        public int mLastOnline;
        public string mLook;
        public string mMotto;

        public GameClient client;
        private Room currentRoom;
        public string mUsername;

        #endregion

        #region Return values

        public int Id => UserId;

        public bool IsOnline => (client != null && client.GetHabbo() != null && client.GetHabbo().GetMessenger() != null &&
                        !client.GetHabbo().GetMessenger().AppearOffline);

        private GameClient Client
        {
            get => client;
            set => client = value;
        }

        public bool InRoom => (currentRoom != null);

        public Room CurrentRoom
        {
            get => currentRoom;
            set => currentRoom = value;
        }

        #endregion

        #region Constructor

        public MessengerBuddy(int UserId, string pUsername, string pLook, string pMotto, int pLastOnline,
                                bool pAppearOffline, bool pHideInroom)
        {
            this.UserId = UserId;
            mUsername = pUsername;
            mLook = pLook;
            mMotto = pMotto;
            mLastOnline = pLastOnline;
            mAppearOffline = pAppearOffline;
            mHideInroom = pHideInroom;
        }

        #endregion

        #region Methods
        public void UpdateUser(GameClient client)
        {
            this.client = client;
            if (client != null && client.GetHabbo() != null)
            {
                currentRoom = client.GetHabbo().CurrentRoom;
            }
        }

        public void Serialize(ServerPacket Message, GameClient Session)
        {
            Relationship Relationship = null;

            if (Session != null && Session.GetHabbo() != null && Session.GetHabbo().Relationships != null)
            {
                Relationship = Session.GetHabbo().Relationships.FirstOrDefault(x => x.Value.UserId == Convert.ToInt32(UserId)).Value;
            }

            int y = Relationship == null ? 0 : Relationship.Type;

            Message.WriteInteger(UserId);
            Message.WriteString(mUsername);
            Message.WriteInteger(1);
            Message.WriteBoolean(!mAppearOffline || Session.GetHabbo().GetPermissions().HasRight("mod_tool") ? IsOnline : false);
            Message.WriteBoolean(!mHideInroom || Session.GetHabbo().GetPermissions().HasRight("mod_tool") ? InRoom : false);
            Message.WriteString(IsOnline ? mLook : "");
            Message.WriteInteger(0); // categoryid
            Message.WriteString(string.Empty);
            Message.WriteString(mMotto); // Facebook username GRASIAS
            Message.WriteString(string.Empty);
            Message.WriteBoolean(true); // Allows offline messaging
            Message.WriteBoolean(false); // ?
            Message.WriteBoolean(false); // Uses phone
            Message.WriteShort(y);
        }

        #endregion
    }
}