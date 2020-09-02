using StarBlue.Communication.Packets.Outgoing.Notifications;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class ViewStaffEventListCommand : IChatCommand
    {
        public string PermissionRequired => "user_15";

        public string Parameters => "";

        public string Description => "Observa uma lista dos eventos abertos por os Staffs.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            Dictionary<Habbo, uint> clients = new Dictionary<Habbo, uint>();

            StringBuilder content = new StringBuilder();
            content.Append("Lista de eventos abertos:\r\n");

            foreach (GameClient client in StarBlueServer.GetGame().GetClientManager()._clients.Values)
            {
                if (client != null && client.GetHabbo() != null && client.GetHabbo().Rank > 5)
                {
                    clients.Add(client.GetHabbo(), (Convert.ToUInt16(client.GetHabbo().Rank)));
                }
            }

            foreach (KeyValuePair<Habbo, uint> client in clients.OrderBy(key => key.Value))
            {
                if (client.Key == null)
                {
                    continue;
                }

                content.Append("¥ " + client.Key.Username + " [Rank: " + client.Key.Rank + "] - Abriu : " + client.Key._eventsopened + " eventos.\r\n");
            }

            Session.SendMessage(new MOTDNotificationComposer(content.ToString()));

            return;
        }
    }
}