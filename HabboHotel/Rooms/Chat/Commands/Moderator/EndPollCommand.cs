using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class EndPollCommand : IChatCommand
    {

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, false, true))
            {
                Session.SendWhisper("Somente pessoas com direitos ou donos podem usar este comando.", 34);
                return;
            }

            Room.endQuestion();
        }

        public string Description =>
            "Terminar enquete.";

        public string Parameters =>
            "";

        public string PermissionRequired =>
            "user_normal";
    }
}