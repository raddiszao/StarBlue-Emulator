using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms;
using StarBlue.Utilities;
using System;
using System.Threading;

namespace StarBlue.HabboHotel.Items.Interactor
{
    internal class InteractorSlotmachine : IFurniInteractor
    {
        private string Rand1;
        private string Rand2;
        private string Rand3;
        private int paga;

        public void OnPlace(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {

            if ((Math.Abs(Item.GetX - Session.GetRoomUser().X) >= 2) || (Math.Abs(Item.GetY - Session.GetRoomUser().Y) >= 2))
            {
                Session.SendWhisper("Você está muito longe da máquina de diamantes para apostar.", 34);
                return;
            }

            if (Session.GetHabbo().Diamonds <= 0)
            {
                Session.SendWhisper("Para poder apostar, você deve ter diamantes, agora você tem 0.", 34);
                return;
            }

            if (Session.GetHabbo()._bet > Session.GetHabbo().Diamonds)
            {
                Session.SendWhisper("Você está apostando mais diamantes do que você tem.", 34);
                return;
            }

            if (Session.GetHabbo()._bet <= 0)
            {
                Session.SendWhisper("Você não pode apostar 0 diamantes. Use o seguinte comando para apostar:", 34);
                Session.SendWhisper("\":apostar %quantidade%\"", 34);
                return;
            }

            if (Session == null || Session.GetHabbo() == null || Item == null)
            {
                return;
            }

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            RoomUser Actor = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (Actor == null)
            {
                return;
            }

            if (Gamemap.TileDistance(Actor.X, Actor.Y, Item.GetX, Item.GetY) < 2)
            {
                int Bet = Session.GetHabbo()._bet;

                Session.GetHabbo().Diamonds -= Bet;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, -Bet, 5));

                int Random1 = RandomNumber.GenerateRandom(1, 3);
                switch (Random1)
                {
                    case 1:
                        Rand1 = "¥";
                        break;
                    case 2:
                        Rand1 = "|";
                        break;
                    case 3:
                        Rand1 = "ª";
                        break;
                }

                int Random2 = RandomNumber.GenerateRandom(1, 3);
                switch (Random2)
                {
                    case 1:
                        Rand2 = "¥";
                        break;
                    case 2:
                        Rand2 = "|";
                        break;
                    case 3:
                        Rand2 = "ª";
                        break;
                }

                int Random3 = RandomNumber.GenerateRandom(1, 3);
                switch (Random3)
                {
                    case 1:
                        Rand3 = "¥";
                        break;
                    case 2:
                        Rand3 = "|";
                        break;
                    case 3:
                        Rand3 = "ª";
                        break;
                }
                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (User == null || User.GetClient() == null)
                {
                    return;
                }

                Session.SendMessage(new ChatComposer(User.VirtualId, Session.GetHabbo().Username + ", não foi dessa vez [ " + Rand1 + " - " + Rand2 + " - " + Rand3 + " ]!", 0, 34));
                Item.ExtraData = "1";
                Item.UpdateState(true, true);

                new Thread(() =>
                {
                    Thread.Sleep(1000);
                    Item.ExtraData = "0";
                    Item.UpdateState(true, true);
                }).Start();

                if (Random1 == Random2 && Random1 == Random3 && Random3 == Random2)
                {

                    switch (Random1)
                    {
                        case 1:
                            Session.GetHabbo().Diamonds += Bet * 3;
                            Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, -Bet, 5));
                            Session.SendMessage(new ChatComposer(User.VirtualId, Session.GetHabbo().Username + " Você ganhou " + Bet * 3 + " diamantes com uma estrela tripla!", 0, 34));
                            StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Actor.GetClient(), "ACH_StarBet", 1);
                            break;
                        case 2:
                            Session.GetHabbo().Diamonds += Bet * 2;
                            Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, -Bet, 5));
                            Session.SendMessage(new ChatComposer(User.VirtualId, Session.GetHabbo().Username + " Você ganhou " + Bet * 2 + " diamantes com um coração triplo!", 0, 34));
                            StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Actor.GetClient(), "ACH_HeartBet", 1);
                            break;
                        case 3:
                            Session.GetHabbo().Diamonds += Bet * 1;
                            Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, -Bet, 5));
                            Session.SendMessage(new ChatComposer(User.VirtualId, Session.GetHabbo().Username + " Você ganhou " + Bet * 1 + " diamantes com uma caveira tripla!", 0, 34));
                            StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Actor.GetClient(), "ACH_SkullBet", 1);
                            break;
                    }
                }

                StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Actor.GetClient(), "ACH_GeneralBet", 1);
                return;
            }
        }

        public void OnWiredTrigger(Item Item)
        {

        }
    }
}
