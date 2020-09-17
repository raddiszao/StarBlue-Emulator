using System;

namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    internal class MutedComposer : MessageComposer
    {
        private double TimeMuted { get; }

        public MutedComposer(double TimeMuted)
            : base(Composers.MutedMessageComposer)
        {
            this.TimeMuted = TimeMuted;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Convert.ToInt32(TimeMuted));
        }
    }
}
