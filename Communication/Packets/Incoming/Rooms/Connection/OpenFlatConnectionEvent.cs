﻿using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items.Wired;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Connection
{
    public class OpenFlatConnectionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            int RoomId = Packet.PopInt();
            string Password = Packet.PopString();

            Session.GetHabbo().PrepareRoom(RoomId, Password);

        }
    }
}