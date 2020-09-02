using System;

namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    internal class MutedComposer : ServerPacket
    {
        public MutedComposer(double TimeMuted)
            : base(ServerPacketHeader.MutedMessageComposer)
        {
            base.WriteInteger(Convert.ToInt32(TimeMuted));
        }
    }
}
