using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class MassiveEventCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_13"; }
        }

        public string Parameters
        {
            get { return "[EVENTO]"; }
        }

        public string Description
        {
            get { return "Envia um evento a todos os usuarios online."; }
        }

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
