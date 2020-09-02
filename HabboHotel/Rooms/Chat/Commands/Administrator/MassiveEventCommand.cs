using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class MassiveEventCommand : IChatCommand
    {
        public string PermissionRequired => "user_15";

        public string Parameters => "[EVENTO]";

        public string Description => "Envia um evento a todos os usuarios online.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor escreva o evento a realizar.");
                return;
            }

            string Event = CommandManager.MergeParams(Params, 1);
            StarBlueServer.GetGame().GetClientManager().SendMessage(new MassEventComposer(Event));
            return;
        }
    }
}
