using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class customalertCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";

        public string Parameters => "[MENSAGEM]";

        public string Description => "Envie uma mensagem de aviso para o hotel.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, escreva uma mensagem para enviar.", 34);
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);

            StarBlueServer.GetGame().GetClientManager().SendMessage(new RoomCustomizedAlertComposer(Message));



            return;
        }
    }
}
