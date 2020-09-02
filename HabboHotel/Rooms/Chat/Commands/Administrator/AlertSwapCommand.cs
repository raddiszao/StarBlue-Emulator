using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator
{
    internal class AlertSwapCommand : IChatCommand
    {
        public string PermissionRequired => "user_16";

        public string Parameters => "%type% %id%";

        public string Description => "Troca o estilo dos alertas.";

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            if (Params.Length == 1)
            {
                Session.SendMessage(new MassEventComposer("habbopages/alertexplanations.txt"));
                return;
            }

            string Type = Params[1];
            string AlertType = Params[2];

            if (Params.Length == 3)
            {
                switch (Type)
                {
                    case "staff":
                        Session.GetHabbo()._alerttype = AlertType;
                        Session.SendWhisper("Escolheu seu estilo de alerta de Staff como " + AlertType + ".", 34);
                        break;
                    case "events":
                        Session.GetHabbo()._eventtype = AlertType;
                        Session.SendWhisper("Escolheu seu estilo de alerta de Eventos como " + AlertType + ".", 34);
                        break;
                }
            }
        }
    }
}