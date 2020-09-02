using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class EndPollCommand : IChatCommand
    {

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            Room.endQuestion();
        }

        public string Description =>
            "Terminar enquete.";

        public string Parameters =>
            "";

        public string PermissionRequired =>
            "user_12";
    }
}