using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class HALCommand : IChatCommand
    {
        public string PermissionRequired => "user_13";
        public string Parameters => "[URL] [MENSAGEM]";
        public string Description => "Mandar mensagem ao hotel com Link.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 2)
            {
                Session.SendWhisper("Escreva a mensagem e o link para enviar.");
                return;
            }

            string URL = Params[1];
            string Message = CommandManager.MergeParams(Params, 2);
            StarBlueServer.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("Heibbo Hotel", Message + "\r\n" + "- " + Session.GetHabbo().Username, "", "Clique aqui", "https://" + URL));
            return;
        }
    }
}
