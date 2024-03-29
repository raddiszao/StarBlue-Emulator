﻿using StarBlue.Communication.Packets.Outgoing.GameCenter;
using StarBlue.HabboHotel.Games;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.GameCenter
{
    internal class GetGameListingEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            ICollection<GameData> Games = StarBlueServer.GetGame().GetGameDataManager().GameData;

            Session.SendMessage(new GameListComposer(Games));
        }
    }
}
