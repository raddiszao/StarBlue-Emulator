using log4net;
using StarBlue.Core;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StarBlue.HabboHotel.Rooms
{
    public class RoomManager
    {
        private static readonly ILog log = LogManager.GetLogger("StarBlue.HabboHotel.Rooms.RoomManager");

        private Dictionary<string, RoomModel> _roomModels;

        private ConcurrentDictionary<int, Room> _rooms;
        private ConcurrentDictionary<int, RoomData> _loadedRoomData;

        private readonly object _roomLoadingSync;

        private DateTime _cycleLastExecution;

        public RoomManager()
        {
            _roomModels = new Dictionary<string, RoomModel>();

            _rooms = new ConcurrentDictionary<int, Room>();
            _loadedRoomData = new ConcurrentDictionary<int, RoomData>();
            _roomLoadingSync = new object();

            LoadModels();
            log.Info(">> Rooms Manager -> READY!");
        }

        public void OnCycle()
        {
            TimeSpan sinceLastTime = DateTime.Now - _cycleLastExecution;
            if (sinceLastTime.TotalMilliseconds >= 500)
            {
                _cycleLastExecution = DateTime.Now;
                foreach (Room Room in _rooms.Values.ToList())
                {
                    if (Room.isCrashed)
                    {
                        continue;
                    }

                    if (Room.ProcessTask == null || Room.ProcessTask.IsCompleted)
                    {
                        Room.ProcessTask = new Task(Room.ProcessRoom);
                        Room.ProcessTask.Start();
                        Room.IsLagging = 0;
                    }
                    else
                    {
                        Room.IsLagging++;
                        if (Room.IsLagging >= 40)
                        {
                            Room.isCrashed = true;
                            UnloadRoom(Room.Id);
                            Logging.WriteLine("[RoomMgr] Room crashed (task didn't complete within 40 seconds): " + Room.Id);
                        }
                    }
                }
            }
        }

        public int LoadedRoomDataCount => _loadedRoomData.Count;

        public int Count => _rooms.Count;

        public void LoadModels()
        {
            if (_roomModels.Count > 0)
            {
                _roomModels.Clear();
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id,door_x,door_y,door_z,door_dir,heightmap,public_items,club_only,wall_height FROM `room_models` WHERE `custom` = '0'");
                DataTable Data = dbClient.GetTable();

                if (Data == null)
                {
                    return;
                }

                foreach (DataRow Row in Data.Rows)
                {
                    string Modelname = Convert.ToString(Row["id"]);

                    _roomModels.Add(Modelname, new RoomModel(Convert.ToInt32(Row["door_x"]), Convert.ToInt32(Row["door_y"]), (Double)Row["door_z"], Convert.ToInt32(Row["door_dir"]),
                        Convert.ToString(Row["heightmap"]), StarBlueServer.EnumToBool(Row["club_only"].ToString()), Convert.ToInt32(Row["wall_height"])));
                }
            }
        }

        public void LoadModel(string Id)
        {
            DataRow Row = null;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id,door_x,door_y,door_z,door_dir,heightmap,public_items,club_only,wall_height FROM `room_models` WHERE `custom` = '1' AND `id` = '" + Id + "' LIMIT 1");
                Row = dbClient.GetRow();

                if (Row == null)
                {
                    return;
                }

                string Modelname = Convert.ToString(Row["id"]);
                if (!this._roomModels.ContainsKey(Id))
                {
                    this._roomModels.Add(Modelname, new RoomModel(Convert.ToInt32(Row["door_x"]), Convert.ToInt32(Row["door_y"]), Convert.ToDouble(Row["door_z"]), Convert.ToInt32(Row["door_dir"]),
                      Convert.ToString(Row["heightmap"]), StarBlueServer.EnumToBool(Row["club_only"].ToString()), Convert.ToInt32(Row["wall_height"])));
                }
            }
        }

        public void ReloadModel(string Id)
        {
            if (!_roomModels.ContainsKey(Id))
            {
                LoadModel(Id);
                return;
            }

            _roomModels.Remove(Id);
            LoadModel(Id);
        }

        public bool TryGetModel(string Id, out RoomModel Model)
        {
            return _roomModels.TryGetValue(Id, out Model);
        }

        public void UnloadRoom(int RoomId, bool RemoveData = false)
        {
            if (_rooms.TryRemove(RoomId, out Room room))
            {
                room.Dispose();

                if (RemoveData)
                {
                    _loadedRoomData.TryRemove(RoomId, out RoomData Data);
                }
            }
            //Logging.WriteLine("[RoomMgr] Unloaded room: \"" + Room.Name + "\" (ID: " + Room.RoomId + ")");
        }

        public List<Room> SearchGroupRooms(GameClient Session, string query)
        {
            return _rooms.Values.Where(x => x.RoomData.Group != null && x.RoomData.Group.Name.ToLower().Contains(query.ToLower()) && x.RoomData.Access != RoomAccess.INVISIBLE || (x.RoomData.Group != null && x.RoomData.Group.Name.ToLower().Contains(query.ToLower()) && x.RoomData.Access == RoomAccess.INVISIBLE && x.CheckRights(Session))).OrderByDescending(x => x.RoomData.UsersNow).Take(50).ToList();
        }

        public List<Room> SearchTaggedRooms(GameClient Session, string query)
        {
            return _rooms.Values.Where(x => x.RoomData.Tags.Contains(query) && x.RoomData.Access != RoomAccess.INVISIBLE || (x.RoomData.Tags.Contains(query) && x.RoomData.Access == RoomAccess.INVISIBLE && x.CheckRights(Session))).OrderByDescending(x => x.RoomData.UsersNow).Take(50).ToList();
        }

        public List<Room> GetPopularRooms(GameClient Session, int category, int Amount = 50)
        {
            return _rooms.Values.Where(x => x.RoomData.UsersNow > 0 && x.RoomData.Access != RoomAccess.INVISIBLE || (x.RoomData.UsersNow > 0 && x.RoomData.Access == RoomAccess.INVISIBLE && x.CheckRights(Session))).OrderByDescending(x => x.RoomData.UsersNow).Take(Amount).ToList();
        }

        public List<Room> GetRecommendedRooms(GameClient Session, int amount = 50, int CurrentRoomId = 0)
        {
            return _rooms.Values.Where(x => x.RoomData.Id != CurrentRoomId && x.RoomData.Access != RoomAccess.INVISIBLE || (x.RoomData.Id != CurrentRoomId && x.RoomData.Access == RoomAccess.INVISIBLE && x.CheckRights(Session))).OrderByDescending(x => x.RoomData.UsersNow).OrderByDescending(x => x.RoomData.Score).Take(amount).ToList();
        }

        public List<Room> GetPopularRatedRooms(GameClient Session, int amount = 50)
        {
            return _rooms.Values.Where(x => x.RoomData.Access != RoomAccess.INVISIBLE || (x.RoomData.Access == RoomAccess.INVISIBLE && x.CheckRights(Session))).OrderByDescending(x => x.RoomData.Score).OrderByDescending(x => x.RoomData.UsersNow).Take(amount).ToList();
        }

        public List<Room> GetRoomsByCategory(GameClient Session, int category, int amount = 50)
        {
            return _rooms.Values.Where(x => x.RoomData.Category == category && x.RoomData.Access != RoomAccess.INVISIBLE && x.RoomData.UsersNow > 0 || (x.RoomData.Category == category && x.RoomData.UsersNow > 0 && x.RoomData.Access == RoomAccess.INVISIBLE && x.CheckRights(Session))).OrderByDescending(x => x.RoomData.UsersNow).Take(amount).ToList();
        }

        public List<Room> GetOnGoingRoomPromotions(GameClient Session, int Mode, int Amount = 50)
        {
            if (Mode == 17)
            {
                return _rooms.Values.Where(x => x.RoomData.HasActivePromotion && x.RoomData.Access != RoomAccess.INVISIBLE || (x.RoomData.HasActivePromotion && x.RoomData.Access == RoomAccess.INVISIBLE && x.CheckRights(Session))).OrderByDescending(x => x.RoomData.Promotion.TimestampStarted).Take(Amount).ToList();
            }

            return _rooms.Values.Where(x => x.RoomData.HasActivePromotion && x.RoomData.Access != RoomAccess.INVISIBLE || (x.RoomData.HasActivePromotion && x.RoomData.Access == RoomAccess.INVISIBLE && x.CheckRights(Session))).OrderByDescending(x => x.RoomData.UsersNow).Take(Amount).ToList();
        }

        public List<Room> GetPromotedRooms(GameClient Session, int categoryId, int amount = 50)
        {
            return _rooms.Values.Where(x => x.RoomData.HasActivePromotion && x.RoomData.Promotion.CategoryId == categoryId && x.RoomData.Access != RoomAccess.INVISIBLE || (x.RoomData.HasActivePromotion && x.RoomData.Promotion.CategoryId == categoryId && x.RoomData.Access == RoomAccess.INVISIBLE && x.CheckRights(Session))).OrderByDescending(x => x.RoomData.Promotion.TimestampStarted).Take(amount).ToList();
        }

        public List<Room> GetGroupRooms(GameClient Session, int amount = 50)
        {
            return _rooms.Values.Where(x => x.RoomData.Group != null && x.RoomData.Access != RoomAccess.INVISIBLE || (x.RoomData.Group != null && x.RoomData.Access == RoomAccess.INVISIBLE && x.CheckRights(Session))).OrderByDescending(x => x.RoomData.Score).Take(amount).ToList();
        }

        public List<Room> GetRoomsByIds(GameClient Session, List<int> ids, int amount = 50)
        {
            return _rooms.Values.Where(x => ids.Contains(x.RoomData.Id) && x.RoomData.Access != RoomAccess.INVISIBLE || (ids.Contains(x.RoomData.Id) && x.RoomData.Access == RoomAccess.INVISIBLE && x.CheckRights(Session))).OrderByDescending(x => x.RoomData.UsersNow).Take(amount).ToList();
        }

        public Room TryGetRandomLoadedRoom(GameClient Session)
        {
            return _rooms.Values.Where(x => x.RoomData.UsersNow > 0 && x.RoomData.Access != RoomAccess.INVISIBLE && x.RoomData.UsersNow < x.RoomData.UsersMax || (x.RoomData.UsersNow < x.RoomData.UsersMax && x.RoomData.UsersNow > 0 && x.RoomData.Access == RoomAccess.INVISIBLE && x.CheckRights(Session))).OrderByDescending(x => x.RoomData.UsersNow).FirstOrDefault();
        }

        public RoomModel GetModel(string Model)
        {
            if (_roomModels.ContainsKey(Model))
            {
                return _roomModels[Model];
            }

            return null;
        }

        public bool TryLoadRoom(int roomId, out Room room)
        {
            Room inst = null;
            if (_rooms.TryGetValue(roomId, out inst))
            {
                if (!inst.mDisposed)
                {
                    room = inst;
                    return true;
                }

                room = null;
                return false;
            }

            lock (_roomLoadingSync)
            {
                if (_rooms.TryGetValue(roomId, out inst))
                {
                    if (!inst.mDisposed)
                    {
                        room = inst;
                        return true;
                    }

                    room = null;
                    return false;
                }

                RoomData data = GenerateRoomData(roomId);
                if (data == null)
                {
                    room = null;
                    return false;
                }

                Room myInstance = new Room(data);
                if (_rooms.TryAdd(roomId, myInstance))
                {
                    room = myInstance;
                    return true;
                }

                room = null;
                return false;
            }
        }

        public RoomData GenerateRoomData(int RoomId)
        {
            if (_loadedRoomData.ContainsKey(RoomId))
            {
                return _loadedRoomData[RoomId];
            }

            RoomData Data = new RoomData();

            if (TryGetRoom(RoomId, out Room Room))
            {
                return Room.RoomData;
            }

            DataRow Row = null;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM rooms WHERE id = " + RoomId + " LIMIT 1");
                Row = dbClient.GetRow();
            }

            if (Row == null)
            {
                return null;
            }

            Data.Fill(Row);

            if (!_loadedRoomData.ContainsKey(RoomId))
            {
                _loadedRoomData.TryAdd(RoomId, Data);
            }

            return Data;
        }

        public RoomData FetchRoomData(int RoomId, DataRow dRow)
        {
            if (_loadedRoomData.ContainsKey(RoomId))
            {
                return _loadedRoomData[RoomId];
            }
            else
            {
                RoomData data = new RoomData();

                data.Fill(dRow);

                if (!_loadedRoomData.ContainsKey(RoomId))
                {
                    _loadedRoomData.TryAdd(RoomId, data);
                }

                return data;
            }
        }

        public Room LoadRoom(int Id)
        {
            if (TryGetRoom(Id, out Room Room))
            {
                return Room;
            }

            RoomData Data = GenerateRoomData(Id);
            if (Data == null)
            {
                return null;
            }

            Room = new Room(Data);

            if (!_rooms.ContainsKey(Room.Id))
            {
                _rooms.TryAdd(Room.Id, Room);
            }

            return Room;
        }

        public bool TryGetRoom(int RoomId, out Room Room)
        {
            return _rooms.TryGetValue(RoomId, out Room);
        }

        public RoomData CreateRoom(GameClient Session, string Name, string Description, string Model, int Category, int MaxVisitors, int TradeSettings)
        {
            if (!_roomModels.ContainsKey(Model))
            {
                Session.SendNotification(StarBlueServer.GetLanguageManager().TryGetValue("room_model_missing"));
                return null;
            }

            if (Name.Length < 3)
            {
                Session.SendNotification(StarBlueServer.GetLanguageManager().TryGetValue("room_name_length_short"));
                return null;
            }

            int RoomId = 0;

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `rooms` (`roomtype`,`caption`,`description`,`owner`,`model_name`,`category`,`users_max`,`trade_settings`) VALUES ('private',@caption,@description,@UserId,@model,@category,@usersmax,@tradesettings)");
                dbClient.AddParameter("caption", Name);
                dbClient.AddParameter("description", Description);
                dbClient.AddParameter("UserId", Session.GetHabbo().Id);
                dbClient.AddParameter("model", Model);
                dbClient.AddParameter("category", Category);
                dbClient.AddParameter("usersmax", MaxVisitors);
                dbClient.AddParameter("tradesettings", TradeSettings);

                RoomId = Convert.ToInt32(dbClient.InsertQuery());
            }

            RoomData newRoomData = GenerateRoomData(RoomId);
            Session.GetHabbo().UsersRooms.Add(newRoomData);
            return newRoomData;
        }

        public ICollection<Room> GetRooms()
        {
            return _rooms.Values;
        }

        public void Dispose()
        {
            int length = _rooms.Count;
            int i = 0;
            foreach (Room room in _rooms.Values.ToList())
            {
                if (room == null)
                {
                    continue;
                }

                StarBlueServer.GetGame().GetRoomManager().UnloadRoom(room.Id);
                Console.Clear();
                log.Info("<<- SERVER SHUTDOWN ->> ROOM ITEM SAVE: " + string.Format("{0:0.##}", ((double)i / length) * 100) + "%");
                i++;
            }

            log.Info("Done disposing rooms!");
        }
    }
}