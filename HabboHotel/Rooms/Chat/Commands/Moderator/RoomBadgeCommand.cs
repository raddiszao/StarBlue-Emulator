using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class RoomBadgeCommand : IChatCommand
    {
        public string PermissionRequired => "user_15";
        public string Parameters => "[CODIGO]";
        public string Description => "Dar emblema pra todo o quarto.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite o código do emblema que você deseja enviar neste quarto.");
                return;
            }

            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                {
                    continue;
                }

                if (!User.GetClient().GetHabbo().GetBadgeComponent().HasBadge(Params[1]))
                {
                    User.GetClient().GetHabbo().GetBadgeComponent().GiveBadge(Params[1], true, User.GetClient());
                    User.GetClient().SendMessage(RoomNotificationComposer.SendBubble("badge/" + Params[1], "Você acaba de receber um emblema!", "/inventory/open/badge"));
                }
                else
                {
                    User.GetClient().SendMessage(RoomNotificationComposer.SendBubble("erro", Session.GetHabbo().Username + " Ele tentou te dar um emblema, mas você já tem!", "/inventory/open/badge"));
                }
            }
            Session.SendWhisper("Você deu emblema a cada usuário desse quarto!", 34);
        }
    }
}
