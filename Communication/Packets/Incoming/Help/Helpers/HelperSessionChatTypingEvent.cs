using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Help.Helpers
{
    class HelperSessionChatTypingEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var element = HabboHotel.Helpers.HelperToolsManager.GetElement(Session);
            if (element != null && element.OtherElement != null)
            {
                element.OtherElement.Session.SendMessage(new Outgoing.Help.Helpers.HelperSessionChatIsTypingComposer(Packet.PopBoolean()));
            }
        }
    }
}
