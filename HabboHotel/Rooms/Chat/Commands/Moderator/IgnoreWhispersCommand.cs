namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class IgnoreWhispersCommand : IChatCommand
    {
        public string PermissionRequired => "user_7";

        public string Parameters => "";

        public string Description => "Te permite que você ignore todos os sussurros da sala";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().IgnorePublicWhispers = !Session.GetHabbo().IgnorePublicWhispers;
            Session.SendWhisper("Você " + (Session.GetHabbo().IgnorePublicWhispers ? "agora" : "já não") + "Ignora os sussurros dos outros!", 34);
        }
    }
}
