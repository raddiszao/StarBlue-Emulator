﻿using StarBlue.Communication.WebSocket;

namespace StarBlue.Communication.Packets.Outgoing.WebSocket
{
    internal class HotelAlertComposer : WebComposer
    {
        public HotelAlertComposer(string author, string authorLook, string message, string noticeLink) : base(11)
        {
            base.WriteString(author);
            base.WriteString(authorLook);
            base.WriteString(message);
            base.WriteString(noticeLink);
        }
    }
}