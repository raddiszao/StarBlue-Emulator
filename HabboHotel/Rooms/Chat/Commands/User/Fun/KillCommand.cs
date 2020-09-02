using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.HabboHotel.GameClients;
using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    internal class KillCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "[USUARIO]";

        public string Description => "Matar o usuário x.x";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Escreva o nome da sua vítima.", 34);
                return;
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);

            if (TargetClient == null)
            {
                Session.SendWhisper("¡Oops!Provavelmente o usuário não está online.", 34);
                return;
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            if (TargetUser == null)
            {
                Session.SendWhisper("¡Oops! Provavelmente o usuário não está no quarto", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Não cometer suicídio! :(", 34);
                return;
            }

            if (TargetClient.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                Session.SendWhisper("Você não pode matar este usuário.", 34);
                return;
            }

            if (TargetUser.isLying || TargetUser.isSitting)
            {
                Session.SendWhisper("Você não pode matá - lo assim...", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == "Raddis")
            {
                Session.SendWhisper("Você não pode matar esse usuário!", 34);
                return;
            }

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
            {
                return;
            }

            if (!((Math.Abs(TargetUser.X - ThisUser.X) > 1) || (Math.Abs(TargetUser.Y - ThisUser.Y) > 1)))
            {
                Room.SendMessage(new ChatComposer(ThisUser.VirtualId, " *Matei " + Params[1] + "*", 0, 32));
                Room.SendMessage(new ChatComposer(TargetUser.VirtualId, "Oh não, eu morri x.x", 0, 32));
                TargetUser.RotBody--;//
                if (!TargetUser.Statusses.ContainsKey("lay"))
                    TargetUser.Statusses.Add("lay", "1.0 null");
                TargetUser.Z -= 0.35;
                TargetUser.isLying = true;
                TargetUser.UpdateNeeded = true;
            }
            else
            {
                Session.SendWhisper("Oops! " + Params[1] + " está muito longe.", 34);
            }
        }
    }
}