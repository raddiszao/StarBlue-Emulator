namespace StarBlue.Communication.Packets.Outgoing.Rooms.Chat
{
    public class FloodControlComposer : MessageComposer
    {
        private int floodTime { get; }

        public FloodControlComposer(int floodTime)
            : base(Composers.FloodControlMessageComposer)
        {
            this.floodTime = floodTime;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(floodTime);
        }
    }
}