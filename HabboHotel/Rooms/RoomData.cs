using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Groups;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace StarBlue.HabboHotel.Rooms
{
    public class RoomData
    {
        public int Id;
        public int AllowPets;
        public int AllowPetsEating;
        public int RoomBlockingEnabled;
        public int Category;
        public string Description;
        public string Floor;
        public int FloorThickness;
        public Group Group;
        public int Hidewall;
        public string Landscape;
        public string ModelName;
        public string Name;
        public string OwnerName;
        public int OwnerId;
        public string Password;
        public int Score;
        public RoomAccess Access;
        public List<string> Tags;
        public string Type;
        public int UsersMax;
        public int UsersNow;
        public int WallThickness;
        public string Wallpaper;
        public int WhoCanBan;
        public int WhoCanKick;
        public int WhoCanMute;
        private RoomModel mModel;
        public int chatMode;
        public int chatSpeed;
        public int chatSize;
        public int extraFlood;
        public int chatDistance;
        public int RollerSpeed;

        public Dictionary<int, KeyValuePair<int, string>> WiredScoreBordDay;
        public Dictionary<int, KeyValuePair<int, string>> WiredCasinoApuestas;
        public Dictionary<int, KeyValuePair<int, string>> WiredScoreBordWeek;
        public Dictionary<int, KeyValuePair<int, string>> WiredScoreBordMonth;
        public List<int> WiredScoreFirstBordInformation = new List<int>();

        // public List<Habbo> QueueingUsers = new List<Habbo>();

        public int TradeSettings;

        public RoomPromotion _promotion;

        public bool GolpeEnabled;
        public bool SexEnabled;
        public bool FastWalkEnabled;
        public bool PushEnabled;
        public bool PullEnabled;
        public bool SPushEnabled;
        public bool SPullEnabled;
        public bool EnablesEnabled;
        public bool HideWired;
        public bool DiagonalEnabled;
        public bool HandItemEnabled;
        public bool RespectNotificationsEnabled;
        public bool PetMorphsAllowed;
        public bool RoomMuted;
        public int Shoot;

        public void Fill(DataRow Row)
        {
            Id = Convert.ToInt32(Row["id"]);
            Name = Convert.ToString(Row["caption"]);
            Description = Convert.ToString(Row["description"]);
            Type = Convert.ToString(Row["roomtype"]);
            OwnerId = Convert.ToInt32(Row["owner"]);

            OwnerName = "";
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `username` FROM `users` WHERE `id` = @owner LIMIT 1");
                dbClient.AddParameter("owner", OwnerId);
                string result = dbClient.GetString();
                if (!string.IsNullOrEmpty(result))
                {
                    OwnerName = result;
                }
            }

            Access = RoomAccessUtility.ToRoomAccess(Row["state"].ToString().ToLower());

            Category = Convert.ToInt32(Row["category"]);
            if (!string.IsNullOrEmpty(Row["users_now"].ToString()))
            {
                UsersNow = Convert.ToInt32(Row["users_now"]);
            }
            else
            {
                UsersNow = 0;
            }

            UsersMax = Convert.ToInt32(Row["users_max"]);
            ModelName = Convert.ToString(Row["model_name"]);
            Score = Convert.ToInt32(Row["score"]);
            Tags = new List<string>();
            AllowPets = Convert.ToInt32(Row["allow_pets"].ToString());
            AllowPetsEating = Convert.ToInt32(Row["allow_pets_eat"].ToString());
            RoomBlockingEnabled = Convert.ToInt32(Row["room_blocking_disabled"].ToString());
            Hidewall = Convert.ToInt32(Row["allow_hidewall"].ToString());
            Password = Convert.ToString(Row["password"]);
            Wallpaper = Convert.ToString(Row["wallpaper"]);
            Floor = Convert.ToString(Row["floor"]);
            Landscape = Convert.ToString(Row["landscape"]);
            FloorThickness = Convert.ToInt32(Row["floorthick"]);
            WallThickness = Convert.ToInt32(Row["wallthick"]);
            WhoCanMute = Convert.ToInt32(Row["mute_settings"]);
            WhoCanKick = Convert.ToInt32(Row["kick_settings"]);
            WhoCanBan = Convert.ToInt32(Row["ban_settings"]);
            chatMode = Convert.ToInt32(Row["chat_mode"]);
            chatSpeed = Convert.ToInt32(Row["chat_speed"]);
            chatSize = Convert.ToInt32(Row["chat_size"]);
            TradeSettings = Convert.ToInt32(Row["trade_settings"]);
            RollerSpeed = Convert.ToInt32(Row["roller_speed"]);
            RoomMuted = false;

            if (StarBlueServer.GetGame().GetGroupManager().TryGetGroup(Convert.ToInt32(Row["group_id"]), out Group G))
            {
                Group = G;
            }
            else
            {
                Group = null;
            }

            foreach (string Tag in Row["tags"].ToString().Split(','))
            {
                Tags.Add(Tag);
            }

            mModel = StarBlueServer.GetGame().GetRoomManager().GetModel(ModelName);

            PushEnabled = StarBlueServer.EnumToBool(Row["push_enabled"].ToString());
            FastWalkEnabled = StarBlueServer.EnumToBool(Row["fastwalk_enabled"].ToString());
            GolpeEnabled = StarBlueServer.EnumToBool(Row["golpe_enabled"].ToString());
            SexEnabled = StarBlueServer.EnumToBool(Row["sex_enabled"].ToString());
            PullEnabled = StarBlueServer.EnumToBool(Row["pull_enabled"].ToString());
            SPushEnabled = StarBlueServer.EnumToBool(Row["spush_enabled"].ToString());
            SPullEnabled = StarBlueServer.EnumToBool(Row["spull_enabled"].ToString());
            EnablesEnabled = StarBlueServer.EnumToBool(Row["enables_enabled"].ToString());
            HideWired = StarBlueServer.EnumToBool(Row["hide_wired"].ToString());
            DiagonalEnabled = StarBlueServer.EnumToBool(Row["diagonal_enabled"].ToString());
            HandItemEnabled = StarBlueServer.EnumToBool(Row["handitem_enabled"].ToString());
            RespectNotificationsEnabled = StarBlueServer.EnumToBool(Row["respect_notifications_enabled"].ToString());
            PetMorphsAllowed = StarBlueServer.EnumToBool(Row["pet_morphs_allowed"].ToString());
            Shoot = Convert.ToInt32(Row["shoot"]);

            WiredScoreBordDay = new Dictionary<int, KeyValuePair<int, string>>();
            WiredScoreBordWeek = new Dictionary<int, KeyValuePair<int, string>>();
            WiredScoreBordMonth = new Dictionary<int, KeyValuePair<int, string>>();

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                List<bool> SuperCheck = new List<bool>()
                {
                    false,
                    false,
                    false
                };

                DateTime now = DateTime.Now;
                int getdaytoday = Convert.ToInt32(now.ToString("MMddyyyy"));
                int getmonthtoday = Convert.ToInt32(now.ToString("MM"));
                int getweektoday = CultureInfo.GetCultureInfo("Nl-nl").Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

                WiredScoreFirstBordInformation = new List<int>()
                {
                    getdaytoday,
                    getmonthtoday,
                    getweektoday
                };

                dbClient.SetQuery("SELECT * FROM wired_scorebord WHERE roomid = @id ORDER BY `punten` DESC ");
                dbClient.AddParameter("id", Id);
                foreach (DataRow row in dbClient.GetTable().Rows)
                {
                    int userid = Convert.ToInt32(row["userid"]);
                    string username = Convert.ToString(row["username"]);
                    int Punten = Convert.ToInt32(row["punten"]);
                    string soort = Convert.ToString(row["soort"]);
                    int timestamp = Convert.ToInt32(row["timestamp"]);
                    if ((!(soort == "day") || WiredScoreBordDay.ContainsKey(userid) ? false : !SuperCheck[0]))
                    {
                        if (timestamp != getdaytoday)
                        {
                            SuperCheck[0] = false;
                        }
                        if (!SuperCheck[0])
                        {
                            WiredScoreBordDay.Add(userid, new KeyValuePair<int, string>(Punten, username));
                        }
                    }
                    if ((!(soort == "month") || WiredScoreBordMonth.ContainsKey(userid) ? false : !SuperCheck[1]))
                    {
                        if (timestamp != getmonthtoday)
                        {
                            SuperCheck[1] = false;
                        }
                        WiredScoreBordMonth.Add(userid, new KeyValuePair<int, string>(Punten, username));
                    }
                    if ((!(soort == "week") || WiredScoreBordWeek.ContainsKey(userid) ? false : !SuperCheck[2]))
                    {
                        if (timestamp != getweektoday)
                        {
                            SuperCheck[2] = false;
                        }
                        WiredScoreBordWeek.Add(userid, new KeyValuePair<int, string>(Punten, username));
                    }
                }
                if (SuperCheck[0])
                {
                    dbClient.RunFastQuery(string.Concat("DELETE FROM `wired_scorebord` WHERE `roomid`='", Id, "' AND `soort`='day'"));
                    WiredScoreBordDay.Clear();
                }
                if (SuperCheck[1])
                {
                    dbClient.RunFastQuery(string.Concat("DELETE FROM `wired_scorebord` WHERE `roomid`='", Id, "' AND `soort`='month'"));
                    WiredScoreBordMonth.Clear();
                }
                if (SuperCheck[2])
                {
                    dbClient.RunFastQuery(string.Concat("DELETE FROM `wired_scorebord` WHERE `roomid`='", Id, "' AND `soort`='week'"));
                    WiredScoreBordDay.Clear();
                }
            }
        }


        public RoomPromotion Promotion
        {
            get => _promotion;
            set => _promotion = value;
        }

        public bool HasActivePromotion => Promotion != null;

        public void EndPromotion()
        {
            if (!HasActivePromotion)
            {
                return;
            }

            Promotion = null;
        }

        public RoomModel Model
        {
            get
            {
                if (mModel == null)
                {
                    mModel = StarBlueServer.GetGame().GetRoomManager().GetModel(ModelName);
                }

                return mModel;
            }
        }
    }
}
