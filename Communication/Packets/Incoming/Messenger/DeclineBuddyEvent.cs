namespace StarBlue.Communication.Packets.Incoming.Messenger
{
    internal class DeclineBuddyEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            bool DeclineAll = Packet.PopBoolean();
            int Amount = Packet.PopInt();

            if (!DeclineAll)
            {
                int RequestId = Packet.PopInt();
                Session.GetHabbo().GetMessenger().HandleRequest(RequestId);
            }
            else
            {
                Session.GetHabbo().GetMessenger().HandleAllRequests();
            }
        }
    }
}
