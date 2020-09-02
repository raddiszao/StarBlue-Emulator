using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Handshake
{
    public class GetClientVersionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Build = Packet.PopString();

            if (StarBlueServer.SWFRevision != Build)
            {
                Session.Disconnect();
            }
        }
    }
}