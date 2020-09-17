using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Helpers;

namespace StarBlue.Communication.Packets.Incoming.Help.Helpers
{
    internal class VisitHelperUserSessionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            IHelperElement Element = HelperToolsManager.GetElement(Session);
            IHelperElement other = Element.OtherElement;
            if (other == null)
            {
                return;
            }

            if (other.Session.GetHabbo().CurrentRoom == null)
            {
                return;
            }

            Element.Session.SendMessage(new Outgoing.Help.Helpers.HelperSessionVisiteRoomComposer(other.Session.GetHabbo().CurrentRoom.Id));

        }
    }
}
