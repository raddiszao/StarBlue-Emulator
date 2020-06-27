using StarBlue.HabboHotel.Cache;
using StarBlue.HabboHotel.Users.Messenger;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    class BuddyRequestsComposer : ServerPacket
    {
        public BuddyRequestsComposer(ICollection<MessengerRequest> Requests)
            : base(ServerPacketHeader.BuddyRequestsMessageComposer)
        {
            base.WriteInteger(Requests.Count);
            base.WriteInteger(Requests.Count);

            foreach (MessengerRequest Request in Requests)
            {
                base.WriteInteger(Request.From);
                base.WriteString(Request.Username);

                UserCache User = StarBlueServer.GetGame().GetCacheManager().GenerateUser(Request.From);
                base.WriteString(User != null ? User.Look : "");
            }
        }
    }
}
