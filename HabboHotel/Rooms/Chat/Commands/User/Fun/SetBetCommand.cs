using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class SetBetCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "[DIAMANTES]";

        public string Description => "Defina quantos diamantes você deseja apostar nos slots.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            if (Params.Length == 1)
            {
                Session.SendWhisper("Você deve inserir um valor em diamantes, por exemplo :apostar 50.", 34);
                return;
            }

            if (!int.TryParse(Params[1].ToString(), out int Bet))
            {
                Session.SendWhisper("Por favor insira um número válido.", 34);
                return;
            }

            if (Bet > 150)
            {
                Session.SendWhisper("A aposta máxima é de 150 diamantes.", 34);
                return;
            }

            Session.GetHabbo()._bet = Bet;
            Session.SendWhisper("Você definiu suas apostas para " + Bet + " diamantes. Aposte com cabeça!", 34);
        }
    }
}