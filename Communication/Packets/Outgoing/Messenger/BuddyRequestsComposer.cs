using StarBlue.HabboHotel.Cache;
using StarBlue.HabboHotel.Users.Messenger;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    internal class BuddyRequestsComposer : MessageComposer
    {
        public ICollection<MessengerRequest> Requests { get; }

        public BuddyRequestsComposer(ICollection<MessengerRequest> requests)
            : base(Composers.BuddyRequestsMessageComposer)
        {
            this.Requests = requests;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Requests.Count);
            packet.WriteInteger(Requests.Count);

            foreach (MessengerRequest Request in Requests)
            {
                packet.WriteInteger(Request.From);
                packet.WriteString(Request.Username);

                UserCache User = StarBlueServer.GetGame().GetCacheManager().GenerateUser(Request.From);
                packet.WriteString(User != null ? User.Look : "");
            }
        }
    }
}
