using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Helpers;

namespace StarBlue.Communication.Packets.Incoming.Help.Helpers
{
    internal class FinishHelperSessionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            bool Voted = Packet.PopBoolean();
            IHelperElement Element = HelperToolsManager.GetElement(Session);
            if (Element is HelperCase)
            {
                if (Voted)
                {
                    Element.OtherElement.Session.SendMessage(RoomNotificationComposer.SendBubble("ambassador", "" + Element.OtherElement.Session.GetHabbo().Username + ", obrigado por usar a ferramenta de embaixador.", ""));
                    //if (Element.OtherElement.Session.GetHabbo()._guidelevel >= 1)
                    //{
                    //    StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Element.OtherElement.Session, "ACH_GuideTourGiver", 1);
                    //}
                }
                else
                {
                    Element.OtherElement.Session.SendMessage(RoomNotificationComposer.SendBubble("ambassador", "" + Element.OtherElement.Session.GetHabbo().Username + ", obrigado por usar a ferramenta de embaixador.", ""));
                }
            }

            Element.Close();
        }
    }
}
