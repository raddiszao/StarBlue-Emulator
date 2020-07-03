namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableMentionCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Desative as menções.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().DisabledMentions = !Session.GetHabbo().DisabledMentions;

            if (!Session.GetHabbo().DisabledMentions)
            {
                Session.SendWhisper("Agora todo mundo pode mencionar você.", 34);
            }
            else
            {
                Session.SendWhisper("Você desativou as menções.", 34);
            }
        }
    }
}