using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    internal class SendUserCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";
        public string Parameters => "[USUARIO] [SALAID]";
        public string Description => "Enviar um usuário para um quarto por id";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            if (Params.Length == 2)
            {
                Session.SendWhisper("Por favor, digite o nome de usuário do usuário que você deseja enviar e o id da sala.");
                return;
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Ocorreu um erro na pesquisa do usuário , pode não estar online.");
                return;
            }


            if (TargetClient.GetHabbo() == null)
            {
                Session.SendWhisper("Ocorreu um erro na pesquisa do usuário , pode não estar online.");
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Consigue uma vida.");
                return;
            }

            if (!int.TryParse(Params[2], out int RoomID))
            {
                Session.SendWhisper("Ocorreu um erro pela busca do usuário, talvez não exista... Lembre-se de usar apenas números para o quarto.");
                return;
            }

            if (TargetClient.GetHabbo().Username == "Raddis")
            {
                Session.SendWhisper("Você não pode enviar esse usuário!", 34);
                return;
            }

            RoomData RoomData = StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(RoomID);
            //TargetClient.SendNotification("Has sido enviado a la sala " + RoomData.Name + "!");
            TargetClient.SendMessage(RoomNotificationComposer.SendBubble("advice", "Você foi enviado para o quarto " + RoomData.Name + "!", ""));
            if (!TargetClient.GetHabbo().InRoom)
            {
                TargetClient.SendMessage(new RoomForwardComposer(RoomID));
            }
            else
            {
                TargetClient.GetHabbo().PrepareRoom(RoomID, "");
            }
        }
    }
}