﻿namespace StarBlue.HabboHotel.Moderation
{
    internal class ModerationPresetActionCategories
    {
        public int Id { get; set; }
        public string Caption { get; set; }

        public ModerationPresetActionCategories(int Id, string Caption)
        {
            this.Id = Id;
            this.Caption = Caption;
        }
    }
}
