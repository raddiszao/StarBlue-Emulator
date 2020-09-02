using StarBlue.HabboHotel.GameClients;
using System.Drawing;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class TeleportToMeCommand : IChatCommand
    {
        public string PermissionRequired => "user_1";
        public string Parameters => "[USUARIO]";
        public string Description => "Teleporta um usuário até você.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, false, true))
            {
                Session.SendWhisper("Oops, somente pessoas com direitos podem usar este comando.", 34);
                return;
            }

            if (Params.Length != 2)
            {
                Session.SendWhisper("Por favor, insira um nome de usuário.");
                return;
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Há um erro, o usuário não está online", 34);
                return;
            }

            if (TargetClient.GetRoomUser() == null)
            {
                Session.SendWhisper("Há um erro, o usuário não está em nenhum quarto", 34);
                return;
            }

            if (Session.GetRoomUser() == null)
            {
                return;
            }

            if (Room.GetGameMap().ValidTile(Session.GetRoomUser().X, Session.GetRoomUser().Y))
            {
                Room.SendMessage(Room.GetRoomItemHandler().UpdateUserOnRoller(TargetClient.GetRoomUser(), new Point(Session.GetRoomUser().X, Session.GetRoomUser().Y), 0, Room.GetGameMap().SqAbsoluteHeight(Session.GetRoomUser().X, Session.GetRoomUser().Y)));
                TargetClient.GetRoomUser().UpdateNeeded = true;
            }
        }
    }
}
