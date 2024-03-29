﻿using System.Collections.Generic;

namespace StarBlue.HabboHotel.Rooms.Chat.Moderation
{
    internal class ModerationRoomChatLog
    {
        public int UserId { get; set; }
        public List<string> Chat { get; set; }

        public ModerationRoomChatLog(int UserId, List<string> Chat)
        {
            this.UserId = UserId;
            this.Chat = Chat;
        }
    }
}
