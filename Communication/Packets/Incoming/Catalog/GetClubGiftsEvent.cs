﻿using StarBlue.Communication.Packets.Outgoing.Catalog;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    internal class GetClubGiftsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {

            Session.SendMessage(new ClubGiftsComposer());
        }
    }
}
