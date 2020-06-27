namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class StackCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return "[VALOR]"; }
        }

        public string Description
        {
            get { return "Altera a altura do seu quarto."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Insira um valor para a altura do seu quarto.", 34);
                return;
            }

            if (!double.TryParse(Params[1], out double newStack))
            {
                Session.SendWhisper("Valor inválido, insira um valor númerico.", 34);
                return;
            }

            if (newStack > 1000.00)
            {
                Session.SendWhisper("Insira um valor menor.", 34);
                return;
            }

            Session.SendWhisper("Sucesso, a altura do quarto foi alterada para " + newStack + ".", 34);
            Session.GetHabbo().StackHeight = newStack;
        }
    }
}
