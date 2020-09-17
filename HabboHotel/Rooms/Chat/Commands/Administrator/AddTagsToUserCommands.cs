using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Communication.Packets.Outgoing.Users;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class AddTagsToUserCommand : IChatCommand
    {
        public string PermissionRequired => "user_16";

        public string Parameters => "<usuario> <tag>";

        public string Description => "Adiciona TAGS a um usuário.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length != 3)
            {
                Session.SendWhisper("Insira o nome de usuário a quem deseja enviar!");
                return;
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient != null)
            {
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("INSERT INTO `user_tags` (user_id, tag_name) VALUES(" + TargetClient.GetHabbo().Id + ", '" + Params[2] + "')");
                    TargetClient.GetHabbo().Tags.Add(Params[2]);
                }

                Session.SendMessage(RoomNotificationComposer.SendBubble("definitions", "Adicionou \"" + Params[2] + "\" a " + TargetClient.GetHabbo().Username + " satisfatoriamente.", ""));
                TargetClient.SendMessage(RoomNotificationComposer.SendBubble("definitions", Session.GetHabbo().Username + " te deu a TAG " + Params[2] + ".", ""));

                foreach (RoomUser RoomUser in Room.GetRoomUserManager().GetRoomUsers())
                {

                    RoomUser.GetClient().SendMessage(new UserTagsComposer(TargetClient.GetHabbo().Id));
                }
            }
        }
    }
}
