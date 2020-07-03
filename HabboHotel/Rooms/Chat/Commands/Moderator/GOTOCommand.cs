namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GOTOCommand : IChatCommand
    {
        public string PermissionRequired => "user_vip";

        public string Parameters => "[ROOMID]";

        public string Description => "Ir para algum quarto";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Você deve especificar o ID do quarto", 34);
                return;
            }


            if (!int.TryParse(Params[1], out int RoomID))
            {
                Session.SendWhisper("Você deve escrever o ID do quarto corretamente", 34);
            }
            else
            {
                Room _room = StarBlueServer.GetGame().GetRoomManager().LoadRoom(RoomID);
                if (_room == null)
                {
                    Session.SendWhisper("Esse quarto não existe!", 34);
                }
                else
                {
                    Session.GetHabbo().PrepareRoom(_room.Id, "");
                }
            }
        }
    }
}