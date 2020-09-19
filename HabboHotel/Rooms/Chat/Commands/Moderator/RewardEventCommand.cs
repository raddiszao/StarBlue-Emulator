using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Communication.Packets.Outgoing.WebSocket;
using StarBlue.HabboHotel.GameClients;
using System;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class RewardEventCommand : IChatCommand
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
                int LastUserPoints = Target.GetHabbo().UserPoints;
                Target.GetHabbo().UserPoints = Target.GetHabbo().UserPoints + 1;
                int Diamonds = Convert.ToInt32(StarBlueServer.GetConfig().data["event.diamonds"]);
                int Gotw = Convert.ToInt32(StarBlueServer.GetConfig().data["event.gotw"]);
                int Credits = Convert.ToInt32(StarBlueServer.GetConfig().data["event.credits"]);
                int Duckets = Convert.ToInt32(StarBlueServer.GetConfig().data["event.duckets"]);
                if (Target.GetHabbo().UserPoints > 50)
                {
                    if (Target.GetHabbo().UserPoints > 100)
                    {
                        Diamonds = 20;
                    }
                    else
                    {
                        Diamonds *= 2;
                    }

                    Credits *= 2;
                    Duckets *= 2;
                    Gotw *= 2;
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
                string LastBadge = Convert.ToString(StarBlueServer.GetConfig().data["event.badge"]) + LastUserPoints;

                if (!Target.GetHabbo().GetBadgeComponent().HasBadge(Badge) && UserPoints <= 100)
                {
                    Target.GetHabbo().GetBadgeComponent().GiveBadge(Badge, true, Target);
                }

                if (LastUserPoints > 0 && LastUserPoints < 100 && Target.GetHabbo().GetBadgeComponent().HasBadge(LastBadge))
                {
                    Target.GetHabbo().GetBadgeComponent().RemoveBadge(LastBadge, Target);
                }

                if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Target.GetHabbo().CurrentRoomId, out Room TargetRoom))
                {
                    return;
                }

                if (TargetRoom.GetRoomUserManager() != null)
                {
                    TargetRoom.GetRoomUserManager().RemoveUserFromRoom(Target, true, false);
                }

                Target.GetHabbo().SaveKey("puntos_eventos", Convert.ToString(Target.GetHabbo().UserPoints));
                Target.SendMessage(RoomNotificationComposer.SendBubble("ganador", "Você ganhou o evento e recebeu 1 ponto (" + Target.GetHabbo().UserPoints + " eventos), 1 emblema, " + Credits + " moedas, " + Duckets + " duckets, " + Diamonds + " diamantes e " + Gotw + " " + Convert.ToString(StarBlueServer.GetConfig().data["seasonal.currency.name"]) + ", parabéns!"));
                Session.SendWhisper("Você premiou com sucesso o usuário " + Target.GetHabbo().Username + ".");

                foreach (GameClient Client in StarBlueServer.GetGame().GetClientManager().GetClients.ToList())
                {
                    if (Client == null || Client.GetHabbo() == null)
                    {
                        continue;
                    }

                    if (!Client.GetHabbo().SendWebPacket(new EventWinnerComposer(Room.RoomData.Name, Target.GetHabbo().Username + ";" + Target.GetHabbo().Look)))
                    {
                        Client.SendMessage(RoomNotificationComposer.SendBubble("eventb", "O usuário " + Target.GetHabbo().Username + " ganhou o evento " + Room.RoomData.Name + "!"));
                    }
                }
            }
        }
    }
}
