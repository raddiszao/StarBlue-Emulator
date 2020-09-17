
using StarBlue.Communication.Encryption;
using StarBlue.Communication.Packets.Outgoing.Handshake;
using StarBlue.HabboHotel.GameClients;
using StarBlue.Network.Codec;

namespace StarBlue.Communication.Packets.Incoming.Handshake
{
    public class GenerateSecretKeyEvent : IPacketEvent
    {
        public void Parse(GameClient session, MessageEvent packet)
        {
            string cipherPublickey = packet.PopString();

            BigInteger sharedKey = HabboEncryptionV2.CalculateDiffieHellmanSharedKey(cipherPublickey);
            if (sharedKey != 0)
            {
                session.SendMessage(new SecretKeyComposer(HabboEncryptionV2.GetRsaDiffieHellmanPublicKey()));
                session.GetChannel().Channel.Pipeline.AddFirst("gameCrypto", new EncryptionDecoder(sharedKey.getBytes()));
            }
            else
            {
                session.SendNotification("There was an error logging you in, please try again!");
            }
        }
    }
}