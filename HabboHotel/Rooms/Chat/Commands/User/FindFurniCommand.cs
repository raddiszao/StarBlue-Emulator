namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class FindFurniCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";
        public string Parameters => "";
        public string Description => "Te diz onde está o mobi no catálogo.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().FindFurniMode = true;
            Session.SendWhisper("Clique 2x em um mobi para saber onde ele é vendido.", 34);
        }
    }
}