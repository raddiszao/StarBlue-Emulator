﻿namespace StarBlue.Communication.Packets.Outgoing.Rooms.Session
{
    class FlatAccessibleComposer : ServerPacket
    {
        public FlatAccessibleComposer(string Username)
            : base(ServerPacketHeader.FlatAccessibleMessageComposer)
        {
            base.WriteString(Username);
        }
    }
}
