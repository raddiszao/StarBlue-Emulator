using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    internal class BuilderCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Habilita o teletransporte no seu quarto para construir mais facilmente.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, false, true))
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            User.TeleportEnabled = !User.TeleportEnabled;
            Room.GetGameMap().GenerateMaps();

            Session.SendMessage(RoomNotificationComposer.SendBubble("builders_club_room_locked_small", "Você ativou o modo de construtor.", ""));
        }
    }
}
