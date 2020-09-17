namespace StarBlue.Communication.Packets.Outgoing.Sound
{
    internal class TraxSongInfoComposer : MessageComposer
    {
        public TraxSongInfoComposer()
            : base(Composers.TraxSongInfoMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(0);//Count
            {

            }
        }
    }
}
