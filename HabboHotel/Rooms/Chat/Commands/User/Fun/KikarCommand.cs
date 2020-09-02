using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    internal class KikarCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";
        public string Parameters => "";
        public string Description => "Kikar.";
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            Room.SendMessage(new ShoutComposer(User.VirtualId, "Ai, como eu gosto de kikar, rs...", 0, 13));
            User.ApplyEffect(502);
            if (!User.Statusses.ContainsKey("sit"))
            {
                User.Statusses.Add("sit", "1.0");
                User.Z -= 0.35;
                User.isSitting = true;
                User.UpdateNeeded = true;
            }
        }
    }
}
