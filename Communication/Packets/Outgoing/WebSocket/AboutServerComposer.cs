﻿using StarBlue.Communication.WebSocket;

namespace StarBlue.Communication.Packets.Outgoing.WebSocket
{
    internal class AboutServerComposer : WebComposer
    {
        public AboutServerComposer(int onlines, int rooms, string lastUpdate, string upTime) : base(6)
        {
            base.WriteInteger(onlines);
            base.WriteInteger(rooms);
            base.WriteString(lastUpdate);
            base.WriteString(upTime);
        }
    }
}