﻿using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    internal class SuperPullCommand : IChatCommand
    {
        public string PermissionRequired => "user_vip";

        public string Parameters => "[USUARIO]";

        public string Description => "Puxe alguém sem limite algum";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite o nome do usuário que você deseja fazer superpuxar.", 34);
                return;
            }

            if (!Room.RoomData.SPullEnabled && !Room.CheckRights(Session, true) && !Session.GetHabbo().GetPermissions().HasRight("room_override_custom_config"))
            {
                Session.SendWhisper("Oops, o dono do quarto desativou esta função.", 34);
                return;
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Há um erro, o usuário não está online ou não está na sala.", 34);
                return;
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (TargetUser == null)
            {
                Session.SendWhisper("Há um erro, o usuário não está online ou não está na sala.", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Certamente você não vai querer se puxar!", 34);
                return;
            }

            if (TargetUser.TeleportEnabled)
            {
                Session.SendWhisper("Ops, você não pode empurrar um usuário enquanto o modo de teleport estiver ativado.", 34);
                return;
            }

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
            {
                return;
            }

            if (ThisUser.RotBody % 2 != 0)
            {
                ThisUser.RotBody--;
            }

            if (ThisUser.RotBody == 0)
            {
                TargetUser.MoveTo(ThisUser.X, ThisUser.Y - 1);
            }
            else if (ThisUser.RotBody == 2)
            {
                TargetUser.MoveTo(ThisUser.X + 1, ThisUser.Y);
            }
            else if (ThisUser.RotBody == 4)
            {
                TargetUser.MoveTo(ThisUser.X, ThisUser.Y + 1);
            }
            else if (ThisUser.RotBody == 6)
            {
                TargetUser.MoveTo(ThisUser.X - 1, ThisUser.Y);
            }

            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "*Superpuxar para " + Params[1] + "*", 0, 31));
            return;
        }
    }
}