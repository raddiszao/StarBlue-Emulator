using StarBlue.Communication.Packets.Outgoing.Handshake;
using StarBlue.HabboHotel.Users;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class FlagMeCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_12"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Troca seu nome de usuário."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!CanChangeName(Session.GetHabbo()))
            {
                Session.SendWhisper("Lo sentimos, parece que actualmente no tienen la opción de cambiar su nombre de usuario, ESPERE UN POCO MAS DE TIEMPO.");
                return;
            }

            Session.GetHabbo().ChangingName = true;
            Session.SendNotification("Tenga en cuenta que si su nombre es prohibido, se le negara el acceso a su usario.\r\rTenga en cuenta que no volvera a cambiar su nombre en caso de tener problemas con el que haya elegido\r\rCierre esta ventana y haga clic en si mismo para empezar a cambiar el nombre!");
            Session.SendMessage(new UserObjectComposer(Session.GetHabbo()));
        }

        private bool CanChangeName(Habbo Habbo)
        {
            if (Habbo.Rank == 1 && Habbo.VIPRank == 0 && Habbo.LastNameChange == 0)
            {
                return true;
            }
            else if (Habbo.Rank == 2 && Habbo.VIPRank == 1 && (Habbo.LastNameChange == 0 || (StarBlueServer.GetUnixTimestamp() + 604800) > Habbo.LastNameChange))
            {
                return true;
            }
            else if (Habbo.Rank == 1 && Habbo.VIPRank == 2 && (Habbo.LastNameChange == 0 || (StarBlueServer.GetUnixTimestamp() + 86400) > Habbo.LastNameChange))
            {
                return true;
            }
            else if (Habbo.Rank == 1 && Habbo.VIPRank == 3)
            {
                return true;
            }
            else if (Habbo.GetPermissions().HasRight("mod_tool"))
            {
                return true;
            }

            return false;
        }
    }
}
