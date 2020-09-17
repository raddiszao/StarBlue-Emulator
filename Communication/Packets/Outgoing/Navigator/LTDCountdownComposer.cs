using System;

namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class LTDCountdownComposer : MessageComposer
    {
        private string time { get; }
        private TimeSpan diff { get; }

        public LTDCountdownComposer(string time, TimeSpan diff)
            : base(Composers.FlatCreatedMessageComposer)
        {
            this.time = time;
            this.diff = diff;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(time);
            packet.WriteInteger(Convert.ToInt32(diff.TotalSeconds));
        }
    }
}
