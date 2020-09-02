using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.HabboHotel.GameClients;
using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    internal class GolpeCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "[USUARIO]";

        public string Description => "Bater em alguém.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("@green@ Digite o nome do usuário que você deseja bater. { :golpe NOME }", 34);
                return;
            }

            if (!Room.RoomData.GolpeEnabled && !Session.GetHabbo().GetPermissions().HasRight("room_override_custom_config"))
            {
                Session.SendWhisper("Oops, o dono do quarto desativou esta função.", 34);
                return;
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Houve um problema, aparentemente o usuário não está online ou você não escreveu o nome corretamente", 34);
                return;
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (TargetUser == null)
            {
                Session.SendWhisper("@red@ Ocorreu um erro, escreva o nome corretamente, o usuário NÃO está online ou na sala.", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("@red@ Você está louco ou o que há de errado com você ? Maldito!", 34);
                return;
            }

            if (TargetUser.TeleportEnabled)
            {
                Session.SendWhisper("Oops, Você não pode bater em alguém se você estiver usando o teleport.", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == "Raddis")
            {
                Session.SendWhisper("Você não pode bater nesse usuário!", 34);
                return;
            }

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
            {
                return;
            }

            if (!((Math.Abs(TargetUser.X - ThisUser.X) >= 2) || (Math.Abs(TargetUser.Y - ThisUser.Y) >= 2)))
            {
                Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "*" + Params[1] + " recebeu um murro na cara*", 0, 3));
                Room.SendMessage(new ChatComposer(TargetUser.VirtualId, "Que acerto men!", 0, 3));

                if (TargetUser.RotBody == 4)
                {
                    if (!TargetUser.Statusses.ContainsKey("lay"))
                    {
                        TargetUser.Statusses.Add("lay", "1.0 null");
                    }

                    TargetUser.Z -= 0.35;
                    TargetUser.isLying = true;
                    TargetUser.UpdateNeeded = true;
                    TargetUser.ApplyEffect(157);
                }

                if (ThisUser.RotBody == 0)
                {
                    if (!TargetUser.Statusses.ContainsKey("lay"))
                    {
                        TargetUser.Statusses.Add("lay", "1.0 null");
                    }

                    TargetUser.Z -= 0.35;
                    TargetUser.isLying = true;
                    TargetUser.UpdateNeeded = true;
                    TargetUser.ApplyEffect(157);
                }

                if (ThisUser.RotBody == 6)
                {
                    if (!TargetUser.Statusses.ContainsKey("lay"))
                    {
                        TargetUser.Statusses.Add("lay", "1.0 null");
                    }

                    TargetUser.Z -= 0.35;
                    TargetUser.isLying = true;
                    TargetUser.UpdateNeeded = true;
                    TargetUser.ApplyEffect(157);
                }

                if (ThisUser.RotBody == 2)
                {
                    if (!TargetUser.Statusses.ContainsKey("lay"))
                    {
                        TargetUser.Statusses.Add("lay", "1.0 null");
                    }

                    TargetUser.Z -= 0.35;
                    TargetUser.isLying = true;
                    TargetUser.UpdateNeeded = true;
                    TargetUser.ApplyEffect(157);
                }

                if (ThisUser.RotBody == 3)
                {
                    if (!TargetUser.Statusses.ContainsKey("lay"))
                    {
                        TargetUser.Statusses.Add("lay", "1.0 null");
                    }

                    TargetUser.Z -= 0.35;
                    TargetUser.isLying = true;
                    TargetUser.UpdateNeeded = true;
                    TargetUser.ApplyEffect(157);
                }

                if (ThisUser.RotBody == 1)
                {
                    if (!TargetUser.Statusses.ContainsKey("lay"))
                    {
                        TargetUser.Statusses.Add("lay", "1.0 null");
                    }

                    TargetUser.Z -= 0.35;
                    TargetUser.isLying = true;
                    TargetUser.UpdateNeeded = true;
                    TargetUser.ApplyEffect(157);
                }

                if (ThisUser.RotBody == 7)
                {
                    if (!TargetUser.Statusses.ContainsKey("lay"))
                    {
                        TargetUser.Statusses.Add("lay", "1.0 null");
                    }

                    TargetUser.Z -= 0.35;
                    TargetUser.isLying = true;
                    TargetUser.UpdateNeeded = true;
                    TargetUser.ApplyEffect(157);
                }

                if (ThisUser.RotBody == 5)
                {
                    if (!TargetUser.Statusses.ContainsKey("lay"))
                    {
                        TargetUser.Statusses.Add("lay", "1.0 null");
                    }

                    TargetUser.Z -= 0.35;
                    TargetUser.isLying = true;
                    TargetUser.UpdateNeeded = true;
                    TargetUser.ApplyEffect(157);
                }

            }
            else
            {
                Session.SendWhisper("Oops, " + Params[1] + " não está perto o suficiente!", 34);
            }
        }
    }
}
