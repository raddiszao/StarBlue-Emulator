using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Core;
using System;
using System.Threading.Tasks;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class ShutdownCommand : IChatCommand
    {
        public string PermissionRequired => "user_18";
        public string Parameters => "";
        public string Description => "Fechar o hotel!";
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            string time = "0";
            if (Params.Length > 1)
            {
                time = Params[1];
            }

            int total_time = int.Parse(time) * 60 * 1000;
            Logging.WriteLine("O servidor irá reiniciar em " + time + " minutos.", ConsoleColor.Yellow);
            StarBlueServer.GoingIsToBeClose = true;
            Task t = Task.Factory.StartNew(() => ConsoleCommandHandler.ShutdownIn(total_time));
        }
    }
}