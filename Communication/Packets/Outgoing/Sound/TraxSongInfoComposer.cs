namespace StarBlue.Communication.Packets.Outgoing.Sound
{
    internal class TraxSongInfoComposer : ServerPacket
    {
        public TraxSongInfoComposer()
            : base(ServerPacketHeader.TraxSongInfoMessageComposer)
        {
            base.WriteInteger(0);//Count
            {

            }
        }
    }
}
