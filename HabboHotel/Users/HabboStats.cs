﻿namespace StarBlue.HabboHotel.Users
{
    public class HabboStats
    {
        public int WelcomeLevel { get; set; }
        public int RoomVisits { get; set; }
        public double OnlineTime { get; set; }
        public int Respect { get; set; }
        public int RespectGiven { get; set; }
        public int GiftsGiven { get; set; }
        public int GiftsReceived { get; set; }
        public int DailyRespectPoints { get; set; }
        public int DailyPetRespectPoints { get; set; }
        public int AchievementPoints { get; set; }
        public int QuestID { get; set; }
        public int QuestProgress { get; set; }
        public int FavouriteGroupId { get; set; }
        public string RespectsTimestamp { get; set; }
        public int ForumPosts { get; set; }
        public bool PurchaseUsersConcurrent { get; set; }
        public int vipGifts { get; set; }

        public HabboStats(int roomVisits, double onlineTime, int Respect, int respectGiven, int giftsGiven, int giftsReceived, int dailyRespectPoints, int dailyPetRespectPoints, int achievementPoints, int questID, int questProgress, int groupID, string RespectsTimestamp, int ForumPosts, bool PurchaseUsersConcurrent, int vipGifts)
        {
            RoomVisits = roomVisits;
            OnlineTime = onlineTime;
            this.Respect = Respect;
            RespectGiven = respectGiven;
            GiftsGiven = giftsGiven;
            GiftsReceived = giftsReceived;
            DailyRespectPoints = dailyRespectPoints;
            DailyPetRespectPoints = dailyPetRespectPoints;
            AchievementPoints = achievementPoints;
            QuestID = questID;
            QuestProgress = questProgress;
            FavouriteGroupId = groupID;
            this.RespectsTimestamp = RespectsTimestamp;
            this.ForumPosts = ForumPosts;
            this.PurchaseUsersConcurrent = PurchaseUsersConcurrent;
            WelcomeLevel = 0;
            this.vipGifts = vipGifts;
        }
    }
}
