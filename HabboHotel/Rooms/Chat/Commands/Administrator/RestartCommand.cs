using StarBlue.Communication.Packets.Outgoing.Moderation;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator
{
    internal class RestartCommand : IChatCommand
    {
        public string PermissionRequired => "user_18";

        public string Parameters => "";

        public string Description => "Reinicia o Hotel";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            StarBlueServer.GetGame().GetClientManager().SendMessage(new BroadcastMessageAlertComposer("<b><font color=\"#ba3733\" size=\"14\">VOLTAMOS LOGO!</font></b><br><br>O hotel será reiniciado nesse instante para aplicarmos atualizações, voltaremos em minutos!"));

            StarBlueServer.PerformRestart();
        }
    }
}