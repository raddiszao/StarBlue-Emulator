using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    internal class SelfieCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Tirar uma selfie";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
            {
                return;
            }

            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "*Pego meu iPhone*", 0, ThisUser.LastBubble));
            Session.GetHabbo().Effects().ApplyEffect(65);
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "*E tiro uma Selfie*", 0, ThisUser.LastBubble));

        }
    }
}
