using StarBlue.Communication.Packets.Outgoing.Sound;

namespace StarBlue.Communication.Packets.Incoming.Sound
{
    internal class GetSongInfoEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new TraxSongInfoComposer());
        }
    }
}
