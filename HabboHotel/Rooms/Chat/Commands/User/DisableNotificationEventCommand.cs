namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableNotificationEventCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Desative os alertas de eventos.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().DisabledNotificationEvents = !Session.GetHabbo().DisabledNotificationEvents;

            if (!Session.GetHabbo().DisabledNotificationEvents)
            {
                Session.SendWhisper("Você ativou os alertas de eventos.", 34);
            }
            else
            {
                Session.SendWhisper("Você desativou os alertas de eventos.", 34);
            }
        }
    }
}