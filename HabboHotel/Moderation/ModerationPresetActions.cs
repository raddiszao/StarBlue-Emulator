﻿namespace StarBlue.HabboHotel.Moderation
{
    public class ModerationPresetActions
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Type { get; set; }
        public string Caption { get; set; }
        public string MessageText { get; set; }
        public int MuteTime { get; set; }
        public int BanTime { get; set; }
        public int IPBanTime { get; set; }
        public int TradeLockTime { get; set; }
        public string DefaultSanction { get; set; }

        public ModerationPresetActions(int id, int parentId, string type, string caption, string messageText, int muteText, int banTime, int ipBanTime, int tradeLockTime, string defaultSanction)
        {
            Id = id;
            ParentId = parentId;
            Type = type;
            Caption = caption;
            MessageText = messageText;
            MuteTime = muteText;
            BanTime = banTime;
            IPBanTime = ipBanTime;
            TradeLockTime = tradeLockTime;
            DefaultSanction = defaultSanction;
        }
    }
}
