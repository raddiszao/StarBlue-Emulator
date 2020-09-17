using StarBlue.Core;
using StarBlue.Utilities;
using System;

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
            if (int.Parse(time) > 0)
            {
                Threading threading = new Threading();
                threading.SetMinutes(int.Parse(time));
                threading.SetAction(() => ConsoleCommandHandler.ShutdownIn());
                threading.Start();
            }
            else
            {
                ConsoleCommandHandler.ShutdownIn();
            }
        }
    }
}