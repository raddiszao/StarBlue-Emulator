using StarBlue.Communication.Packets.Outgoing.Notifications;
using StarBlue.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class ViewOnlineCommand : IChatCommand
    {
        public string PermissionRequired => "user_3";
        public string Parameters => "";
        public string Description => "Ver os usuários online.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Dictionary<Habbo, UInt32> clients = new Dictionary<Habbo, UInt32>();

            StringBuilder content = new StringBuilder();
            content.Append("- LISTA DE USUÁRIOS ONLINE -\r\n");

            foreach (var client in StarBlueServer.GetGame().GetClientManager().GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                {
                    continue;
                }

                content.Append("¥ " + client.GetHabbo().Username + " » Se encontra no quarto: " + (client.GetHabbo().CurrentRoom == null ? "Em nenhum quarto." : client.GetHabbo().CurrentRoom.Name) + "\r\n");
            }

            Session.SendMessage(new MOTDNotificationComposer(content.ToString()));
            return;
        }
    }
}
