﻿using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;
using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class ViewVIPStatusCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_info"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Informações do seu VIP " + Convert.ToString(StarBlueServer.GetConfig().data["hotel.name"]) + "."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            Session.SendMessage(RoomNotificationComposer.SendBubble("abuse", "Você não é membro VIP do " + Convert.ToString(StarBlueServer.GetConfig().data["hotel.name"]) + ", clique aqui para voltar.", ""));
        }
    }
}