﻿namespace StarBlue.Communication.Packets.Outgoing.Rooms.Chat
{
    public class WhisperComposer : ServerPacket
    {
        public WhisperComposer(int VirtualId, string Text, int Emotion, int Colour)
            : base(ServerPacketHeader.WhisperMessageComposer)
        {
            base.WriteInteger(VirtualId);
            base.WriteString(Text);
            base.WriteInteger(Emotion);
            base.WriteInteger(Colour);

            base.WriteInteger(0);
            base.WriteInteger(-1);
        }
    }
}