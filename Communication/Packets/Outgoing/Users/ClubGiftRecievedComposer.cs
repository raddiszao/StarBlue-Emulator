using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Outgoing.Users
{
    internal class ClubGiftRecievedComposer : ServerPacket
    {
        public ClubGiftRecievedComposer(GameClient Session) : base(ServerPacketHeader.ClubGiftRecievedComposer)
        {
            base.WriteString("PENE");
            base.WriteInteger(1);
            base.WriteString("b"); // tipo de furni
            base.WriteString("ADMIN"); // nombre
        }
    }
}