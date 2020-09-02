using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    internal class LinkCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";
        public string Parameters => "[link]";
        public string Description => "Envia um link.";
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oops, você deve digitar um link.", 34);
                return;
            }

            string link = Params[1];

            if (link.Contains("http") || link.Contains("https"))
            {
                string Username;
                RoomUser TargetUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
                Username = Session.GetHabbo().Username + " enviou um link, <b><u><a href='" + link + "' target='_blank'>clique aqui para acessá-lo</a></u></b>";
                if (Room != null)
                {
                    Room.SendMessage(new UserNameChangeComposer(Session.GetHabbo().CurrentRoomId, TargetUser.VirtualId, Username));
                }

                string Message = " ";
                Room.SendMessage(new ChatComposer(TargetUser.VirtualId, Message, 0, 34));
                TargetUser.SendNamePacket();
            }
            else
            {
                Session.SendWhisper("Oops, acho que este link é inválido.", 34);
            }
        }
    }
}
