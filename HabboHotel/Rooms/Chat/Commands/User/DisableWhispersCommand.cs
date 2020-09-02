namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class DisableWhispersCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Ativa ou desativa a opção de receber sussurros.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().ReceiveWhispers = !Session.GetHabbo().ReceiveWhispers;
            Session.SendWhisper("Você " + (Session.GetHabbo().ReceiveWhispers ? "agora" : "não") + " recebe susurros!", 34);
        }
    }
}
