using System.Collections.Generic;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class RoomUnmuteCommand : IChatCommand
    {
        public string PermissionRequired => "user_7";
        public string Parameters => "";
        public string Description => "Desmutar quarto.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.RoomData.RoomMuted)
            {
                Session.SendWhisper("Este quarto não está mudo.", 34);
                return;
            }

            Room.RoomData.RoomMuted = false;

            List<RoomUser> RoomUsers = Room.GetRoomUserManager().GetRoomUsers();
            if (RoomUsers.Count > 0)
            {
                foreach (RoomUser User in RoomUsers)
                {
                    if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                    {
                        continue;
                    }

                    User.GetClient().SendWhisper("Este quarto foi desmutado, você pode falar normalmente.", 34);
                }
            }
        }
    }
}