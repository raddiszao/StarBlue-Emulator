using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Core;
using System;
using System.Threading.Tasks;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class ShutdownCommand : IChatCommand
    {
        public string PermissionRequired => "user_18";
        public string Parameters => "";
        public string Description => "Fechar o hotel!";
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            int total_time = 1 * 60 * 1000;
            Logging.WriteLine("O servidor irá fechar em 1 minuto.", ConsoleColor.Yellow);
            StarBlueServer.GetGame().GetClientManager().SendMessage(new BroadcastMessageAlertComposer("O " + StarBlueServer.HotelName + " Hotel irá reinicializar em 1 minuto.\r\n" + "- Equipe " + StarBlueServer.HotelName));
            StarBlueServer.GetConfig().data["going.is.to.be.close"] = "true";
            Task t = Task.Factory.StartNew(() => ConsoleCommandHandler.ShutdownIn(total_time));
        }
    }
}