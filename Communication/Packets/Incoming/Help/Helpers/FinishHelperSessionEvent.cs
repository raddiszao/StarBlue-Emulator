using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Helpers;

namespace StarBlue.Communication.Packets.Incoming.Help.Helpers
{
    class FinishHelperSessionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var Voted = Packet.PopBoolean();
            var Element = HelperToolsManager.GetElement(Session);
            if (Element is HelperCase)
            {
                if (Voted)
                {
                    Element.OtherElement.Session.SendMessage(RoomNotificationComposer.SendBubble("ambassador", "" + Element.OtherElement.Session.GetHabbo().Username + ", gracias por colaborar en el programa de Alfas, has atendido correctamente la duda del usuario.", ""));
                    //if (Element.OtherElement.Session.GetHabbo()._guidelevel >= 1)
                    //{
                    //    StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Element.OtherElement.Session, "ACH_GuideTourGiver", 1);
                    //}
                }
                else
                {
                    Element.OtherElement.Session.SendMessage(RoomNotificationComposer.SendBubble("ambassador", "" + Element.OtherElement.Session.GetHabbo().Username + ", gracias por colaborar en el programa de Alfas, has atendido satisfactoriamente la duda del usuario.", ""));
                }
            }

            Element.Close();
        }
    }
}
