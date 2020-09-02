using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Communication.Packets.Outgoing.WebSocket;
using StarBlue.HabboHotel.GameClients;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator
{
    internal class HALCommand : IChatCommand
    {
        public string PermissionRequired => "user_15";
        public string Parameters => "[URL] [MENSAGEM]";
        public string Description => "Mandar mensagem ao hotel com Link.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 2)
            {
                Session.SendWhisper("Escreva a mensagem e o link para enviar.");
                return;
            }

            string URL = Params[1].Replace("https://", "").Replace("http://", "");
            string Message = CommandManager.MergeParams(Params, 2);
            foreach (GameClient Client in StarBlueServer.GetGame().GetClientManager().GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                {
                    continue;
                }

                if (!Client.GetHabbo().SendWebPacket(new HotelAlertComposer(Session.GetHabbo().Username, Session.GetHabbo().Look, Message, URL)))
                {
                    Client.SendMessage(new RoomNotificationComposer("Heibbo Hotel", Message + "\r\n" + "- " + Session.GetHabbo().Username, "", "Clique aqui", "https://" + URL));
                }
            }

            return;
        }
    }
}
