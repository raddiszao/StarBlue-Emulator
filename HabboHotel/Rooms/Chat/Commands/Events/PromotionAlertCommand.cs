using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Events
{
    internal class PromotionAlertCommand : IChatCommand
    {
        public string PermissionRequired => "user_15";
        public string Parameters => "";
        public string Description => "Envia alerta com um botão para o quarto.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            string Message = CommandManager.MergeParams(Params, 1);

            foreach (GameClient Client in StarBlueServer.GetGame().GetClientManager().GetClients)
            {
                Client.SendMessage(new RoomNotificationComposer("Alerta do " + StarBlueServer.HotelName + " Hotel", Message, "", "Ir ao quarto " + Room.RoomData.Name, "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
            }
        }
    }
}

