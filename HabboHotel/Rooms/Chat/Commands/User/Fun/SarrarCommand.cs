using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    internal class SarrarCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";
        public string Parameters => "";
        public string Description => "Sarrar.";
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            Room.SendMessage(new ShoutComposer(User.VirtualId, "Sarrei!", 0, 13));
            User.ApplyEffect(507);
        }
    }
}
