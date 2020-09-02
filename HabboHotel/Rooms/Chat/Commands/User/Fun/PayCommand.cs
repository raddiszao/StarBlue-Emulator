using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Users;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class PayCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return "%user% %type% %amount%"; }
        }

        public string Description
        {
            get { return "Pague um usuário moedas/duckets/diamantes/honras!"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room CurrentRoom, string[] Params)
        {
            if (Params.Length != 4)
            {
                Session.SendWhisper("Use o seguinte formato para pagar um usuário :pay %usuário% %tipo% %quantidade%");
                return;
            }

            Habbo Habbo = Session.GetHabbo();
            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);

            if (TargetClient == null)
            {
                Session.SendWhisper("Ocorreu um erro ao encontrar esse usuário, talvez ele não esteja online.");
                return;
            }

            Habbo User = TargetClient.GetHabbo();
            if (User == Habbo)
            {
                Session.SendWhisper("Você não pode pagar si mesmo.");
                return;
            }

            int Amount = 0;
            if (!int.TryParse(Params[3], out Amount) || Amount <= 0)
            {
                Session.SendWhisper("Insira um valor válido.");
                return;
            }

            switch (Params[2].ToLower())
            {
                case "credit":
                case "coin":
                case "credits":
                case "coins":
                case "moedas":

                    if (Habbo.Credits < Amount)
                    {
                        Session.SendWhisper("Você não tem créditos suficientes.");
                        return;
                    }

                    Habbo.Credits -= Amount;
                    Session.SendMessage(new CreditBalanceComposer(Habbo.Credits));
                    User.Credits += Amount;
                    User.GetClient().SendMessage(new CreditBalanceComposer(User.Credits));

                    Session.SendNotification($"Você enviou {Amount} moedas para {User.Username}.");
                    User.GetClient().SendNotification($"Você recebeu {Amount} moedas de {Habbo.Username}.");

                    return;

                case "duckets":
                case "pixels":
                case "ducket":
                case "pixel":
                    if (Habbo.Duckets < Amount)
                    {
                        Session.SendWhisper("Você não tem duckets suficientes..");
                        return;
                    }

                    Habbo.Duckets -= Amount;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Habbo.Duckets, Amount * -1));
                    User.Duckets += Amount;
                    User.GetClient().SendMessage(new HabboActivityPointNotificationComposer(User.Duckets, Amount));

                    Session.SendNotification($"Você enviou {Amount} duckets para {User.Username}.");
                    User.GetClient().SendNotification($"Você recebeu {Amount} duckets de {Habbo.Username}.");

                    return;

                case "diamond":
                case "diamonds":
                case "diamantes":
                    if (Habbo.Diamonds < Amount)
                    {
                        Session.SendWhisper("Você não possui diamantes suficientes.");
                        return;
                    }

                    Habbo.Diamonds -= Amount;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Habbo.Diamonds, Amount * -1, 5));
                    User.Diamonds += Amount;
                    User.GetClient().SendMessage(new HabboActivityPointNotificationComposer(User.Diamonds, Amount, 5));

                    Session.SendNotification($"Você enviou {Amount} diamantes para {User.Username}.");
                    User.GetClient().SendNotification($"Você recebeu {Amount} diamantes de {Habbo.Username}.");
                    return;

                case "honras":
                case "gotw":
                    if (Habbo.GOTWPoints < Amount)
                    {
                        Session.SendWhisper("Você não possui honras suficientes.");
                        return;
                    }

                    Habbo.GOTWPoints -= Amount;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Habbo.GOTWPoints, Amount * -1, 103));
                    User.GOTWPoints += Amount;
                    User.GetClient().SendMessage(new HabboActivityPointNotificationComposer(User.GOTWPoints, Amount, 103));

                    Session.SendNotification($"Você enviou {Amount} honras para {User.Username}.");
                    User.GetClient().SendNotification($"Você recebeu {Amount} honras de {Habbo.Username}.");
                    return;

                default:
                    Session.SendWhisper("Insira um tipo de moeda válida.");
                    return;
            }

        }
    }
}