using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class RestartCommand : IChatCommand
    {
        public string PermissionRequired => "user_18";

        public string Parameters => "";

        public string Description => "Reinicia o Hotel";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            StarBlueServer.GetGame().GetClientManager().SendMessage(new RoomCustomizedAlertComposer(StarBlueServer.HotelName + " fará um reinício rápido, para aplicar todas as atualizações.\n\nVoltaremos em seguida :)\n\n - " + Session.GetHabbo().Username + ""));

            StarBlueServer.PerformRestart();
        }
    }
}