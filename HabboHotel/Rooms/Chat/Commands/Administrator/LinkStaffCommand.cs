﻿using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator
{
    internal class LinkStaffCommand : IChatCommand
    {
        public string PermissionRequired => "user_15";
        public string Parameters => "%message%";
        public string Description => "Envia um link ao quarto";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            string Link = Params[1];
            string Message = CommandManager.MergeParams(Params, 2);

            RoomUser actor = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            Room.SendMessage(new ChatComposer(actor.VirtualId, "<font color=\"#2E9AFE\"><a href='" + Link + "' target='_blank'><b>" + Message + "</b></a></font>", 0, 2));
        }
    }
}

