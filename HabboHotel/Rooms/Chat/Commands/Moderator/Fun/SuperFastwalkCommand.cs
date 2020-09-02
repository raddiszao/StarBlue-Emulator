namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    internal class SuperFastwalkCommand : IChatCommand
    {
        public string PermissionRequired => "user_7";
        public string Parameters => "";
        public string Description => "Dá-lhe a capacidade de andar muito rápido.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            User.SuperFastWalking = !User.SuperFastWalking;

            if (User.FastWalking)
            {
                User.FastWalking = false;
            }

            if (User.SuperFastWalking)
            {
                Session.SendWhisper("Andar Super Rápido Ativado!");
            }
            else
            {
                Session.SendWhisper("Andar Super Rápido Desativado!");
            }
        }
    }
}
