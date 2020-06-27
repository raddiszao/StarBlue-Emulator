using StarBlue.Communication.Packets.Outgoing.Moderation;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class HotelAlertCommand : IChatCommand
    {
        public string PermissionRequired => "user_15";
        public string Parameters => "[MENSAGEM]";
        public string Description => "Enviar alerta para o hotel.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, escreva a mensagem para enviar", 34);
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);

            if (StarBlueServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Message, out string MessageFiltered))
            {
                Session.Disconnect();
                return;
            }
            StarBlueServer.GetGame().GetClientManager().SendMessage(new BroadcastMessageAlertComposer(Message + "\r\n" + "- " + Session.GetHabbo().Username));

            return;
        }
    }
}
