namespace StarBlue.Communication.Packets.Outgoing.GameCenter
{
    internal class GameAccountStatusComposer : ServerPacket
    {
        public GameAccountStatusComposer(int GameID)
            : base(ServerPacketHeader.GameAccountStatusMessageComposer)
        {
            base.WriteInteger(GameID);
            base.WriteInteger(100); // Games Left
            base.WriteInteger(0);//Was 16?
        }
    }
}