namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class FastwalkCommand : IChatCommand
    {
        public string PermissionRequired => "user_vip";

        public string Parameters => "";

        public string Description => "Obter a habilidade de andar rápido";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            User.FastWalking = !User.FastWalking;

            if (User.SuperFastWalking)
            {
                User.SuperFastWalking = false;
            }

            Session.SendWhisper("Caminhar rápido Act.", 34);
        }
    }
}
