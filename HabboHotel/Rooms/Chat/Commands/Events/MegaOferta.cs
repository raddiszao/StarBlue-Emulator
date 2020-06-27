using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class MegaOfertCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_16"; }
        }

        public string Parameters
        {
            get { return "[ON] ou [OFF]"; }
        }

        public string Description
        {
            get { return "Cria e deleta uma MegaOfertCommand."; ; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("advice", "Ops, você deve esrever assim: ':megaoferta on ou :megaoferta off'!", ""));
                return;
            }

            if (Params[1] == "on" || Params[1] == "ON")
            {
                // Comando editaveu abaixo mais cuidado pra não faze merda
                using (var dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("UPDATE targeted_offers SET active = 'true' WHERE active = 'false'");
                    dbClient.RunFastQuery("UPDATE users SET targeted_buy = '0'");
                }
                StarBlueServer.GetGame().GetTargetedOffersManager().Initialize(StarBlueServer.GetDatabaseManager().GetQueryReactor());
                StarBlueServer.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("volada", "Corre, uma nova MegaOfertCommand foi lançada!", ""));
                Session.SendWhisper("Nova mega oferta iniciada!");
            }

            if (Params[1] == "off" || Params[1] == "OFF")
            {
                // Comando editaveu abaixo mais cuidado pra não faze merda
                using (var dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("UPDATE targeted_offers SET active = 'false' WHERE active = 'true'");
                    dbClient.RunFastQuery("UPDATE users SET targeted_buy = '0'");
                }
                StarBlueServer.GetGame().GetTargetedOffersManager().Initialize(StarBlueServer.GetDatabaseManager().GetQueryReactor());
                StarBlueServer.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("ADM", "Que pena, a MegaOfertCommand foi removida!", ""));
                Session.SendWhisper("Mega oferta retirada!");
            }

            if (Params[1] != "on" || Params[1] != "off")
            {
                //Session.SendMessage(new RoomNotificationComposer("erro", "message", "Ops, usted debe teclear así: ':megaoferta on o :megaoferta off'!"));
                Session.SendMessage(RoomNotificationComposer.SendBubble("advice", "Ops, você deve escrever assim: ':megaoferta on o :megaoferta off'!", ""));

            }
        }
    }
}
