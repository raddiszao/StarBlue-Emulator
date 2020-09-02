using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.HabboHotel.GameClients;
using System;
using System.Threading;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    internal class CutHeadCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";
        public string Parameters => "[USUARIO]";
        public string Description => "Cortar a cabeça de um usuário.";
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
                Session.SendWhisper("Esse usuário não pode ser encontrado, talvez ele não esteja online ou não esteja no quarto.", 34);
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (User == null)
            {
                Session.SendWhisper("Esse usuário não pode ser encontrado, talvez ele não esteja online ou não esteja no quarto.", 34);
                return;
            }
            RoomUser Self = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == Self)
            {
                Session.SendWhisper("Você não poder cortar a cabeça!", 34);
                return;
            }
            if (TargetClient.GetHabbo().Username == "Raddis")
            {
                Session.SendWhisper("Ele é seu patrão!", 34);
                return;
            }
            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
            {
                return;
            }

            if (Math.Abs(User.X - ThisUser.X) < 2 && Math.Abs(User.Y - ThisUser.Y) < 2)
            {
                Room.SendMessage(new ShoutComposer(ThisUser.VirtualId, "*Cortar a cabeça de " + TargetClient.GetHabbo().Username + "*", 0, 32));
                Room.SendMessage(new ChatComposer(User.VirtualId, "*Morrendo*", 0, 32));

                ThisUser.ApplyEffect(117);
                User.ApplyEffect(93);
                TargetClient.SendMessage(new FloodControlComposer(3));
                if (User != null)
                {
                    User.Frozen = true;
                }

                System.Threading.Thread thrd = new System.Threading.Thread(delegate ()
                {
                    Thread.Sleep(4000);
                    if (User != null)
                    {
                        User.Frozen = false;
                    }

                    User.ApplyEffect(93);
                    Thread.Sleep(2000);
                    ThisUser.ApplyEffect(117);
                    User.ApplyEffect(93);
                });
                thrd.Start();
            }
            else
            {
                Session.SendWhisper("Esse usuário está muito longe, tente se aproximar.", 34);
                return;
            }
        }
    }
}
