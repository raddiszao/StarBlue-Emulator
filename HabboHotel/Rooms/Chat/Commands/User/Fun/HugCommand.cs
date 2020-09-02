using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.HabboHotel.GameClients;
using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    internal class HugCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";
        public string Parameters => "[USUARIO]";
        public string Description => "Abraçar um usuário.";
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Você deve digitar um nome de usuário!", 34);
                return;
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Este usuário não pode ser encontrado, talvez ele não esteja online ou não esteja no quarto.", 34);
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (User == null)
            {
                Session.SendWhisper("Este usuário não pode ser encontrado, talvez ele não esteja online ou não esteja no quarto.", 34);
                return;
            }
            RoomUser Self = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == Self)
            {
                Session.SendWhisper("Você não pode peidar em seu próprio rosto!", 34);
                return;
            }
            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
            {
                return;
            }

            if (Math.Abs(User.X - ThisUser.X) < 2 && Math.Abs(User.Y - ThisUser.Y) < 2)
            {
                Room.SendMessage(new ShoutComposer(ThisUser.VirtualId, "*Eu abracei " + TargetClient.GetHabbo().Username + "*", 0, 5));
                Room.SendMessage(new ShoutComposer(User.VirtualId, "*-*", 0, 5));
                ThisUser.ApplyEffect(4);
                User.ApplyEffect(4);
            }
            else
            {
                Session.SendWhisper("Este usuário está longe demais.", 34);
                return;
            }
        }
    }
}
