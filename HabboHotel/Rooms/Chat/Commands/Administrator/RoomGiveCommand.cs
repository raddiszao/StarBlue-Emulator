using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Communication.Packets.Outgoing.Rooms.Nux;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class RoomGiveCommand : IChatCommand
    {
        public string PermissionRequired => "user_16";
        public string Parameters => "[MOEDA] [QUANTIDADE]";
        public string Description => "Dar créditos, duckets, diamantes para o quarto.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduza ! (coins, duckets, diamonds, pixels)");
                return;
            }

            string UpdateVal = Params[1];
            switch (UpdateVal.ToLower())
            {
                case "coins":
                case "credits":
                case "creditos":
                    {
                        if (Params.Length == 1)
                        {
                            Session.SendWhisper("Introduza o número de créditos");
                            return;
                        }
                        else
                        {
                            if (int.TryParse(Params[2], out int Amount))
                            {
                                foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
                                {
                                    User.GetClient().GetHabbo().Credits = User.GetClient().GetHabbo().Credits + Amount;
                                    User.GetClient().SendMessage(new CreditBalanceComposer(User.GetClient().GetHabbo().Credits));
                                    User.GetClient().SendMessage(new RoomCustomizedAlertComposer(Session.GetHabbo().Username + " te enviou  " + Amount + " Créditos."));
                                }
                            }
                        }
                        Session.SendWhisper("Enviou para o quarto " + Params[2] + " Créditos!");
                    }
                    break;

                case "diamonds":
                case "diamantes":

                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_give_diamonds"))
                        {
                            Session.SendWhisper("Oops, não tem permissão para usar este comando.", 34);
                            break;
                        }
                        else
                        {
                            if (int.TryParse(Params[2], out int Amount))
                            {
                                if (Amount > 5000)
                                {
                                    Session.SendWhisper("Não é possivel enviar mais de 50 pixels, isto será notificado ao CEO e ele tomará providências.");
                                    return;
                                }
                                else
                                {
                                    foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers())
                                    {
                                        User.GetClient().GetHabbo().Diamonds = User.GetClient().GetHabbo().Diamonds + Amount;
                                        User.GetClient().SendMessage(new HabboActivityPointNotificationComposer(User.GetClient().GetHabbo().Diamonds, Amount, 5));
                                        User.GetClient().SendMessage(new RoomCustomizedAlertComposer(Session.GetHabbo().Username + " te enviou " + Amount + " Diamantes."));
                                    }
                                    Session.SendWhisper("Enviou para o quarto " + Params[2] + " diamantes!");

                                }
                            }
                        }
                    }
                    break;

                case "reward":
                case "regalo":
                case "premio":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_roomgive_reward"))
                        {
                            Session.SendWhisper("Oops, não tem permissão para usar este comando!");
                            break;
                        }

                        else
                        {
                            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers())
                            {
                                User.GetClient().SendMessage(new NuxItemListComposer());
                            }
                        }
                    }
                    break;

                case "duckets":
                    {
                        if (Params.Length == 1)
                        {
                            Session.SendWhisper("Introduza o número de duckets");
                            return;
                        }
                        else
                        {
                            if (int.TryParse(Params[2], out int Amount))
                            {
                                if (Amount > 300)
                                {
                                    Session.SendWhisper("Não é possivel enviar mais de 300 duckets, isto será notificado ao CEO e ele tomará providências.");
                                    return;
                                }
                                else
                                {
                                    foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers())
                                    {
                                        User.GetClient().GetHabbo().Duckets = User.GetClient().GetHabbo().Duckets + Amount;
                                        User.GetClient().SendMessage(new HabboActivityPointNotificationComposer(User.GetClient().GetHabbo().Duckets, Amount));
                                        User.GetClient().SendMessage(new RoomCustomizedAlertComposer(Session.GetHabbo().Username + " te enviou " + Amount + " Duckets."));
                                    }
                                }
                            }
                        }
                        Session.SendWhisper("Enviou para o quarto " + Params[2] + " Duckets!");
                    }
                    break;
            }
        }
    }
}
