using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class ControlCommand : IChatCommand
    {
        public string PermissionRequired => "user_16";

        public string Parameters => "<usuario>";

        public string Description => "Controla o usuário que escolher.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length != 2)
            {
                Session.SendWhisper("Introduza o nome do usuário que deseja controlar!");
                return;
            }

            if (Params.Length == 2 && Params[1] == "end")
            {
                Session.SendWhisper("Deixou de controlar o " + Session.GetHabbo().Opponent + ".");
                Session.GetHabbo().isControlling = false;
                return;
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient != null)
            {
                Session.GetHabbo().Opponent = TargetClient.GetHabbo().Username;
                Session.GetHabbo().isControlling = true;
                Session.SendMessage(RoomNotificationComposer.SendBubble("definitions", "Você agora controla o/a " + TargetClient.GetHabbo().Username + ". Para parar diga :controlar end."));
                return;
            }

            else
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("definitions", "Não encontramos o usuário " + Params[1] + ".", ""));
            }
        }
    }
}
