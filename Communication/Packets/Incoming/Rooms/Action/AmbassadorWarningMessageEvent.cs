using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Action
{
    internal class AmbassadorWarningMessageEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            int UserId = Packet.PopInt();
            Room Room = Session.GetHabbo().CurrentRoom;
            RoomUser Target = Room.GetRoomUserManager().GetRoomUserByHabbo(StarBlueServer.GetUsernameById(UserId));
            if (Target == null)
            {
                return;
            }

            long nowTime = StarBlueServer.CurrentTimeMillis();
            long timeBetween = nowTime - Session.GetHabbo()._lastTimeUsedHelpCommand;
            if (timeBetween < 60000)
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("abuse", "Espere 1 minuto para enviar um novo alerta.", ""));
                return;
            }

            else
            {
                StarBlueServer.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("advice", "" + Session.GetHabbo().Username + " acaba de mandar um alerta para " + Target.GetClient().GetHabbo().Username + ", clique aqui para seguir.", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
            }

            Target.GetClient().SendMessage(new BroadcastMessageAlertComposer("<b><font size='15px' color='#c40101'>Mensagem dos Embaixadores<br></font></b>A equipe detectou seu comportamento fora do normal, se continuar será punido."));

            Session.GetHabbo()._lastTimeUsedHelpCommand = nowTime;
        }
    }
}
