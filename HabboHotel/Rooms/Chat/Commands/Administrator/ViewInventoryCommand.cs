using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class ViewInventoryCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_16"; }
        }

        public string Parameters
        {
            get { return "[USUARIO]"; }
        }

        public string Description
        {
            get { return "Permite ver o inventario de um usuário"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Room == null)
            {
                return;
            }

            if (Params.Length == 2)
            {
                string Username = Params[1];

                GameClient Client = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Username);
                if (Client != null)
                {
                    Session.SendWhisper("O usuario está online. Espere que ele saia para ver seu inventário.");
                    return;
                }

                int UserId = StarBlueServer.GetGame().GetClientManager().GetUserIdByUsername(Username);
                if (UserId == 0)
                {
                    Session.SendWhisper("O nome de usuario não existe.");
                    return;
                }

                Session.GetHabbo().GetInventoryComponent().LoadUserInventory(UserId);

                Session.SendWhisper("Seu inventário foi trocado pelo de " + Username);
            }
            else
            {
                Session.GetHabbo().GetInventoryComponent().LoadUserInventory(0);

                Session.SendWhisper("Seu inventário voltou ao normal.");
            }

            Session.SendWhisper("A sala foi salva na sua lista.");
        }
    }
}