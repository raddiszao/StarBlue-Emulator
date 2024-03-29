﻿using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    internal class PushCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "[USUARIO]";

        public string Description => "Empurra alguém da tua posição.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.RoomData.PushEnabled && !Session.GetHabbo().GetPermissions().HasRight("room_override_custom_config"))
            {
                Session.SendWhisper("Oops, o dono do quarto desativou esta função.", 34);
                return;
            }

            GameClient TargetClient = null;
            if (Params.Length == 1)
            {
                Point UserInFront = Session.GetRoomUser().SquareInFront;
                List<RoomUser> UsersInSquare = Room.GetGameMap().GetRoomUsers(UserInFront);
                if (UsersInSquare.Count > 0)
                {
                    TargetClient = UsersInSquare[0].GetClient();
                }
            }
            else
            {
                TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            }

            if (TargetClient == null)
            {
                Session.SendWhisper("Houve um problema, aparentemente o usuário não está online ou você não escreveu o nome corretamente", 34);
                return;
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (TargetUser == null)
            {
                Session.SendWhisper("Ocorreu um erro, escreva o nome corretamente, o usuário NÃO está online ou na sala.", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Venha! você não vai querer se esforçar", 34);
                return;
            }

            if (TargetUser.TeleportEnabled)
            {
                Session.SendWhisper("Oops, você não pode empurrar um usuário enquanto ele tiver o modo de teletransporte ativado.", 34);
                return;
            }

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
            {
                return;
            }

            if (!((Math.Abs(TargetUser.X - ThisUser.X) >= 2) || (Math.Abs(TargetUser.Y - ThisUser.Y) >= 2)))
            {
                if (ThisUser.RotBody == 4)
                {
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y + 1);
                }

                if (ThisUser.RotBody == 0)
                {
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y - 1);
                }

                if (ThisUser.RotBody == 6)
                {
                    TargetUser.MoveTo(TargetUser.X - 1, TargetUser.Y);
                }

                if (ThisUser.RotBody == 2)
                {
                    TargetUser.MoveTo(TargetUser.X + 1, TargetUser.Y);
                }

                if (ThisUser.RotBody == 3)
                {
                    TargetUser.MoveTo(TargetUser.X + 1, TargetUser.Y);
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y + 1);
                }

                if (ThisUser.RotBody == 1)
                {
                    TargetUser.MoveTo(TargetUser.X + 1, TargetUser.Y);
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y - 1);
                }

                if (ThisUser.RotBody == 7)
                {
                    TargetUser.MoveTo(TargetUser.X - 1, TargetUser.Y);
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y - 1);
                }

                if (ThisUser.RotBody == 5)
                {
                    TargetUser.MoveTo(TargetUser.X - 1, TargetUser.Y);
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y + 1);
                }

                Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "*Eu empurrei " + TargetUser.GetClient().GetHabbo().Username + "* ", 0, 31));
            }
            else
            {
                Session.SendWhisper("Oops, " + TargetUser.GetClient().GetHabbo().Username + " não está perto o suficiente!");
            }
        }
    }
}
