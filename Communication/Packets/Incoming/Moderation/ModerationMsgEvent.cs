﻿using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Moderation
{
    internal class ModerationMsgEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_alert"))
            {
                return;
            }

            int UserId = Packet.PopInt();
            string Message = Packet.PopString();

            GameClient Client = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(UserId);
            if (Client == null)
            {
                return;
            }

            Client.SendNotification(Message);
        }
    }
}
