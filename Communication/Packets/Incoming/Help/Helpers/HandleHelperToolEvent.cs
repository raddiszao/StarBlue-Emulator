using StarBlue.Communication.Packets.Outgoing.Help.Helpers;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Helpers;

namespace StarBlue.Communication.Packets.Incoming.Help.Helpers
{
    internal class HandleHelperToolEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            if (Session.GetHabbo().Rank > 2 || Session.GetHabbo()._guidelevel > 0)
            {

                bool onDuty = Packet.PopBoolean();
                bool isGuide = Packet.PopBoolean();
                bool isHelper = Packet.PopBoolean();
                bool isGuardian = Packet.PopBoolean();
                if (onDuty)
                {
                    HelperToolsManager.AddHelper(Session, isHelper, isGuardian, isGuide);
                }
                else
                {
                    HelperToolsManager.RemoveHelper(Session);
                }

                Session.SendMessage(new HandleHelperToolComposer(onDuty));
            }
            else
            {
                Session.SendMessage(new RoomNotificationComposer("Error al cargar las herramientas alfa:", "Por razones desconocidas has tenido un error al tratar de iniciar las herramientas de alfa, contacta con un administrador.", "", "Ok", "event:close"));
            }

        }
    }
}
