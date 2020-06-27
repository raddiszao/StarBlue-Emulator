using StarBlue.HabboHotel.GameClients;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class DisconnectAllCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_18"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Reinicia (deconecta todos)."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            foreach (GameClient Client in StarBlueServer.GetGame().GetClientManager().GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().Username == Session.GetHabbo().Username)
                {
                    continue;
                }

                if (!Client.GetHabbo().InRoom)
                {
                    Client.GetConnection().Dispose();
                }
                else if (Client.GetHabbo().InRoom)
                {
                    Client.GetConnection().Dispose();
                }

                Client.SendNotification("O hotel dará um pequeno reinicio, para aplicar todas as alterações dentro do Hotel. \n\nVoltaremos em seguida :)\n\n\n- " + Session.GetHabbo().Username + "");
            }



        }
    }
}
