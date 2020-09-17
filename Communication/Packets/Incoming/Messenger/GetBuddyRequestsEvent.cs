using StarBlue.Communication.Packets.Outgoing.Messenger;
using StarBlue.HabboHotel.Users.Messenger;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Messenger
{
    internal class GetBuddyRequestsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            ICollection<MessengerRequest> Requests = Session.GetHabbo().GetMessenger().GetRequests().ToList();

            Session.SendMessage(new BuddyRequestsComposer(Requests));
        }
    }
}
