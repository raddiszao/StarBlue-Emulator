
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Games;

namespace StarBlue.Communication.Packets.Incoming.GameCenter
{
    internal class UnknownGameCenterEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int GameId = Packet.PopInt();
            int UserId = Packet.PopInt();

            if (StarBlueServer.GetGame().GetGameDataManager().TryGetGame(GameId, out GameData GameData))
            {
                // Session.SendMessage(new Game2WeeklyLeaderboardComposer(GameId)); Comentado y funciona
            }
        }
    }
}
