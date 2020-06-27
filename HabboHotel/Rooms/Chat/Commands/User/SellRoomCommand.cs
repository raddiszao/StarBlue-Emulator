using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class SellRoomCommand : IChatCommand
    {
        public string Description
        {
            get { return "Coloca a sala em que você está para venda."; }
        }

        public string Parameters
        {
            get { return "%preço%"; }
        }

        public string PermissionRequired
        {
            get { return "user_normal"; }
        }


        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Deve colocar um preço.");
                return;
            }

            if (!Room.CheckRights(Session, true))
            {
                return;
            }

            if (Room == null)
            {
                if (Params.Length == 1)
                {
                    Session.SendWhisper("Oops, você deve escolher um preço para vender este quarto.");
                    return;
                }
                else if (Room.Group != null)
                {
                    Session.SendWhisper("Oops, ao que parece está sala tem um grupo, para poder vender, delete o grupo.");
                    return;
                }
            }

            if (!int.TryParse(Params[1], out int Value))
            {
                Session.SendWhisper("Oops, coloque um valor valido");
                return;
            }

            if (Value < 0)
            {
                Session.SendWhisper("Não é possivel vender a sala com um valor negativo.");
                return;
            }

            if (Room.ForSale)
            {
                Room.SalePrice = Value;
            }
            else
            {
                Room.ForSale = true;
                Room.SalePrice = Value;
            }

            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers())
            {
                if (User == null || User.GetClient() == null)
                {
                    continue;
                }

                User.GetClient().SendWhisper("Está sala esta a venda, seu preço atual é de :  " + Value + " Duckets! Compre-a esrevendo :comprarquarto.");
            }

            Session.SendNotification("Se você quer vender uma sala, deve colocar um valor numérico. \n\nPOR FAVOR NOTA:\nSe você vender um quarto, não poderá recupera-lo.!\n\n" +
        "Você pode cancelar uma venda escrevendo ':unload' (sem as '')");

            return;
        }
    }
}
