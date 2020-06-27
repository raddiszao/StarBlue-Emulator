using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using System.Collections.Generic;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class RoomMuteCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";
        public string Parameters => "[MENSAGEM]";
        public string Description => "Mutar quarto.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.RoomMuted)
            {
                Room.RoomMuted = true;
            }

            List<RoomUser> RoomUsers = Room.GetRoomUserManager().GetRoomUsers();
            if (RoomUsers.Count > 0)
            {
                foreach (RoomUser User in RoomUsers)
                {
                    if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                    {
                        continue;
                    }

                    User.GetClient().SendMessage(new RoomCustomizedAlertComposer("Este quarto foi silenciado."));
                }
            }
        }
    }
}
