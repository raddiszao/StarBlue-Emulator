using StarBlue.Communication.Packets.Outgoing.Notifications;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class StaffInfoCommand : IChatCommand
    {
        public string PermissionRequired => "user_14";
        public string Parameters => "";
        public string Description => "Lista com todos os staffs onlines.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            Dictionary<Habbo, UInt32> clients = new Dictionary<Habbo, UInt32>();

            StringBuilder content = new StringBuilder();
            content.Append("Staffs conectados no " + StarBlueServer.HotelName + ":\r\n");

            foreach (var client in StarBlueServer.GetGame().GetClientManager()._clients.Values)
            {
                if (client != null && client.GetHabbo() != null && client.GetHabbo().Rank > 3)
                {
                    clients.Add(client.GetHabbo(), (Convert.ToUInt16(client.GetHabbo().Rank)));
                }
            }

            foreach (KeyValuePair<Habbo, UInt32> client in clients.OrderBy(key => key.Value))
            {
                if (client.Key == null)
                {
                    continue;
                }

                content.Append("¥ " + client.Key.Username + " [Rank: " + client.Key.Rank + "] - Se encontra na sala: " + ((client.Key.CurrentRoom == null) ? "Em nenhuma sala." : client.Key.CurrentRoom.RoomData.Name) + "\r\n");
            }

            Session.SendMessage(new MOTDNotificationComposer(content.ToString()));

            return;
        }
    }
}