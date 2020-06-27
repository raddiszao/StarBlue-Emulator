using System;

namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    class MutedComposer : ServerPacket
    {
        public MutedComposer(Double TimeMuted)
            : base(ServerPacketHeader.MutedMessageComposer)
        {
            base.WriteInteger(Convert.ToInt32(TimeMuted));
        }
    }
}
