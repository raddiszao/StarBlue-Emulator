using StarBlue.HabboHotel.Users.Messenger;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    internal class HabboSearchResultComposer : MessageComposer
    {
        public List<SearchResult> Friends { get; }
        public List<SearchResult> OtherUsers { get; }

        public HabboSearchResultComposer(List<SearchResult> Friends, List<SearchResult> OtherUsers)
            : base(Composers.HabboSearchResultMessageComposer)
        {
            this.Friends = Friends;
            this.OtherUsers = OtherUsers;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Friends.Count);
            foreach (SearchResult Friend in Friends.ToList())
            {
                bool Online = (StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Friend.UserId) != null);

                packet.WriteInteger(Friend.UserId);
                packet.WriteString(Friend.Username);
                packet.WriteString(Friend.Motto);
                packet.WriteBoolean(Online);
                packet.WriteBoolean(false);
                packet.WriteString(string.Empty);
                packet.WriteInteger(0);
                packet.WriteString(Online ? Friend.Figure : "");
                packet.WriteString(Friend.LastOnline);
            }

            packet.WriteInteger(OtherUsers.Count);
            foreach (SearchResult OtherUser in OtherUsers.ToList())
            {
                bool Online = (StarBlueServer.GetGame().GetClientManager().GetClientByUserID(OtherUser.UserId) != null);

                packet.WriteInteger(OtherUser.UserId);
                packet.WriteString(OtherUser.Username);
                packet.WriteString(OtherUser.Motto);
                packet.WriteBoolean(Online);
                packet.WriteBoolean(false);
                packet.WriteString(string.Empty);
                packet.WriteInteger(0);
                packet.WriteString(Online ? OtherUser.Figure : "");
                packet.WriteString(OtherUser.LastOnline);
            }
        }
    }
}
