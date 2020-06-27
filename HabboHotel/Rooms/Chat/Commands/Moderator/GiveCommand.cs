using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Communication.Packets.Outgoing.Rooms.Nux;
using StarBlue.HabboHotel.GameClients;
using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GiveCommand : IChatCommand
    {
        public string PermissionRequired => "user_15";
        public string Parameters => "[USUARIO] [MOEDA] [QUANTIDADE]";
        public string Description => "Dar créditos, duckets, diamantes a um usuário.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, insira (moedas, duckets, diamantes, gotw)", 34);
                return;
            }

            GameClient Target = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (Target == null)
            {
                Session.SendWhisper("Oops, Este usuário não foi encontrado!", 34);
                return;
            }

            string UpdateVal = Params[2];
            switch (UpdateVal.ToLower())
            {
                case "coins":
                case "credits":
                case "moedas":
                case "creditos":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_give_coins"))
                        {
                            Session.SendWhisper("Oops, você não tem as permissões necessárias para usar este comando!");
                            break;
                        }
                        else
                        {
                            if (int.TryParse(Params[3], out int Amount))
                            {
                                Target.GetHabbo().Credits = Target.GetHabbo().Credits += Amount;
                                Target.SendMessage(new CreditBalanceComposer(Target.GetHabbo().Credits));

                                if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                {
                                    Target.SendMessage(RoomNotificationComposer.SendBubble("cred", "" + Session.GetHabbo().Username + " te enviou " + Amount.ToString() + " créditos.", ""));
                                }

                                Session.SendWhisper("Você enviou " + Amount + " Credito(s) a " + Target.GetHabbo().Username + "!", 34);
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Oops, quantidades apenas em números..");
                                break;
                            }
                        }
                    }

                case "pixels":
                case "duckets":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_give_pixels"))
                        {
                            Session.SendWhisper("Oops, você não tem as permissões necessárias para enviar duckets!");
                            break;
                        }
                        else
                        {
                            if (int.TryParse(Params[3], out int Amount))
                            {
                                {
                                    Target.GetHabbo().Duckets += Amount;
                                    Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().Duckets, Amount));

                                    if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                    {
                                        Target.SendMessage(RoomNotificationComposer.SendBubble("duckets", "" + Session.GetHabbo().Username + " te enviou " + Amount.ToString() + " duckets.", ""));
                                    }

                                    Session.SendWhisper("Você enviou " + Amount + " Ducket(s) a " + Target.GetHabbo().Username + "!", 34);
                                    break;
                                }
                            }
                            else
                            {
                                Session.SendWhisper("Oops, quantidades apenas em números..");
                                break;
                            }
                        }
                    }

                case "diamonds":
                case "diamantes":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_give_diamonds"))
                        {
                            Session.SendWhisper("Oops, você não tem as permissões necessárias para usar este comando!");
                            break;
                        }
                        else
                        {
                            if (int.TryParse(Params[3], out int Amount))
                            {
                                if (Session.GetHabbo().Rank >= 16)
                                {
                                    Target.GetHabbo().Diamonds += Amount;
                                    Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().Diamonds, Amount, 5));

                                    if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                    {
                                        Target.SendNotification(Session.GetHabbo().Username + " te enviou " + Amount.ToString() + " Diamante(s)!");
                                    }

                                    Session.SendWhisper("Você enviou " + Amount + " Diamante(s) a " + Target.GetHabbo().Username + "!");
                                    break;
                                }
                                else

                                if (Amount > 5000)
                                {
                                    Session.SendWhisper("Não pode enviar mais de 5000 diamantes, isso será notificado ao CEO e tomará providências.");
                                    return;
                                }
                                Target.GetHabbo().Diamonds += Amount;
                                Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().Diamonds, Amount, 5));

                                if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                {
                                    Target.SendMessage(RoomNotificationComposer.SendBubble("diamonds", "" + Session.GetHabbo().Username + " te enviou " + Amount.ToString() + " Diamante(s).", ""));
                                }

                                Session.SendWhisper("Você enviou " + Amount + " Diamante(s) a " + Target.GetHabbo().Username + "!");
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Oops, as quantidades apenas em números..!");
                                break;
                            }
                        }
                    }

                case "premio":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_massgive_reward"))
                        {
                            Session.SendWhisper("Oops, você não tem as permissões necessárias para usar este comando!");
                            break;
                        }
                        else
                        {

                            if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                            {
                                Target.SendMessage(new NuxItemListComposer());
                            }

                            break;
                        }

                    }

                case "gotw":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_give_gotw"))
                        {
                            Session.SendWhisper("Opa, você não tem as permissões necessárias para usar este comando!");
                            break;
                        }

                        else
                        {
                            if (int.TryParse(Params[3], out int Amount))
                            {
                                if (Session.GetHabbo().Rank < 15 && Amount > 5000)
                                {
                                    Session.SendWhisper("Não pode enviar mais de 5000 " + Convert.ToString(StarBlueServer.GetConfig().data["seasonal.currency.name"]) + ", isso será notificado ao CEO e as medidas apropriadas serão tomadas.");
                                    return;
                                }

                                Target.GetHabbo().GOTWPoints = Target.GetHabbo().GOTWPoints + Amount;
                                Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().GOTWPoints, Amount, 103));

                                if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                {
                                    Target.SendMessage(RoomNotificationComposer.SendBubble("eventoxx", "" + Session.GetHabbo().Username + " te enviou " + Amount + " " + Convert.ToString(StarBlueServer.GetConfig().data["seasonal.currency.name"]) + "."));
                                }

                                Session.SendMessage(RoomNotificationComposer.SendBubble("eventoxx", "Acaba de enviar " + Amount + " " + Convert.ToString(StarBlueServer.GetConfig().data["seasonal.currency.name"]) + " a " + Target.GetHabbo().Username + "\nLembre-se que colocamos sua confiança em você e que esses comandos são vistos ao vivo."));
                                StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Target, "ACH_AnimationRanking", 1);
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Você só pode inserir parâmetros numéricos, de 1 a 50.");
                                break;
                            }
                        }
                    }
                default:
                    Session.SendWhisper("'" + UpdateVal + "' Não é uma moeda válida.");
                    break;
            }
        }
    }
}