using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class CustomizedHotelAlert : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_15"; }
        }

        public string Parameters
        {
            get { return "[MENSAGEM]"; }
        }

        public string Description
        {
            get { return "Envie uma mensagem para todo o Hotel"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, escreva a mensagem para enviar.");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            StarBlueServer.GetGame().GetClientManager().SendMessage(new RoomCustomizedAlertComposer("\n" + Message + "\n\n - " + Session.GetHabbo().Username + ""));
            return;
        }
    }
}
