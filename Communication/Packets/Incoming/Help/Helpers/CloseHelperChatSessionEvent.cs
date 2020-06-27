using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Helpers;

namespace StarBlue.Communication.Packets.Incoming.Help.Helpers
{
    class CloseHelperChatSessionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var Element = HelperToolsManager.GetElement(Session);

            if (Element != null)
            {
                Element.End();
                if (Element.OtherElement != null)
                {
                    Element.OtherElement.End();
                }
            }

            if (Session.GetHabbo().OnHelperDuty)
            {
                StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_GuideTourGiver", 1);
            }
        }
    }
}
