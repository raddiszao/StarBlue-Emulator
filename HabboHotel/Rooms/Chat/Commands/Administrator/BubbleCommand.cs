using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.HabboHotel.Rooms.Chat.Styles;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class BubbleCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_vip"; }
        }

        public string Parameters
        {
            get { return "[ID]"; }
        }

        public string Description
        {
            get { return "Altere o balão do chat"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            if (Params.Length == 1)
            {
                Session.SendWhisper("Oops, você não inseriu o ID", 34);
                return;
            }

            if (!int.TryParse(Params[1].ToString(), out int Bubble))
            {
                Session.SendWhisper("Por favor insira um número valido.", 34);
                return;
            }

            if (!StarBlueServer.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Bubble, out ChatStyle Style) || (Style.RequiredRight.Length > 0 && !Session.GetHabbo().GetPermissions().HasRight(Style.RequiredRight)))
            {
                Session.SendWhisper("Oops, não pode usar isso por causa do bloqueio de Ranks [ Raros: 32, 28]!", 34);
                return;
            }

            User.LastBubble = Bubble;
            Session.GetHabbo().CustomBubbleId = Bubble;
            Session.SendWhisper("Bolha escolhida como: " + Bubble);
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE `users` SET `bubble_id` = '" + Bubble + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }
        }
    }
}