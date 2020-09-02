using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    internal class PullCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "[USUARIO]";

        public string Description => "Puxar um usuário até você.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.RoomData.PullEnabled && !Session.GetHabbo().GetPermissions().HasRight("room_override_custom_config"))
            {
                Session.SendWhisper("Oops, o dono do quarto desativou esta função.", 34);
                return;
            }

            GameClient TargetClient = null;
            if (Params.Length == 1)
            {
                Point UserInFront2 = Session.GetRoomUser().SquareInFront2;
                List<RoomUser> UsersInSquare2 = Room.GetGameMap().GetRoomUsers(UserInFront2);
                if (UsersInSquare2.Count > 0)
                {
                    TargetClient = UsersInSquare2[0].GetClient();
                }
                else
                {
                    Point UserInFront = Session.GetRoomUser().SquareInFront;
                    List<RoomUser> UsersInSquare = Room.GetGameMap().GetRoomUsers(UserInFront);
                    if (UsersInSquare.Count > 0)
                    {
                        TargetClient = UsersInSquare[0].GetClient();
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            }

            if (TargetClient == null)
            {
                Session.SendWhisper("Ocorreu um erro, o usuário não foi obtido, talvez não esteja online.", 34);
                return;
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (TargetUser == null)
            {
                Session.SendWhisper("Ocorreu um erro, escreva o nome corretamente, o usuário NÃO está on-line ou na sala.", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Venha! Você não pode se puxar..", 34);
                return;
            }

            if (TargetUser.TeleportEnabled)
            {
                Session.SendWhisper("Oops, você não pode puxar um usuário enquanto ele tiver o modo de teletransporte ativado.", 34);
                return;
            }

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
            {
                return;
            }

            string PushDirection = "down";
            if (TargetClient.GetHabbo().CurrentRoomId == Session.GetHabbo().CurrentRoomId && (Math.Abs(ThisUser.X - TargetUser.X) < 3 && Math.Abs(ThisUser.Y - TargetUser.Y) < 3))
            {
                Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "*Eu puxei " + TargetUser.GetClient().GetHabbo().Username + "*", 0, 31));

                if (ThisUser.RotBody == 0)
                {
                    PushDirection = "up";
                }

                if (ThisUser.RotBody == 2)
                {
                    PushDirection = "right";
                }

                if (ThisUser.RotBody == 4)
                {
                    PushDirection = "down";
                }

                if (ThisUser.RotBody == 6)
                {
                    PushDirection = "left";
                }

                if (PushDirection == "up")
                {
                    TargetUser.MoveTo(ThisUser.X, ThisUser.Y - 1);
                }

                if (PushDirection == "right")
                {
                    TargetUser.MoveTo(ThisUser.X + 1, ThisUser.Y);
                }

                if (PushDirection == "down")
                {
                    TargetUser.MoveTo(ThisUser.X, ThisUser.Y + 1);
                }

                if (PushDirection == "left")
                {
                    TargetUser.MoveTo(ThisUser.X - 1, ThisUser.Y);
                }
            }
            else
            {
                Session.SendWhisper("Este usuário não está perto o suficiente, aproxime-se!!", 34);
                return;
            }
        }
    }
}
