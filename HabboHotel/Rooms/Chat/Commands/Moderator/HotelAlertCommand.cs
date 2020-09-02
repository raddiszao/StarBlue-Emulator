using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Communication.Packets.Outgoing.WebSocket;
using StarBlue.HabboHotel.GameClients;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class HotelAlertCommand : IChatCommand
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

            foreach (GameClient Client in StarBlueServer.GetGame().GetClientManager().GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                {
                    continue;
                }

                if (!Client.GetHabbo().SendWebPacket(new HotelAlertComposer(Session.GetHabbo().Username, Session.GetHabbo().Look, Message, "")))
                {
                    Client.SendMessage(new BroadcastMessageAlertComposer(Message + "\r\n" + "- " + Session.GetHabbo().Username));
                }
            }

            return;
        }
    }
}
