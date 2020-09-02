using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using StarBlue.HabboHotel.GameClients;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    internal class SummonAllCommand : IChatCommand
    {
        public string PermissionRequired => "user_15";
        public string Parameters => "";
        public string Description => "Trazer todos os usuários.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            foreach (GameClient Client in StarBlueServer.GetGame().GetClientManager().GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().Username == Session.GetHabbo().Username)
                {
                    continue;
                }

                if (Client.GetHabbo().InRoom && Client.GetHabbo().CurrentRoomId != Session.GetHabbo().CurrentRoomId)
                {
                    Client.SendMessage(new RoomForwardComposer(Session.GetHabbo().CurrentRoomId));
                    Client.SendMessage(RoomNotificationComposer.SendBubble("voado", "Você foi puxado por " + Session.GetHabbo().Username + "!", ""));
                }
                else if (!Client.GetHabbo().InRoom)
                {
                    Client.SendMessage(new RoomForwardComposer(Session.GetHabbo().CurrentRoomId));
                    Client.SendMessage(RoomNotificationComposer.SendBubble("voado", "Você foi puxado por " + Session.GetHabbo().Username + "!", ""));
                }
                else if (Client.GetHabbo().CurrentRoomId == Session.GetHabbo().CurrentRoomId)
                {
                    Client.SendWhisper("Uau, parece que você acabou de trazer todo o hotel na sala onde você está...", 34);
                }
            }

            Session.SendWhisper("Você acabou de puxar todos do hotel.");


        }
    }
}
