using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator
{
    internal class CarryCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "[ID]";

        public string Description => "Permite carregar um item em sua mão.";

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                return;
            }

            if (!Room.RoomData.HandItemEnabled && !Room.CheckRights(Session, true) && !Session.GetHabbo().GetPermissions().HasRight("room_override_custom_config"))
            {
                Session.SendWhisper("Oops, o dono do quarto desativou esta função.", 34);
                return;
            }

            if (!int.TryParse(Convert.ToString(Params[1]), out int ItemId))
            {
                Session.SendWhisper("Por favor introduza um item valido", 34);
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            User.CarryItem(ItemId);
        }
    }
}
