using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class RoomKickCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";
        public string Parameters => "[MENSAGEM]";
        public string Description => "Expulsar todos os usuários do quarto.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, dê um motivo para os usuários.");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            foreach (RoomUser RoomUser in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (RoomUser == null || RoomUser.IsBot || RoomUser.GetClient() == null || RoomUser.GetClient().GetHabbo() == null || (RoomUser.GetClient().GetHabbo().GetPermissions().HasRight("mod_tool") && Session.GetHabbo().Rank < RoomUser.GetClient().GetHabbo().Rank) || RoomUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)
                {
                    continue;
                }

                if (Message.Equals(""))
                    RoomUser.GetClient().SendNotification("Você foi expulso por um moderador.");
                else
                    RoomUser.GetClient().SendNotification("Você foi expulso por um moderador pelo seguinte motivo: " + Message);

                Room.GetRoomUserManager().RemoveUserFromRoom(RoomUser.GetClient(), true, false);
            }

            Session.SendWhisper("Expulsou todos do quarto corretamente.", 34);
        }
    }
}
