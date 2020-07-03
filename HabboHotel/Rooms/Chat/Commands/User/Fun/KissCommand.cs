using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.HabboHotel.GameClients;
using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class KissCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "[USUARIO]";

        public string Description => "Beijar alguém da sala.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("@green@ Introduza o nome do usuário que você deseja beijar. { :beijar NOME }", 34);
                return;
            }

            //if (!Room.BesosEnabled && !Session.GetHabbo().GetPermissions().HasRight("room_override_custom_config"))
            //{
            //    Session.SendWhisper("@red@ Oops, el dueño de la sala no permite que des besos a otros en su sala.");
            //    return;
            //}

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Ocorreu um erro, parace que o usuário não se encontra online ou você escreveu o nome errado", 34);
                return;
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (TargetUser == null)
            {
                Session.SendWhisper("Ocorreu um erro, escreva corretamente o nome, o usuario não se encontra online ou no quarto", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Não é ruim que você tente se beijar... mas pode paracer estranho.", 34);
                return;
            }

            if (TargetUser.TeleportEnabled)
            {
                Session.SendWhisper("Oops, Você não pode beijar alguém se você usar o teleport.", 34);
                return;
            }

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
            {
                return;
            }

            if (!((Math.Abs(TargetUser.X - ThisUser.X) >= 2) || (Math.Abs(TargetUser.Y - ThisUser.Y) >= 2)))
            {
                Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "*Eu beijei " + Params[1] + "*", 0, ThisUser.LastBubble));
                ThisUser.ApplyEffect(9);
                Room.SendMessage(new ChatComposer(TargetUser.VirtualId, "Oh wow... me deram um beijo :$", 0, ThisUser.LastBubble));
                TargetUser.ApplyEffect(9);
            }
            else
            {
                Session.SendWhisper("Oops, " + Params[1] + "não está perto o suficiente!", 34);
            }
        }
    }
}
