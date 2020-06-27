using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Handshake

{
    public class SSOTicketEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.RC4Client == null || Session.GetHabbo() != null)
            {
                return;
            }

            string SSO = Packet.PopString();

            if (string.IsNullOrEmpty(SSO))
            {
                return;
            }

            Session.TryAuthenticate(SSO);
        }
    }
}