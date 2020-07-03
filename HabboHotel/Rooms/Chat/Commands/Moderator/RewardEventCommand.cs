using Newtonsoft.Json.Linq;
using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;
using System;
using System.Text;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class RewardEventCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";

        public string Parameters => "[USUARIO]";

        public string Description => "Todas as funções para recompensar um vencedor do evento.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, digite o usuário que deseja premiar!", 34);
                return;
            }

            GameClient Target = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (Target == null)
            {
                Session.SendWhisper("Opa, não foi possível encontrar esse usuário!", 34);
                return;
            }

            if (Target.GetHabbo().Username == Session.GetHabbo().Username && Session.GetHabbo().Rank <= 14)
            {
                Session.SendWhisper("Você não pode se premiar!", 34);
                return;
            }

            if (Target.GetRoomUser() != null)
            {
                Target.GetHabbo().UserPoints = Target.GetHabbo().UserPoints + 1;
                int Diamonds = Convert.ToInt32(StarBlueServer.GetConfig().data["event.diamonds"]);
                int Gotw = Convert.ToInt32(StarBlueServer.GetConfig().data["event.gotw"]);
                int Credits = Convert.ToInt32(StarBlueServer.GetConfig().data["event.credits"]);
                int Duckets = Convert.ToInt32(StarBlueServer.GetConfig().data["event.duckets"]);
                if (Target.GetHabbo().UserPoints > 50)
                {
                    if (Target.GetHabbo().UserPoints > 100)
                        Diamonds = 20;
                    else
                        Diamonds = Diamonds *= 2;
                    Credits = Credits *= 2;
                    Duckets = Duckets *= 2;
                }

                Target.GetHabbo().Diamonds += Diamonds;
                Target.GetHabbo().GOTWPoints += Gotw;
                Target.GetHabbo().Credits = Target.GetHabbo().Credits += Credits;
                Target.GetHabbo().Duckets = Target.GetHabbo().Duckets += Duckets;
                Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().Diamonds, Diamonds, 5));
                Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().GOTWPoints, Gotw, 103));
                Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().Duckets, Duckets));

                int UserPoints = Target.GetHabbo().UserPoints;
                string Badge = Convert.ToString(StarBlueServer.GetConfig().data["event.badge"]) + UserPoints;

                if (!Target.GetHabbo().GetBadgeComponent().HasBadge(Badge) && UserPoints <= 100)
                {
                    Target.GetHabbo().GetBadgeComponent().GiveBadge(Badge, true, Target);
                }

                if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Target.GetHabbo().CurrentRoomId, out Room TargetRoom))
                {
                    return;
                }

                if (TargetRoom.GetRoomUserManager() != null)
                {
                    TargetRoom.GetRoomUserManager().RemoveUserFromRoom(Target, true, false);
                }

                Target.SendMessage(RoomNotificationComposer.SendBubble("ganador", "Você ganhou o evento e ganhou 1 ponto, 1 emblema, " + Credits + " moedas, " + Duckets + " duckets, " + Diamonds + " diamantes e " + Gotw + " " + Convert.ToString(StarBlueServer.GetConfig().data["seasonal.currency.name"]) + ", parabéns!"));
                Session.SendWhisper("Você premiou com sucesso o usuário " + Target.GetHabbo().Username + ".");
                JObject WebEventData =
                new JObject(new JProperty("type", "eventwinner"), new JProperty("data", new JObject(
                    new JProperty("event", Encoding.UTF8.GetString(Encoding.Default.GetBytes(Room.Name))),
                    new JProperty("winner", Target.GetHabbo().Username + ";" + Target.GetHabbo().Look)
                )));
                StarBlueServer.GetGame().GetWebEventManager().BroadCastWebData(WebEventData.ToString());
            }
        }
    }
}
