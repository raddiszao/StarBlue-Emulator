﻿using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.GameCenter
{
    class UnknownGameCenterEvent4 : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int pop = Packet.PopInt();
        }
    }
}