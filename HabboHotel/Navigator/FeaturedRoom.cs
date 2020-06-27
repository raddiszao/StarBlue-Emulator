﻿namespace StarBlue.HabboHotel.Navigator
{
    public class FeaturedRoom
    {
        public int roomId { get; set; }
        public string Image { get; set; }

        public FeaturedRoom(int roomId, string image)
        {
            this.roomId = roomId;
            Image = image;
        }
    }
}