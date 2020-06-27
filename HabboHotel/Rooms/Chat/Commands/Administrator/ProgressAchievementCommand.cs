using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class ProgressAchievementCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_17"; }
        }

        public string Parameters
        {
            get { return "<usuario> <conquista> < pontos>"; }
        }

        public string Description
        {
            get { return "Altera os conquista de um usuário."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length != 4)
            {
                Session.SendWhisper("Introduza o nome do usuário que deseja alterar!");
                return;
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient != null)
            {
                if (StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(TargetClient, "ACH_" + Params[2], int.Parse(Params[3])))
                {
                    Session.SendMessage(RoomNotificationComposer.SendBubble("definitions", "Avançou " + Params[2] + " a " + TargetClient.GetHabbo().Username + " " + Params[3] + " puntos.", ""));
                }
                else { Session.SendWhisper("Não deu certo, tente: ACH = " + Params[2] + " PROGRESO = " + Params[3] + "."); }
            }
        }
    }
}
