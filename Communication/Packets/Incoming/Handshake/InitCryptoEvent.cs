
using StarBlue.Communication.Encryption;
using StarBlue.Communication.Packets.Outgoing.Handshake;

namespace StarBlue.Communication.Packets.Incoming.Handshake
{
    public class InitCryptoEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new InitCryptoComposer(HabboEncryptionV2.GetRsaDiffieHellmanPrimeKey(), HabboEncryptionV2.GetRsaDiffieHellmanGeneratorKey()));
        }
    }
}