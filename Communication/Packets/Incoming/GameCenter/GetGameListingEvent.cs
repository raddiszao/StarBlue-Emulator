using StarBlue.Communication.Packets.Outgoing.GameCenter;
using StarBlue.HabboHotel.Games;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.GameCenter
{
    class GetGameListingEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            ICollection<GameData> Games = StarBlueServer.GetGame().GetGameDataManager().GameData;

            Session.SendMessage(new GameListComposer(Games));
        }
    }
}
