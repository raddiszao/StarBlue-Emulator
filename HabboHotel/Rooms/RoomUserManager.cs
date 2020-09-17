using StarBlue.Communication.Packets.Outgoing.Handshake;
using StarBlue.Communication.Packets.Outgoing.Rooms.Avatar;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Communication.Packets.Outgoing.Rooms.Permissions;
using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using StarBlue.Core;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Items.Wired;
using StarBlue.HabboHotel.Rooms.AI;
using StarBlue.HabboHotel.Rooms.Games.Teams;
using StarBlue.HabboHotel.Rooms.PathFinding;
using StarBlue.HabboHotel.Users;
using StarBlue.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms
{
    public class RoomUserManager
    {
        private Room _room;
        private ConcurrentDictionary<int, RoomUser> _users;
        private ConcurrentDictionary<int, RoomUser> _bots;
        private ConcurrentDictionary<int, RoomUser> _pets;

        private int primaryPrivateUserID;
        private int secondaryPrivateUserID;

        public int userCount;

        public RoomUserManager(Room room)
        {
            _room = room;
            _users = new ConcurrentDictionary<int, RoomUser>();
            _pets = new ConcurrentDictionary<int, RoomUser>();
            _bots = new ConcurrentDictionary<int, RoomUser>();

            primaryPrivateUserID = 0;
            secondaryPrivateUserID = 0;

            userCount = 0;
        }

        public void Dispose()
        {
            _users.Clear();
            _pets.Clear();
            _bots.Clear();

            _users = null;
            _pets = null;
            _bots = null;
            _room = null;
        }

        public RoomUser DeployBot(RoomBot Bot, Pet PetData)
        {
            RoomUser BotUser = new RoomUser(0, _room.Id, primaryPrivateUserID++, _room);
            Bot.VirtualId = primaryPrivateUserID;

            int PersonalID = secondaryPrivateUserID++;
            BotUser.InternalRoomID = PersonalID;
            _users.TryAdd(PersonalID, BotUser);

            DynamicRoomModel Model = _room.GetGameMap().Model;

            if ((Bot.X > 0 && Bot.Y > 0) && Bot.X < Model.MapSizeX && Bot.Y < Model.MapSizeY)
            {
                BotUser.SetPos(Bot.X, Bot.Y, Bot.Z);
                BotUser.SetRot(Bot.Rot, false);
            }
            else
            {
                Bot.X = Model.DoorX;
                Bot.Y = Model.DoorY;

                BotUser.SetPos(Model.DoorX, Model.DoorY, Model.DoorZ);
                BotUser.SetRot(Model.DoorOrientation, false);
            }

            BotUser.BotData = Bot;
            BotUser.BotAI = Bot.GenerateBotAI(BotUser.VirtualId);

            if (BotUser.IsPet)
            {
                BotUser.BotAI.Init(Bot.BotId, BotUser.VirtualId, _room.Id, BotUser, _room);
                BotUser.PetData = PetData;
                BotUser.PetData.VirtualId = BotUser.VirtualId;
            }
            else
            {
                BotUser.BotAI.Init(Bot.BotId, BotUser.VirtualId, _room.Id, BotUser, _room);
            }

            UpdateUserStatus(BotUser, false);
            BotUser.UpdateNeeded = true;

            _room.SendMessage(new UsersComposer(BotUser));

            if (BotUser.IsPet)
            {
                if (_pets.ContainsKey(BotUser.PetData.PetId)) //Pet allready placed
                {
                    _pets[BotUser.PetData.PetId] = BotUser;
                }
                else
                {
                    _pets.TryAdd(BotUser.PetData.PetId, BotUser);
                }
            }
            else if (BotUser.IsBot)
            {
                if (_bots.ContainsKey(BotUser.BotData.BotId))
                {
                    _bots[BotUser.BotData.BotId] = BotUser;
                }
                else
                {
                    _bots.TryAdd(BotUser.BotData.Id, BotUser);
                }

                _room.SendMessage(new DanceComposer(BotUser, BotUser.BotData.DanceId));
            }
            return BotUser;
        }

        public void RemoveBot(int VirtualId, bool Kicked)
        {
            RoomUser User = GetRoomUserByVirtualId(VirtualId);
            if (User == null || !User.IsBot)
            {
                return;
            }

            if (User.IsPet)
            {

                _pets.TryRemove(User.PetData.PetId, out RoomUser PetRemoval);
            }
            else
            {
                _bots.TryRemove(User.BotData.Id, out RoomUser BotRemoval);
            }

            User.BotAI.OnSelfLeaveRoom(Kicked);

            _room.SendMessage(new UserRemoveComposer(User.VirtualId));


            if (_users != null)
            {
                _users.TryRemove(User.InternalRoomID, out RoomUser toRemove);
            }

            onRemove(User);
        }

        public RoomUser GetUserForSquare(int x, int y)
        {
            return _room.GetGameMap().GetRoomUsers(new Point(x, y)).FirstOrDefault();
        }

        public bool AddAvatarToRoom(GameClient Session)
        {
            if (_room == null)
            {
                return false;
            }

            if (Session == null || Session.GetHabbo() == null)
            {
                return false;
            }

            if (Session.GetHabbo().CurrentRoom == null)
            {
                return false;
            }

            RoomUser User = new RoomUser(Session.GetHabbo().Id, _room.Id, primaryPrivateUserID++, _room);

            if (User == null || User.GetClient() == null)
            {
                return false;
            }

            User.UserId = Session.GetHabbo().Id;
            Session.GetHabbo().TentId = 0;

            int PersonalID = secondaryPrivateUserID++;
            User.InternalRoomID = PersonalID;

            Session.GetHabbo().CurrentRoomId = _room.Id;

            if (_room.MultiWhispers.Contains(User))
            {
                _room.MultiWhispers.Remove(User);
            }

            if (!_users.TryAdd(PersonalID, User))
            {
                return false;
            }

            DynamicRoomModel Model = _room.GetGameMap().Model;
            if (Model == null)
            {
                return false;
            }

            if (!_room.RoomData.PetMorphsAllowed && Session.GetHabbo().PetId != 0)
            {
                Session.GetHabbo().PetId = 0;
            }

            if (!Session.GetHabbo().IsTeleporting && !Session.GetHabbo().IsHopping)
            {
                if (!Model.DoorIsValid())
                {
                    Point Square = _room.GetGameMap().getRandomWalkableSquare();
                    Model.DoorX = Square.X;
                    Model.DoorY = Square.Y;
                    Model.DoorZ = _room.GetGameMap().GetHeightForSquareFromData(Square);
                }

                User.SetPos(Model.DoorX, Model.DoorY, Model.DoorZ);
                User.SetRot(Model.DoorOrientation, false);
            }
            else if (!User.IsBot && (User.GetClient().GetHabbo().IsTeleporting || User.GetClient().GetHabbo().IsHopping))
            {
                Item Item = null;
                if (Session.GetHabbo().IsTeleporting)
                {
                    Item = _room.GetRoomItemHandler().GetItem(Session.GetHabbo().TeleporterId);
                }
                else if (Session.GetHabbo().IsHopping)
                {
                    Item = _room.GetRoomItemHandler().GetItem(Session.GetHabbo().HopperId);
                }

                if (Item != null)
                {
                    if (Session.GetHabbo().IsTeleporting)
                    {
                        Item.ExtraData = "2";
                        Item.UpdateState(false, true);
                        User.SetPos(Item.GetX, Item.GetY, Item.GetZ);
                        User.SetRot(Item.Rotation, false);
                        Item.InteractingUser2 = Session.GetHabbo().Id;
                        Item.ExtraData = "0";
                        Item.UpdateState(false, true);
                    }
                    else if (Session.GetHabbo().IsHopping)
                    {
                        Item.ExtraData = "1";
                        Item.UpdateState(false, true);
                        User.SetPos(Item.GetX, Item.GetY, Item.GetZ);
                        User.SetRot(Item.Rotation, false);
                        User.AllowOverride = false;
                        Item.InteractingUser2 = Session.GetHabbo().Id;
                        Item.ExtraData = "2";
                        Item.UpdateState(false, true);
                    }
                }
                else
                {
                    User.SetPos(Model.DoorX, Model.DoorY, Model.DoorZ - 1);
                    User.SetRot(Model.DoorOrientation, false);
                }
            }

            if (!Session.GetHabbo().Spectating)
            {
                _room.SendMessage(new UsersComposer(User));

                if (_room.CheckRights(Session, true))
                {
                    User.SetStatus("flatctrl", "useradmin");
                    Session.SendMessage(new YouAreOwnerComposer());
                    Session.SendMessage(new YouAreControllerComposer(4));
                }
                else if (_room.CheckRights(Session, false) && _room.RoomData.Group == null)
                {
                    User.SetStatus("flatctrl", "1");
                    Session.SendMessage(new YouAreControllerComposer(1));
                }
                else if (_room.RoomData.Group != null && _room.CheckRights(Session, false, true))
                {
                    User.SetStatus("flatctrl", "3");
                    Session.SendMessage(new YouAreControllerComposer(3));
                }
                else
                {
                    Session.SendMessage(new YouAreNotControllerComposer());
                }

                User.UpdateNeeded = true;
            }

            if (_room.ForSale && _room.SalePrice > 0 && (_room.GetRoomUserManager().GetRoomUserByHabbo(_room.RoomData.OwnerName) != null))
            {
                Session.SendWhisper("Este quarto está a venda, em  " + _room.SalePrice + " duckets. Escreva :buyroom se quer comprá-lo!");
            }
            else if (_room.ForSale && _room.GetRoomUserManager().GetRoomUserByHabbo(_room.RoomData.OwnerName) == null)
            {
                foreach (RoomUser _User in _room.GetRoomUserManager().GetRoomUsers())
                {
                    if (_User.GetClient() != null && _User.GetClient().GetHabbo() != null && _User.GetClient().GetHabbo().Id != Session.GetHabbo().Id)
                    {
                        _User.GetClient().SendWhisper("Esta sala não está a venda.");
                    }
                }
                _room.ForSale = false;
                _room.SalePrice = 0;
            }

            return true;
        }

        public void RemoveUserFromRoom(GameClient Session, bool NotifyClient, bool NotifyKick = false)
        {
            try
            {
                if (_room == null)
                {
                    return;
                }

                Habbo Habbo = Session.GetHabbo();
                if (Session == null || Habbo == null)
                {
                    return;
                }

                if (NotifyKick)
                {
                    Session.SendMessage(new GenericErrorComposer(4008));
                }

                if (NotifyClient)
                {
                    Session.SendMessage(new CloseConnectionComposer());
                }

                if (Habbo.CurrentRoom != null && Habbo.CurrentRoom.GetWired() != null)
                {
                    Habbo.CurrentRoom.GetWired().TriggerEvent(WiredBoxType.TriggerLeaveRoom, Session.GetHabbo());
                }

                Habbo.BuilderTool = false;
                RoomUser User = GetRoomUserByHabbo(Habbo.Id);

                if (_room.Tents.ContainsKey(Habbo.TentId))
                {
                    foreach (KeyValuePair<int, List<RoomUser>> Tent in _room.Tents)
                    {
                        RoomUser TentUser = Tent.Value.Where(x => x != null && x.HabboId == User.HabboId).FirstOrDefault();
                        if (TentUser != null)
                            _room.RemoveUserFromTent(Tent.Key, TentUser);
                    }
                }

                if (Habbo.TentId > 0)
                {
                    Habbo.TentId = 0;
                }

                if (User != null)
                {
                    if (User.RidingHorse)
                    {
                        User.RidingHorse = false;
                        RoomUser UserRiding = GetRoomUserByVirtualId(User.HorseID);
                        if (UserRiding != null)
                        {
                            UserRiding.RidingHorse = false;
                            UserRiding.HorseID = 0;
                        }
                    }

                    if (User.Team != TEAM.NONE)
                    {
                        TeamManager Team = _room.GetTeamManagerForFreeze();
                        if (Team != null)
                        {
                            Team.OnUserLeave(User);

                            User.Team = TEAM.NONE;

                            if (User.GetClient().GetHabbo().Effects().CurrentEffect != 0)
                            {
                                User.GetClient().GetHabbo().Effects().ApplyEffect(0);
                            }
                        }
                    }

                    RemoveRoomUser(User);

                    if (User.CurrentItemEffect != ItemEffectType.NONE)
                    {
                        if (Habbo.Effects() != null)
                        {
                            Habbo.Effects().CurrentEffect = -1;
                        }
                    }

                    if (_room != null)
                    {
                        if (_room.HasActiveTrade(Habbo.Id))
                        {
                            _room.TryStopTrade(Habbo.Id);
                        }
                    }

                    //Session.GetHabbo().CurrentRoomId = 0;

                    //if (Habbo.GetMessenger() != null)
                    //  {
                    //      Habbo.GetMessenger().OnStatusChanged(true);
                    //   }

                    if (Habbo != null && Habbo.Id == _room.RoomData.OwnerId && _room.ForSale)
                    {
                        _room.ForSale = false;
                        _room.SalePrice = 0;
                        foreach (RoomUser m in GetRoomUsers())
                        {
                            if (m != null && m.GetClient() != null && m.GetClient().GetHabbo() != null && m.GetClient().GetHabbo().Id != _room.RoomData.OwnerId)
                            {
                                m.GetClient().SendWhisper("Esta sala não está a venda.");
                            }
                        }
                    }

                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunFastQuery("UPDATE user_roomvisits SET exit_timestamp = '" + StarBlueServer.GetUnixTimestamp() + "' WHERE room_id = '" + _room.Id + "' AND user_id = '" + Habbo.Id + "' ORDER BY exit_timestamp DESC LIMIT 1");
                        if (_room.RoomData != null)
                            dbClient.RunFastQuery("UPDATE `rooms` SET `users_now` = '" + _room.RoomData.UsersNow + "' WHERE `id` = '" + _room.Id + "' LIMIT 1");
                    }

                    if (User != null)
                    {
                        User.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e.ToString());
            }
        }

        private void onRemove(RoomUser user)
        {
            try
            {

                GameClient session = user.GetClient();
                if (session == null)
                {
                    return;
                }

                List<RoomUser> Bots = new List<RoomUser>();

                try
                {
                    foreach (RoomUser roomUser in GetUserList().ToList())
                    {
                        if (roomUser == null)
                        {
                            continue;
                        }

                        if (roomUser.IsBot && !roomUser.IsPet)
                        {
                            if (!Bots.Contains(roomUser))
                            {
                                Bots.Add(roomUser);
                            }
                        }
                    }
                }
                catch { }

                List<RoomUser> PetsToRemove = new List<RoomUser>();
                foreach (RoomUser Bot in Bots.ToList())
                {
                    if (Bot == null || Bot.BotAI == null)
                    {
                        continue;
                    }

                    Bot.BotAI.OnUserLeaveRoom(session);

                    if (Bot.IsPet && Bot.PetData.OwnerId == user.UserId && !_room.CheckRights(session, true))
                    {
                        if (!PetsToRemove.Contains(Bot))
                        {
                            PetsToRemove.Add(Bot);
                        }
                    }
                }

                foreach (RoomUser toRemove in PetsToRemove.ToList())
                {
                    if (toRemove == null)
                    {
                        continue;
                    }

                    if (user.GetClient() == null || user.GetClient().GetHabbo() == null || user.GetClient().GetHabbo().GetInventoryComponent() == null)
                    {
                        continue;
                    }

                    user.GetClient().GetHabbo().GetInventoryComponent().TryAddPet(toRemove.PetData);
                    RemoveBot(toRemove.VirtualId, false);
                }

                _room.GetGameMap().RemoveUserFromMap(user, new Point(user.X, user.Y));
            }
            catch (Exception e)
            {
                Logging.LogCriticalException(e.ToString());
            }
        }

        private void RemoveRoomUser(RoomUser user)
        {
            int X = user.GetX();
            int Y = user.GetY();
            if (_room.GetGameMap().ValidTile(X, Y))
                _room.GetGameMap().GameMap[X, Y] = user.SqState;
            _room.GetGameMap().RemoveUserFromMap(user, new Point(user.X, user.Y));
            _room.SendMessage(new UserRemoveComposer(user.VirtualId));

            if (_users.TryRemove(user.InternalRoomID, out RoomUser toRemove))
            {
                //uhmm, could put the below stuff in but idk.
            }

            user.InternalRoomID = -1;
            onRemove(user);
        }

        public bool TryGetPet(int PetId, out RoomUser Pet)
        {
            return _pets.TryGetValue(PetId, out Pet);
        }

        public bool TryGetBot(int BotId, out RoomUser Bot)
        {
            return _bots.TryGetValue(BotId, out Bot);
        }

        public RoomUser GetBotByName(string Name)
        {
            bool FoundBot = _bots.Where(x => x.Value.BotData != null && x.Value.BotData.Name.ToLower() == Name.ToLower()).ToList().Count() > 0;
            if (FoundBot)
            {
                int Id = _bots.FirstOrDefault(x => x.Value.BotData != null && x.Value.BotData.Name.ToLower() == Name.ToLower()).Value.BotData.Id;

                return _bots[Id];
            }

            return null;
        }

        public void UpdateUserCount(int count)
        {
            userCount = count;
            _room.RoomData.UsersNow = count;

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE `rooms` SET `users_now` = '" + count + "' WHERE `id` = '" + _room.Id + "' LIMIT 1");
            }
        }

        public RoomUser GetRoomUserByVirtualId(int VirtualId)
        {
            if (!_users.TryGetValue(VirtualId, out RoomUser User))
            {
                return null;
            }

            return User;
        }

        public RoomUser GetRoomUserByHabbo(int Id)
        {
            if (this == null)
            {
                return null;
            }

            RoomUser User = GetUserList().Where(x => x != null && x.GetClient() != null && x.GetClient().GetHabbo() != null && x.GetClient().GetHabbo().Id == Id).FirstOrDefault();

            if (User != null)
            {
                return User;
            }

            return null;
        }

        public RoomUser GetRoomUserByUsername(string Username)
        {
            if (this == null)
            {
                return null;
            }

            RoomUser User = GetUserList().Where(x => x != null && x.GetClient() != null && x.GetClient().GetHabbo() != null && x.GetClient().GetHabbo().Username == Username).FirstOrDefault();

            if (User != null)
            {
                return User;
            }

            return null;
        }

        public List<RoomUser> GetRoomUsers()
        {
            List<RoomUser> List = new List<RoomUser>();

            List = GetUserList().Where(x => (!x.IsBot)).ToList();

            return List;
        }

        public List<RoomUser> GetRoomusersByChat(bool chat)
        {
            List<RoomUser> returnList = new List<RoomUser>();
            foreach (RoomUser user in GetUserList().ToList())
            {
                if (user == null)
                {
                    continue;
                }

                if (!user.IsBot && user.GetClient() != null && user.GetClient().GetHabbo() != null && user.GetClient().GetHabbo().ChatPreference == false)
                {
                    returnList.Add(user);
                }
            }

            return returnList;
        }

        public List<RoomUser> GetRoomUserByRank(int minRank)
        {
            List<RoomUser> returnList = new List<RoomUser>();
            foreach (RoomUser user in GetUserList().ToList())
            {
                if (user == null)
                {
                    continue;
                }

                if (!user.IsBot && user.GetClient() != null && user.GetClient().GetHabbo() != null && user.GetClient().GetHabbo().Rank >= minRank)
                {
                    returnList.Add(user);
                }
            }

            return returnList;
        }

        public RoomUser GetRoomUserByHabbo(string pName)
        {
            RoomUser User = GetUserList().Where(x => x != null && x.GetClient() != null && x.GetClient().GetHabbo() != null && x.GetClient().GetHabbo().Username.Equals(pName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (User != null)
            {
                return User;
            }

            return null;
        }

        public void UpdatePets()
        {
            foreach (Pet Pet in GetPets().ToList())
            {
                if (Pet == null)
                {
                    continue;
                }

                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    if (Pet.DBState == DatabaseUpdateState.NeedsInsert)
                    {
                        dbClient.SetQuery("INSERT INTO `bots` (`id`,`user_id`,`room_id`,`name`,`x`,`y`,`z`) VALUES ('" + Pet.PetId + "','" + Pet.OwnerId + "','" + Pet.RoomId + "',@name,'0','0','0')");
                        dbClient.AddParameter("name", Pet.Name);
                        dbClient.RunQuery();

                        dbClient.SetQuery("INSERT INTO `bots_petdata` (`type`,`race`,`color`,`experience`,`energy`,`createstamp`,`nutrition`,`respect`) VALUES ('" + Pet.Type + "',@race,@color,'0','100','" + Pet.CreationStamp + "','0','0')");
                        dbClient.AddParameter(Pet.PetId + "race", Pet.Race);
                        dbClient.AddParameter(Pet.PetId + "color", Pet.Color);
                        dbClient.RunQuery();
                    }
                    else if (Pet.DBState == DatabaseUpdateState.NeedsUpdate)
                    {
                        //Surely this can be *99 better?
                        RoomUser User = GetRoomUserByVirtualId(Pet.VirtualId);

                        dbClient.RunFastQuery("UPDATE `bots` SET room_id = " + Pet.RoomId + ", x = " + (User != null ? User.X : 0) + ", Y = " + (User != null ? User.Y : 0) + ", Z = " + (User != null ? User.Z : 0) + " WHERE `id` = '" + Pet.PetId + "' LIMIT 1");
                        dbClient.RunFastQuery("UPDATE `bots_petdata` SET `experience` = '" + Pet.experience + "', `energy` = '" + Pet.Energy + "', `nutrition` = '" + Pet.Nutrition + "', `respect` = '" + Pet.Respect + "' WHERE `id` = '" + Pet.PetId + "' LIMIT 1");
                    }

                    Pet.DBState = DatabaseUpdateState.Updated;
                }
            }
        }

        public List<Pet> GetPets()
        {
            List<Pet> Pets = new List<Pet>();
            foreach (RoomUser User in _pets.Values.ToList())
            {
                if (User == null || !User.IsPet)
                {
                    continue;
                }

                Pets.Add(User.PetData);
            }

            return Pets;
        }

        public void SerializeStatusUpdates()
        {
            List<RoomUser> Users = new List<RoomUser>();
            ICollection<RoomUser> RoomUsers = GetUserList();

            if (RoomUsers == null)
            {
                return;
            }

            foreach (RoomUser User in RoomUsers.ToList())
            {
                if (User == null || !User.UpdateNeeded || Users.Contains(User))
                {
                    continue;
                }

                User.UpdateNeeded = false;
                Users.Add(User);
            }

            if (Users.Count > 0)
            {
                _room.SendMessage(new UserUpdateComposer(Users));
            }
        }

        public void UpdateUserStatusses()
        {
            foreach (RoomUser user in GetUserList().ToList())
            {
                if (user == null)
                {
                    continue;
                }

                UpdateUserStatus(user, false);
            }
        }

        private bool isValid(RoomUser user)
        {
            if (user == null)
            {
                return false;
            }

            if (user.IsBot)
            {
                return true;
            }

            if (user.GetClient() == null)
            {
                return false;
            }

            if (user.GetClient().GetHabbo() == null)
            {
                return false;
            }

            if (user.GetClient().GetHabbo().CurrentRoomId != _room.Id)
            {
                return false;
            }

            return true;
        }


        public void OnCycle()
        {
            int userCounter = 0;

            try
            {

                List<RoomUser> ToRemove = new List<RoomUser>();
                Gamemap gameMap = _room.GetGameMap();

                foreach (RoomUser User in GetUserList().ToList())
                {
                    if (User == null)
                    {
                        continue;
                    }

                    if (!isValid(User))
                    {
                        if (User.GetClient() != null)
                        {
                            RemoveUserFromRoom(User.GetClient(), false, false);
                        }
                        else
                        {
                            RemoveRoomUser(User);
                        }
                    }

                    if (User.NeedsAutokick && !ToRemove.Contains(User))
                    {
                        ToRemove.Add(User);
                        continue;
                    }

                    //bool updated = false;
                    User.IdleTime++;

                    if (User.ChatSpamTicks >= 0)
                    {
                        User.ChatSpamTicks--;

                        if (User.ChatSpamTicks == -1)
                        {
                            User.ChatSpamCount = 0;
                        }
                    }

                    if (!User.IsBot && !User.IsAsleep && User.IdleTime >= 600)
                    {
                        if (_room.GetWired() != null)
                        {
                            _room.GetWired().TriggerEvent(WiredBoxType.TriggerUserAfk, User.GetClient().GetHabbo());
                        }

                        User.IsAsleep = true;
                        _room.SendMessage(new SleepComposer(User, true));
                    }

                    if (User.CarryItemID > 0)
                    {
                        User.CarryTimer--;
                        if (User.CarryTimer <= 0)
                        {
                            User.CarryItem(0);
                        }
                    }

                    if (_room.GotFreeze())
                    {
                        _room.GetFreeze().CycleUser(User);
                    }

                    bool InvalidStep = false;

                    if (User.isRolling)
                    {
                        if (User.rollerDelay <= 0)
                        {
                            List<Item> items = gameMap.GetCoordinatedItems(new Point(User.X, User.Y));
                            if (items.ToList().Where(x => x != null && x.GetBaseItem().InteractionType == InteractionType.ROLLER).FirstOrDefault() == null)
                                UpdateUserStatus(User, false);

                            User.isRolling = false;
                        }
                        else
                        {
                            User.rollerDelay--;
                        }
                    }

                    if (User.SetStep)
                    {
                        if (gameMap.IsValidStep2(User, new Vector2D(User.X, User.Y), new Vector2D(User.SetX, User.SetY), (User.GoalX == User.SetX && User.GoalY == User.SetY), User.AllowOverride, true) || User.RidingHorse)
                        {
                            if (!User.RidingHorse && User.GetClient() != null)
                                _room.GetGameMap().UpdateUserMovement(new Point(User.Coordinate.X, User.Coordinate.Y), new Point(User.SetX, User.SetY), User);

                            List<Item> items = gameMap.GetCoordinatedItems(new Point(User.X, User.Y));
                            foreach (Item Item in items.ToList())
                            {
                                Item.UserWalksOffFurni(User);
                            }

                            if (!User.IsBot)
                            {
                                User.X = User.SetX;
                                User.Y = User.SetY;
                                User.Z = User.SetZ;
                            }
                            else if (User.IsBot && !User.RidingHorse)
                            {
                                User.X = User.SetX;
                                User.Y = User.SetY;
                                User.Z = User.SetZ;
                            }

                            if (!User.IsBot && User.RidingHorse)
                            {
                                RoomUser Horse = GetRoomUserByVirtualId(User.HorseID);
                                if (Horse != null)
                                {
                                    Horse.X = User.SetX;
                                    Horse.Y = User.SetY;
                                    Horse.Z = User.SetZ;
                                }
                            }

                            if (User.X == gameMap.Model.DoorX && User.Y == gameMap.Model.DoorY && !ToRemove.Contains(User) && !User.IsBot)
                            {
                                ToRemove.Add(User);
                                continue;
                            }

                            List<Item> Items = gameMap.GetCoordinatedItems(new Point(User.X, User.Y));
                            foreach (Item Item in Items.ToList())
                            {
                                Item.UserWalksOnFurni(User);
                            }

                            UpdateUserStatus(User, true);
                        }
                        else
                        {
                            InvalidStep = true;
                        }

                        User.SetStep = false;
                    }

                    if (User.PathRecalcNeeded)
                    {
                        User.Path.Clear();
                        User.Path = PathFinder.FindPath(User, _room.RoomData.DiagonalEnabled, gameMap, new Vector2D(User.X, User.Y), new Vector2D(User.GoalX, User.GoalY));

                        if (User.Path.Count > 1)
                        {
                            User.PathStep = 1;
                            User.IsWalking = true;
                            User.PathRecalcNeeded = false;
                        }
                        else
                        {
                            User.PathRecalcNeeded = false;
                            User.Path.Clear();
                        }
                    }

                    if (User.IsWalking && !User.Freezed)
                    {
                        if (InvalidStep || User.PathStep >= User.Path.Count || User.GoalX == User.X && User.GoalY == User.Y)
                        {
                            User.IsWalking = false;
                            User.RemoveStatus("mv");
                            User.UserOnBall = true;

                            if (User.Statusses.ContainsKey("sign"))
                            {
                                User.RemoveStatus("sign");
                            }

                            if (User.IsBot && User.BotData.TargetUser > 0)
                            {
                                if (User.CarryItemID > 0)
                                {
                                    RoomUser Target = _room.GetRoomUserManager().GetRoomUserByHabbo(User.BotData.TargetUser);

                                    if (Target != null && Gamemap.TilesTouching(User.X, User.Y, Target.X, Target.Y))
                                    {
                                        User.SetRot(Rotation.Calculate(User.X, User.Y, Target.X, Target.Y), false);
                                        Target.SetRot(Rotation.Calculate(Target.X, Target.Y, User.X, User.Y), false);
                                        Target.CarryItem(User.CarryItemID);
                                    }
                                }

                                User.CarryItem(0);
                                User.BotData.TargetUser = 0;
                            }

                            if (User.UserToVendingMachine != null)
                            {
                                if (Gamemap.TilesTouching(User.X, User.Y, User.UserToVendingMachine.GetX, User.UserToVendingMachine.GetY))
                                {
                                    User.UnlockWalking();
                                    User.SetRot(Rotation.Calculate(User.X, User.Y, User.UserToVendingMachine.GetX, User.UserToVendingMachine.GetY), false);
                                    if (User.UserToVendingMachine.GetBaseItem().VendingIds.Count > 0)
                                    {
                                        int randomDrink = User.UserToVendingMachine.GetBaseItem().VendingIds[RandomNumber.GenerateRandom(0, (User.UserToVendingMachine.GetBaseItem().VendingIds.Count - 1))];
                                        User.CarryItem(randomDrink);
                                    }

                                    User.UserToVendingMachine.RequestUpdate(2, true);
                                    User.UserToVendingMachine.ExtraData = "1";
                                    User.UserToVendingMachine.UpdateState(false, true);


                                    Threading threading = new Threading();
                                    threading.SetSeconds(1);
                                    threading.SetAction(() =>
                                    {
                                        User.UserToVendingMachine.InteractingUser = 0;
                                        User.UserToVendingMachine.ExtraData = "0";
                                        User.UserToVendingMachine.UpdateState(false, true);
                                        User.UserToVendingMachine = null;
                                    });
                                    threading.Start();
                                }
                            }

                            if (User.RidingHorse && User.IsPet == false && !User.IsBot)
                            {
                                RoomUser HorseRiding = GetRoomUserByVirtualId(User.HorseID);
                                if (HorseRiding != null)
                                {
                                    HorseRiding.IsWalking = false;
                                    HorseRiding.RemoveStatus("mv");
                                    HorseRiding.UpdateNeeded = true;
                                }
                            }

                            User.UserHandlingBall = false;
                        }
                        else
                        {
                            Vector2D NextStep = User.Path[User.Path.Count - User.PathStep - 1];
                            if (((!gameMap.SquareIsOpen(NextStep.X, NextStep.Y, User.AllowOverride) || (gameMap.SquareHasUsers(NextStep.X, NextStep.Y, User) && _room.RoomData.RoomBlockingEnabled == 0)) && gameMap.GetCoordinatedItems(new Point(NextStep.X, NextStep.Y)).Find(x => x != null && x.GetBaseItem().InteractionType == InteractionType.GUILD_GATE) == null) && !User.AllowOverride)
                            {
                                User.RemoveStatus("mv");
                                User.Path.Clear();
                                User.Path = PathFinder.FindPath(User, _room.RoomData.DiagonalEnabled, gameMap, new Vector2D(User.X, User.Y), new Vector2D(User.GoalX, User.GoalY));
                                if (User.Path.Count > 1)
                                {
                                    User.PathStep = 1;
                                    User.IsWalking = true;
                                    User.PathRecalcNeeded = false;
                                    NextStep = User.Path[User.Path.Count - User.PathStep - 1];
                                }
                                else
                                {
                                    if (User.Statusses.ContainsKey("sign"))
                                        User.RemoveStatus("sign");

                                    User.ClearMovement(true);
                                }
                            }

                            if (User.Path.Count > 1)
                            {
                                User.PathStep++;
                                if (User.FastWalking && User.PathStep < User.Path.Count)
                                {
                                    int s2 = User.Path.Count - User.PathStep - 1;
                                    NextStep = User.Path[s2];
                                    User.PathStep++;
                                }

                                if (User.SuperFastWalking && User.PathStep < User.Path.Count)
                                {
                                    int s2 = User.Path.Count - User.PathStep - 1;
                                    NextStep = User.Path[s2];
                                    User.PathStep++;
                                    User.PathStep++;
                                }

                                int nextX = NextStep.X;
                                int nextY = NextStep.Y;
                                User.RemoveStatus("mv");

                                if (gameMap.IsValidStep2(User, new Vector2D(User.X, User.Y), NextStep, User.GoalX == nextX && User.GoalY == nextY, User.AllowOverride))
                                {
                                    double nextZ = gameMap.SqAbsoluteHeight(nextX, nextY);

                                    if (!User.IsBot)
                                    {
                                        if (User.isSitting)
                                        {
                                            User.Statusses.Remove("sit");
                                            User.Z += 0.35;
                                            User.isSitting = false;
                                            User.UpdateNeeded = true;
                                        }
                                        else if (User.isLying)
                                        {
                                            User.Statusses.Remove("sit");
                                            User.Z += 0.35;
                                            User.isLying = false;
                                            User.UpdateNeeded = true;
                                        }

                                    }

                                    if (!User.IsBot)
                                    {
                                        User.Statusses.Remove("lay");
                                        User.Statusses.Remove("sit");
                                    }

                                    if (!User.IsBot && !User.IsPet && User.GetClient() != null)
                                    {
                                        if (User.GetClient().GetHabbo().IsTeleporting)
                                        {
                                            User.GetClient().GetHabbo().IsTeleporting = false;
                                            User.GetClient().GetHabbo().TeleporterId = 0;
                                        }
                                        else if (User.GetClient().GetHabbo().IsHopping)
                                        {
                                            User.GetClient().GetHabbo().IsHopping = false;
                                            User.GetClient().GetHabbo().HopperId = 0;
                                        }
                                    }

                                    if (!User.IsBot && User.RidingHorse && User.IsPet == false)
                                    {
                                        RoomUser Horse = GetRoomUserByVirtualId(User.HorseID);
                                        if (Horse != null)
                                            Horse.AddStatus("mv", nextX + "," + nextY + "," + TextHandling.GetString(nextZ));
                                        User.AddStatus("mv", +nextX + "," + nextY + "," + TextHandling.GetString(nextZ + 1));
                                        User.UpdateNeeded = true;
                                        Horse.UpdateNeeded = true;
                                    }
                                    else
                                    {
                                        User.AddStatus("mv", nextX + "," + nextY + "," + TextHandling.GetString(nextZ));
                                    }

                                    int newRot = Rotation.Calculate(User.X, User.Y, nextX, nextY, User.moonwalkEnabled);

                                    User.RotBody = newRot;
                                    User.RotHead = newRot;

                                    User.SetStep = true;
                                    User.SetX = nextX;
                                    User.SetY = nextY;
                                    User.SetZ = nextZ;

                                    if (_room.GotSoccer())
                                        _room.GetSoccer().OnUserWalk(User);

                                    UpdateUserEffect(User, User.SetX, User.SetY);

                                    if (User.RidingHorse && User.IsPet == false && !User.IsBot)
                                    {
                                        RoomUser Horse = GetRoomUserByVirtualId(User.HorseID);
                                        if (Horse != null)
                                        {
                                            Horse.RotBody = newRot;
                                            Horse.RotHead = newRot;

                                            Horse.SetStep = true;
                                            Horse.SetX = nextX;
                                            Horse.SetY = nextY;
                                            Horse.SetZ = nextZ;
                                        }
                                    }

                                    gameMap.GameMap[User.X, User.Y] = User.SqState; // REstore the old one
                                    User.SqState = gameMap.GameMap[User.SetX, User.SetY]; //Backup the new one

                                    if (_room.RoomData.RoomBlockingEnabled == 0 && !User.RidingHorse)
                                    {
                                        gameMap.GameMap[nextX, nextY] = 0;
                                    }
                                    else
                                        gameMap.GameMap[nextX, nextY] = 1;
                                }
                            }
                        }

                        if (!User.RidingHorse)
                        {
                            User.UpdateNeeded = true;
                        }
                    }
                    else
                    {
                        if (User.Statusses.ContainsKey("mv"))
                        {
                            User.RemoveStatus("mv");
                            User.UpdateNeeded = true;

                            if (User.RidingHorse)
                            {
                                RoomUser Horse = GetRoomUserByVirtualId(User.HorseID);
                                if (Horse != null)
                                {
                                    Horse.RemoveStatus("mv");
                                    Horse.UpdateNeeded = true;
                                }
                            }
                        }
                    }

                    if (User != null && User.GetClient() != null && User.GetClient().GetHabbo().Effects() != null && User.RidingHorse && User.CurrentEffect != 77)
                        User.ApplyEffect(77);

                    if (User.IsBot && User.BotAI != null)
                    {
                        User.BotAI.OnTimerTick();
                    }
                    else
                    {
                        userCounter++;
                    }
                }

                foreach (RoomUser toRemove in ToRemove.ToList())
                {
                    GameClient client = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(toRemove.HabboId);
                    if (client != null)
                    {
                        RemoveUserFromRoom(client, true, false);
                    }
                    else
                    {
                        RemoveRoomUser(toRemove);
                    }
                }

                if (userCount != userCounter)
                {
                    UpdateUserCount(userCounter);
                }

            }
            catch (Exception e)
            {
                Logging.HandleException(e, "");
            }
        }

        public void UpdateUserStatus(RoomUser User, bool cyclegameitems)
        {
            if (User == null || this._room == null)
            {
                return;
            }

            try
            {
                bool isBot = User.IsBot;
                if (isBot)
                {
                    cyclegameitems = false;
                }

                if (StarBlueServer.GetUnixTimestamp() > StarBlueServer.GetUnixTimestamp() + User.SignTime)
                {
                    if (User.Statusses.ContainsKey("sign"))
                    {
                        User.Statusses.Remove("sign");
                        User.UpdateNeeded = true;
                    }
                }

                if ((User.Statusses.ContainsKey("lay") && !User.isLying) || (User.Statusses.ContainsKey("sit") && !User.isSitting))
                {
                    if (User.Statusses.ContainsKey("lay"))
                    {
                        User.Statusses.Remove("lay");
                    }

                    if (User.Statusses.ContainsKey("sit"))
                    {
                        User.Statusses.Remove("sit");
                    }

                    User.UpdateNeeded = true;
                }
                else if (User.isLying || User.isSitting)
                {
                    return;
                }

                List<Item> roomItemForSquare = this._room.GetGameMap().GetCoordinatedItems(new Point(User.X, User.Y)).OrderBy(p => p.GetZ).ToList();
                double newZ = !User.RidingHorse || User.IsPet ? this._room.GetGameMap().SqAbsoluteHeight(User.X, User.Y, roomItemForSquare) : this._room.GetGameMap().SqAbsoluteHeight(User.X, User.Y, roomItemForSquare) + 1.0;
                if (newZ != User.Z)
                {
                    User.Z = newZ;
                    User.UpdateNeeded = true;
                }

                foreach (Item Item in roomItemForSquare.ToList())
                {
                    if (Item == null)
                    {
                        continue;
                    }

                    if (Item.GetBaseItem().IsSeat)
                    {
                        if (!User.Statusses.ContainsKey("sit"))
                        {
                            if (!User.Statusses.ContainsKey("sit"))
                            {
                                User.Statusses.Add("sit", TextHandling.GetString(Item.GetBaseItem().Height));
                            }
                        }

                        User.Z = Item.GetZ;
                        User.RotHead = Item.Rotation;
                        User.RotBody = Item.Rotation;
                        User.UpdateNeeded = true;

                    }

                    switch (Item.GetBaseItem().InteractionType)
                    {
                        #region Beds & Tents
                        case InteractionType.BED:
                        case InteractionType.TENT_SMALL:
                            {
                                if (!User.Statusses.ContainsKey("lay"))
                                    User.Statusses.Add("lay", TextHandling.GetString(Item.GetBaseItem().Height) + " null");

                                User.Z = Item.GetZ;
                                User.RotHead = Item.Rotation;
                                User.RotBody = Item.Rotation;

                                User.UpdateNeeded = true;
                                break;
                            }
                        #endregion

                        #region Banzai Gates
                        case InteractionType.banzaigategreen:
                        case InteractionType.banzaigateblue:
                        case InteractionType.banzaigatered:
                        case InteractionType.banzaigateyellow:
                            {
                                if (cyclegameitems)
                                {
                                    int effectID = Convert.ToInt32(Item.team + 32);
                                    TeamManager t = User.GetClient().GetHabbo().CurrentRoom.GetTeamManagerForBanzai();

                                    if (User.Team == TEAM.NONE)
                                    {
                                        if (t.CanEnterOnTeam(Item.team))
                                        {
                                            if (User.Team != TEAM.NONE)
                                            {
                                                t.OnUserLeave(User);
                                            }

                                            User.Team = Item.team;

                                            t.AddUser(User);

                                            if (User.GetClient().GetHabbo().Effects().CurrentEffect != effectID)
                                            {
                                                User.GetClient().GetHabbo().Effects().ApplyEffect(effectID);
                                            }
                                        }
                                    }
                                    else if (User.Team != TEAM.NONE && User.Team != Item.team)
                                    {
                                        t.OnUserLeave(User);
                                        User.Team = TEAM.NONE;
                                        User.GetClient().GetHabbo().Effects().ApplyEffect(0);
                                    }
                                    else
                                    {
                                        //usersOnTeam--;
                                        t.OnUserLeave(User);
                                        if (User.GetClient().GetHabbo().Effects().CurrentEffect == effectID)
                                        {
                                            User.GetClient().GetHabbo().Effects().ApplyEffect(0);
                                        }

                                        User.Team = TEAM.NONE;
                                    }
                                    //Item.ExtraData = usersOnTeam.ToString();
                                    //Item.UpdateState(false, true);
                                }
                                break;
                            }
                        #endregion

                        #region Freeze Gates
                        case InteractionType.FREEZE_YELLOW_GATE:
                        case InteractionType.FREEZE_RED_GATE:
                        case InteractionType.FREEZE_GREEN_GATE:
                        case InteractionType.FREEZE_BLUE_GATE:
                            {
                                if (cyclegameitems)
                                {
                                    int effectID = Convert.ToInt32(Item.team + 39);
                                    TeamManager t = User.GetClient().GetHabbo().CurrentRoom.GetTeamManagerForFreeze();

                                    if (User.Team == TEAM.NONE)
                                    {
                                        if (t.CanEnterOnTeam(Item.team))
                                        {
                                            if (User.Team != TEAM.NONE)
                                            {
                                                t.OnUserLeave(User);
                                            }

                                            User.Team = Item.team;
                                            t.AddUser(User);

                                            if (User.GetClient().GetHabbo().Effects().CurrentEffect != effectID)
                                            {
                                                User.GetClient().GetHabbo().Effects().ApplyEffect(effectID);
                                            }
                                        }
                                    }
                                    else if (User.Team != TEAM.NONE && User.Team != Item.team)
                                    {
                                        t.OnUserLeave(User);
                                        User.Team = TEAM.NONE;
                                        User.GetClient().GetHabbo().Effects().ApplyEffect(0);
                                    }
                                    else
                                    {
                                        //usersOnTeam--;
                                        t.OnUserLeave(User);
                                        if (User.GetClient().GetHabbo().Effects().CurrentEffect == effectID)
                                        {
                                            User.GetClient().GetHabbo().Effects().ApplyEffect(0);
                                        }

                                        User.Team = TEAM.NONE;
                                    }
                                    //Item.ExtraData = usersOnTeam.ToString();
                                    //Item.UpdateState(false, true);
                                }
                                break;
                            }
                        #endregion

                        #region Banzai Teles
                        case InteractionType.banzaitele:
                            {
                                //if (User.Statusses.ContainsKey("mv"))
                                //{
                                _room.GetGameItemHandler().onTeleportRoomUserEnter(User, Item);
                                //}

                                break;
                            }
                        #endregion

                        #region Football Gate

                        #endregion

                        #region HI PROVIDER
                        case InteractionType.HI_PROVIDER:
                            {
                                if (User == null)
                                {
                                    return;
                                }

                                if (!User.IsBot)
                                {
                                    int handitem = int.Parse(Item.ExtraData);
                                    User.CarryItem(handitem);
                                    Item.UpdateState(false, true);
                                    Item.RequestUpdate(2, true);
                                }
                                break;
                            }
                        #endregion

                        #region CHESSCHAIR
                        case InteractionType.chesschair:
                        case InteractionType.SILLAGUIA:
                            {
                                if (User == null)
                                {
                                    return;
                                }

                                if (!User.IsBot)
                                {
                                    RoomUser ThisUser = User.GetClient().GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(User.GetClient().GetHabbo().Id);
                                    User.GetClient().GetHabbo().PlayingChess = true;
                                    User.GetClient().SendMessage(new MassEventComposer("habbopages/chess.txt"));
                                }
                                break;
                            }
                        #endregion

                        #region DA PROVIDER
                        case InteractionType.DA_PROVIDER:
                            {
                                if (User == null)
                                {
                                    return;
                                }

                                if (!User.IsBot)
                                {
                                    RoomUser ThisUser = User.GetClient().GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(User.GetClient().GetHabbo().Id);
                                    int dance = int.Parse(Item.ExtraData);
                                    User.GetClient().GetHabbo().CurrentRoom.SendMessage(new DanceComposer(ThisUser, dance));
                                    Item.UpdateState(false, true);
                                    Item.RequestUpdate(2, true);
                                }
                                break;
                            }
                        #endregion

                        #region Effects
                        case InteractionType.EFFECT:
                            {
                                if (User == null)
                                {
                                    return;
                                }

                                if (!User.IsBot)
                                {
                                    if (Item == null || Item.GetBaseItem() == null || User.GetClient() == null || User.GetClient().GetHabbo() == null || User.GetClient().GetHabbo().Effects() == null)
                                    {
                                        return;
                                    }

                                    if (Item.GetBaseItem().EffectId == 0 && User.GetClient().GetHabbo().Effects().CurrentEffect == 0)
                                    {
                                        return;
                                    }

                                    User.GetClient().GetHabbo().Effects().ApplyEffect(Item.GetBaseItem().EffectId);
                                    Item.ExtraData = "1";
                                    Item.UpdateState(false, true);
                                    Item.RequestUpdate(2, true);
                                }
                                break;
                            }
                        #endregion

                        #region InfoLink
                        case InteractionType.ROOM_PROVIDER:
                            {
                                {
                                    if (User == null)
                                    {
                                        return;
                                    }

                                    if (!User.IsBot)
                                    {
                                        string room = Item.ExtraData;
                                        User.GetClient().SendMessage(new MassEventComposer("habbopages/" + room + ".txt"));
                                    }
                                    break;
                                }
                            }
                        #endregion

                        #region Arrows
                        case InteractionType.ARROW:
                            {
                                if (User.GoalX == Item.GetX && User.GoalY == Item.GetY)
                                {
                                    if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                                    {
                                        continue;
                                    }


                                    if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(User.GetClient().GetHabbo().CurrentRoomId, out Room Room))
                                    {
                                        return;
                                    }

                                    if (!ItemTeleporterFinder.IsTeleLinked(Item.Id, Room))
                                    {
                                        User.UnlockWalking();
                                    }
                                    else
                                    {
                                        int LinkedTele = ItemTeleporterFinder.GetLinkedTele(Item.Id, Room);
                                        int TeleRoomId = ItemTeleporterFinder.GetTeleRoomId(LinkedTele, Room);

                                        if (TeleRoomId == Room.Id)
                                        {
                                            Item TargetItem = Room.GetRoomItemHandler().GetItem(LinkedTele);
                                            if (TargetItem == null)
                                            {
                                                if (User.GetClient() != null)
                                                {
                                                    User.GetClient().SendWhisper("Ei, algum erro aconteceu avise um administrador!");
                                                }

                                                return;
                                            }
                                            else
                                            {
                                                Room.GetGameMap().TeleportToItem(User, TargetItem);
                                            }
                                        }
                                        else if (TeleRoomId != Room.Id)
                                        {
                                            if (User != null && !User.IsBot && User.GetClient() != null && User.GetClient().GetHabbo() != null)
                                            {
                                                User.GetClient().GetHabbo().IsTeleporting = true;
                                                User.GetClient().GetHabbo().TeleportingRoomID = TeleRoomId;
                                                User.GetClient().GetHabbo().TeleporterId = LinkedTele;

                                                User.GetClient().GetHabbo().PrepareRoom(TeleRoomId, "");
                                            }
                                        }
                                        else if (_room.GetRoomItemHandler().GetItem(LinkedTele) != null)
                                        {
                                            User.SetPos(Item.GetX, Item.GetY, Item.GetZ);
                                            User.SetRot(Item.Rotation, false);
                                        }
                                        else
                                        {
                                            User.UnlockWalking();
                                        }
                                    }
                                }
                                break;
                            }
                        #endregion

                        #region Pinatas
                        case InteractionType.PINATA:
                            {
                                if (User.IsWalking && Item.ExtraData.Length > 0)
                                {
                                    int givenHits = int.Parse(Item.ExtraData);
                                    if (givenHits < 1 && User.CurrentEffect == 158)
                                    {
                                        givenHits++;
                                        Item.ExtraData = givenHits.ToString();
                                        Item.UpdateState();

                                        if (givenHits == 1)
                                        {
                                            StarBlueServer.GetGame().GetPinataManager().ReceiveCrackableReward(User, _room, Item);
                                        }

                                        #region Achievements
                                        StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_PinataWhacker", 1);
                                        StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_PinataBreaker", 1);
                                        #endregion

                                    }
                                    else
                                    {
                                        User.GetClient().SendWhisper("Ops, você não esta com a vareta! digite o comando[:efeito 158]", 34);
                                        return;
                                    }
                                }
                                break;
                            }
                        #endregion

                        #region Plantas
                        case InteractionType.PLANT_SEED:
                            {
                                if (User.IsWalking && Item.ExtraData.Length > 0)
                                {
                                    int givenHits = int.Parse(Item.ExtraData);
                                    if (givenHits < 1 && User.CurrentEffect == 192)
                                    {
                                        givenHits++;
                                        Item.ExtraData = givenHits.ToString();
                                        Item.UpdateState();

                                        if (givenHits > 5)
                                        {
                                            StarBlueServer.GetGame().GetPinataManager().ReceiveCrackableReward(User, _room, Item);
                                        }

                                        {
                                            givenHits = 0;
                                            Item.ExtraData = givenHits.ToString();
                                            Item.UpdateState();
                                            StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_PinataWhacker", 1);
                                        }
                                    }
                                }
                                break;
                            }
                        #endregion

                        case InteractionType.FOOTBALL_GATE:
                            {
                                if (!(User.IsBot || User == null || cyclegameitems))
                                {
                                    Room Room = User.GetClient().GetHabbo().CurrentRoom;
                                    if (User.GetClient().GetHabbo().LastMovFGate && User.GetClient().GetHabbo().BackupGender == User.GetClient().GetHabbo().Gender)
                                    {
                                        User.GetClient().GetHabbo().LastMovFGate = false;
                                        User.GetClient().GetHabbo().Look = User.GetClient().GetHabbo().BackupLook;
                                    }
                                    else
                                    {
                                        string _gateLook = ((User.GetClient().GetHabbo().Gender.ToUpper() == "M") ? Item.ExtraData.Split(',')[0] : Item.ExtraData.Split(',')[1]);
                                        if (_gateLook == "")
                                            break;
                                        string gateLook = "";
                                        foreach (string part in _gateLook.Split('.'))
                                        {
                                            if (part.StartsWith("hd"))
                                                continue;
                                            gateLook += part + ".";
                                        }
                                        gateLook = gateLook.Substring(0, gateLook.Length - 1);

                                        // Generating New Look.
                                        string[] Parts = User.GetClient().GetHabbo().Look.Split('.');
                                        string NewLook = "";
                                        foreach (string Part in Parts)
                                        {
                                            if (/*Part.StartsWith("hd") || */Part.StartsWith("sh") || Part.StartsWith("cp") || Part.StartsWith("cc") || Part.StartsWith("ch") || Part.StartsWith("lg") || Part.StartsWith("ca") || Part.StartsWith("wa"))
                                                continue;
                                            NewLook += Part + ".";
                                        }
                                        NewLook += gateLook;

                                        User.GetClient().GetHabbo().BackupLook = User.GetClient().GetHabbo().Look;
                                        User.GetClient().GetHabbo().BackupGender = User.GetClient().GetHabbo().Gender;
                                        User.GetClient().GetHabbo().Look = NewLook;
                                        User.GetClient().GetHabbo().LastMovFGate = true;
                                    }

                                    Room.SendMessage(new UsersComposer(User));
                                }
                                break;
                            }
                    }
                }

                if (User.isSitting && User.TeleportEnabled)
                {
                    User.Z -= 0.35;
                    User.UpdateNeeded = true;
                }

                if (cyclegameitems)
                {
                    if (_room.GotSoccer())
                    {
                        _room.GetSoccer().OnUserWalk(User);
                    }

                    if (_room.GotBanzai())
                    {
                        _room.GetBanzai().OnUserWalk(User);
                    }

                    if (_room.GotFreeze())
                    {
                        _room.GetFreeze().OnUserWalk(User);
                    }
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e.ToString());
            }
        }

        private void UpdateUserEffect(RoomUser User, int x, int y)
        {
            if (User == null || User.IsBot || User.GetClient() == null || User.GetClient().GetHabbo() == null)
            {
                return;
            }

            try
            {
                byte NewCurrentUserItemEffect = _room.GetGameMap().EffectMap[x, y];
                if (User.CurrentEffect == 39 || User.CurrentEffect == 38 && User.IsWalking)
                {
                    StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_TagC", 1);
                }
                if (User.CurrentEffect == 55 || User.CurrentEffect == 56 && User.IsWalking)
                {
                    StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_RbTagA", 1);
                }
                if (User.CurrentEffect == 97 && User.IsWalking)
                {
                    StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_snowBoardBuild", 1);
                }
                if (NewCurrentUserItemEffect > 0)
                {
                    if (User.GetClient().GetHabbo().Effects().CurrentEffect == 0)
                    {
                        User.CurrentItemEffect = ItemEffectType.NONE;
                    }

                    ItemEffectType Type = ByteToItemEffectEnum.Parse(NewCurrentUserItemEffect);
                    if (Type != User.CurrentItemEffect)
                    {
                        switch (Type)
                        {
                            case ItemEffectType.Iceskates:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(User.GetClient().GetHabbo().Gender == "M" ? 38 : 39);
                                    User.CurrentItemEffect = ItemEffectType.Iceskates;
                                    break;
                                }

                            case ItemEffectType.Normalskates:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(User.GetClient().GetHabbo().Gender == "M" ? 55 : 56);
                                    User.CurrentItemEffect = Type;
                                    break;
                                }
                            case ItemEffectType.SWIM:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(29);
                                    User.CurrentItemEffect = Type;
                                    break;
                                }
                            case ItemEffectType.SwimLow:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(30);
                                    User.CurrentItemEffect = Type;
                                    break;
                                }
                            case ItemEffectType.SwimHalloween:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(37);
                                    User.CurrentItemEffect = Type;
                                    break;
                                }

                            case ItemEffectType.SillaGuia:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(187);
                                    User.CurrentItemEffect = Type;
                                    break;
                                }

                            case ItemEffectType.SillonVIP:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(187);
                                    User.CurrentItemEffect = Type;
                                    User.GetClient().SendMessage(new MassEventComposer("habbopages/vip.txt"));
                                    break;
                                }

                            case ItemEffectType.NONE:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(-1);
                                    User.CurrentItemEffect = Type;
                                    break;
                                }
                        }
                    }
                }
                else if (User.CurrentItemEffect != ItemEffectType.NONE && NewCurrentUserItemEffect == 0)
                {
                    User.GetClient().GetHabbo().Effects().ApplyEffect(-1);
                    User.CurrentItemEffect = ItemEffectType.NONE;
                }
            }
            catch
            {
            }
        }

        public int PetCount => _pets.Count;

        public ConcurrentDictionary<int, RoomUser> GetBots()
        {
            return _bots;
        }

        public ICollection<RoomUser> GetUserList()
        {
            if (this._users == null)
                return new List<RoomUser>();

            return this._users.Values;
        }
    }
}
