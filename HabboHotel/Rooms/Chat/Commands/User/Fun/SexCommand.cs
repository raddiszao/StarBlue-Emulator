using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;
using System;
using System.Threading;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    internal class SexCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "[USUARIO]";

        public string Description => "Faça sexo com um usuário.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (!Room.RoomData.SexEnabled && !Room.CheckRights(Session, true) && !Session.GetHabbo().GetPermissions().HasRight("room_override_custom_config"))
            {
                Session.SendWhisper("Oops, o dono do quarto desativou esta função.", 34);
                return;
            }

            long nowTime = StarBlueServer.CurrentTimeMillis();
            long timeBetween = nowTime - Session.GetHabbo()._lastTimeUsedHelpCommand;
            if (timeBetween < 60000)
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("advice", "Espere pelo menos 1 minuto para reutilizar o comando.", ""));
                return;
            }

            RoomUser roomUserByHabbo1 = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (roomUserByHabbo1 == null)
            {
                return;
            }

            if (Params.Length == 1)
            {
                Session.SendWhisper("Você deve digitar o nome de usuário da pessoa com quem deseja fazer sexo.", 34);
            }
            else
            {
                Session.GetHabbo()._lastTimeUsedHelpCommand = nowTime;
                RoomUser roomUserByHabbo2 = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Params[1]);
                GameClient clientByUsername = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
                if (clientByUsername == null)
                {
                    Session.SendWhisper("Usuário não encontrado.", 34);
                    return;
                }

                if (clientByUsername.GetHabbo().Username == Session.GetHabbo().Username)
                {

                    RoomUser Self = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                    if (roomUserByHabbo1 == Self)
                    {
                        Session.SendWhisper("Você não pode fazer sexo com você mesmo!", 34);
                        return;
                    }
                }
                else if (clientByUsername.GetHabbo().CurrentRoomId == Session.GetHabbo().CurrentRoomId && (Math.Abs(checked(roomUserByHabbo1.X - roomUserByHabbo2.X)) < 2 && Math.Abs(checked(roomUserByHabbo1.Y - roomUserByHabbo2.Y)) < 2))
                {
                    if ((Session.GetHabbo().sexWith == null || Session.GetHabbo().sexWith == "") && (clientByUsername.GetHabbo().Username != Session.GetHabbo().sexWith && Session.GetHabbo().Username != clientByUsername.GetHabbo().sexWith))
                    {
                        Session.GetHabbo().sexWith = clientByUsername.GetHabbo().Username;
                        clientByUsername.SendNotification(Session.GetHabbo().Username + " pediu para ter sexo com você, ter relações com " + Session.GetHabbo().Username + " escreva \":sexo " + Session.GetHabbo().Username + "\"");
                        Session.SendNotification("Você enviou sua solicitação de sexo para " + clientByUsername.GetHabbo().Username + ". Se el@ responder, você poderá fazer sexo.");
                    }
                    else if (roomUserByHabbo2 != null)
                    {
                        if (clientByUsername.GetHabbo().sexWith == Session.GetHabbo().Username)
                        {
                            if (roomUserByHabbo2.GetClient() != null && roomUserByHabbo2.GetClient().GetHabbo() != null)
                            {
                                if (clientByUsername.GetHabbo().CurrentRoomId == Session.GetHabbo().CurrentRoomId && (Math.Abs(checked(roomUserByHabbo1.X - roomUserByHabbo2.X)) < 2 && Math.Abs(checked(roomUserByHabbo1.Y - roomUserByHabbo2.Y)) < 2))
                                {
                                    clientByUsername.GetHabbo().sexWith = null;
                                    Session.GetHabbo().sexWith = null;
                                    if (Session.GetHabbo().Gender == "m")
                                    {
                                        Room.SendMessage(new ChatComposer(roomUserByHabbo1.VirtualId, "*Agarro o " + Params[1] + " ele vira, e começo a meter com força* ", 0, 16), false);
                                        Thread.Sleep(2000);
                                        roomUserByHabbo2.ApplyEffect(9);
                                        roomUserByHabbo1.ApplyEffect(469);
                                        Room.SendMessage(new ChatComposer(roomUserByHabbo2.VirtualId, "*Olho de forma maliciosa o" + Session.GetHabbo().Username + " * ", 0, 16), false);
                                        Thread.Sleep(2000);
                                        Room.SendMessage(new ChatComposer(roomUserByHabbo1.VirtualId, "*Tocando devagar o " + Params[1] + ", coloco meu pau * ", 0, 16), false);
                                        Room.SendMessage(new ChatComposer(roomUserByHabbo1.VirtualId, "*Você gosta né " + Params[1] + " ?, enfiando rapido* ", 0, 16), false);
                                        Room.SendMessage(new ShoutComposer(roomUserByHabbo2.VirtualId, "*Chupar o " + Session.GetHabbo().Username + ", me encanta *", 0, 16), false);
                                        Room.SendMessage(new ChatComposer(roomUserByHabbo2.VirtualId, "ahhh ahhh, como é bom gozar, ahhhhhhh *", 0, 16), false);
                                        Thread.Sleep(2000);
                                        Room.SendMessage(new ChatComposer(roomUserByHabbo1.VirtualId, "*É gostoso, estou prestes a gozar*", 0, 16), false);
                                        Room.SendMessage(new ChatComposer(roomUserByHabbo2.VirtualId, "*Toco minha virilha para melhorar o orgasmo " + Session.GetHabbo().Username + " *", 0, 16), false);
                                        Thread.Sleep(2000);
                                        Room.SendMessage(new ChatComposer(roomUserByHabbo1.VirtualId, "*Pego meu pau e gozo nela " + Params[1] + " * ", 0, 16), false);
                                        Room.SendMessage(new ChatComposer(roomUserByHabbo2.VirtualId, "*Olhar malicioso* Espero que isso se repita!", 0, 16), false);
                                        Thread.Sleep(2000);
                                    }
                                    else
                                    {
                                        Room.SendMessage(new ChatComposer(roomUserByHabbo2.VirtualId, "*Bato uma punheta para " + Session.GetHabbo().Username + "*", 0, 16), false);
                                        Thread.Sleep(1000);
                                        roomUserByHabbo2.ApplyEffect(9);
                                        roomUserByHabbo1.ApplyEffect(469);
                                        Room.SendMessage(new ChatComposer(roomUserByHabbo1.VirtualId, "*Agarro " + Params[1] + " e começo meter com força*", 0, 16), false);
                                        Thread.Sleep(2000);
                                        Room.SendMessage(new ChatComposer(roomUserByHabbo2.VirtualId, "*Lubrificando minha buceta para " + Session.GetHabbo().Username + "*", 0, 16), false);
                                        Room.SendMessage(new ChatComposer(roomUserByHabbo1.VirtualId, "*Mmmmmm* *Metendo forte!* Que gostoso!.. |.|", 0, 16), false);
                                        Thread.Sleep(2000);
                                        Room.SendMessage(new ChatComposer(roomUserByHabbo2.VirtualId, "*Estou prestes a gozar " + Session.GetHabbo().Username + "* ", 0, 16), false);
                                        Room.SendMessage(new ChatComposer(roomUserByHabbo1.VirtualId, "*mm, mm, mm, mm, mmmmmm vou gozaaarrr!*", 0, 16), false);
                                        Thread.Sleep(2000);
                                        Room.SendMessage(new ChatComposer(roomUserByHabbo2.VirtualId, "*Goza em mim " + Session.GetHabbo().Username + "*", 0, 16), false);
                                        Room.SendMessage(new ChatComposer(roomUserByHabbo1.VirtualId, "*Olhar malicioso* Espero que isso se repita!", 0, 16), false);
                                        Thread.Sleep(1000);
                                    }

                                    if (!roomUserByHabbo1.Statusses.ContainsKey("lay"))
                                    {
                                        roomUserByHabbo1.RotBody--;//
                                        roomUserByHabbo1.Statusses.Add("lay", "1.0 null");
                                        roomUserByHabbo1.Z -= 0.35;
                                        roomUserByHabbo1.isLying = true;
                                        roomUserByHabbo1.UpdateNeeded = true;
                                    }

                                    if (!roomUserByHabbo2.Statusses.ContainsKey("lay"))
                                    {
                                        roomUserByHabbo2.RotBody--;//
                                        roomUserByHabbo2.Statusses.Add("lay", "1.0 null");
                                        roomUserByHabbo2.Z -= 0.35;
                                        roomUserByHabbo2.isLying = true;
                                        roomUserByHabbo2.UpdateNeeded = true;
                                    }

                                }
                                else
                                {
                                    Session.SendWhisper("Este usuário está muito longe para fazer sexo com você!", 34);
                                }
                            }
                            else
                            {
                                Session.SendWhisper("Houve um erro ao encontrar o usuário.", 34);
                            }
                        }
                        else
                        {
                            Session.SendWhisper("Este usuário não aceitou sua solicitação por sexo.", 34);
                        }
                    }
                    else
                    {
                        Session.SendWhisper("Este usuário não pôde ser encontrado na sala.", 34);
                    }
                }
                else
                {
                    Session.SendWhisper("Esse usuário está muito longe para fazer sexo com você!", 34);
                }
            }
        }
    }
}
