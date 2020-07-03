using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Inventory.Furni;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms.AI;
using StarBlue.HabboHotel.Users.Inventory.Bots;
using StarBlue.HabboHotel.Users.Inventory.Pets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace StarBlue.HabboHotel.Users.Inventory
{
    public class InventoryComponent
    {
        private int _userId;
        private GameClient _client;

        public int InventaryUserId;

        public ConcurrentDictionary<int, Bot> _botItems;
        public ConcurrentDictionary<int, Pet> _petsItems;
        public ConcurrentDictionary<int, Item> _floorItems;
        public ConcurrentDictionary<int, Item> _wallItems;
        public ConcurrentDictionary<int, Item> _songDisks;

        public InventoryComponent(int UserId, GameClient Client)
        {
            _client = Client;
            _userId = UserId;
            InventaryUserId = 0;

            _floorItems = new ConcurrentDictionary<int, Item>();
            _wallItems = new ConcurrentDictionary<int, Item>();
            _petsItems = new ConcurrentDictionary<int, Pet>();
            _botItems = new ConcurrentDictionary<int, Bot>();
            _songDisks = new ConcurrentDictionary<int, Item>();

            Init();
        }

        public void Init()
        {
            if (_floorItems.Count > 0)
            {
                _floorItems.Clear();
            }

            if (_wallItems.Count > 0)
            {
                _wallItems.Clear();
            }

            if (_petsItems.Count > 0)
            {
                _petsItems.Clear();
            }

            if (_botItems.Count > 0)
            {
                _botItems.Clear();
            }

            if (_songDisks.Count > 0)
            {
                _songDisks.Clear();
            }

            List<Item> Items = ItemLoader.GetItemsForUser(GetUserInventaryId());
            foreach (Item Item in Items.ToList())
            {
                if (Item.IsFloorItem)
                {
                    if (!_floorItems.TryAdd(Item.Id, Item))
                    {
                        continue;
                    }
                }
                else if (Item.IsWallItem)
                {
                    if (!_wallItems.TryAdd(Item.Id, Item))
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }

            List<Pet> Pets = PetLoader.GetPetsForUser(Convert.ToInt32(_userId));
            foreach (Pet Pet in Pets)
            {
                if (!_petsItems.TryAdd(Pet.PetId, Pet))
                {
                    Console.WriteLine("Error whilst loading pet x1: " + Pet.PetId);
                }
            }

            List<Bot> Bots = BotLoader.GetBotsForUser(Convert.ToInt32(_userId));
            foreach (Bot Bot in Bots)
            {
                if (!_botItems.TryAdd(Bot.Id, Bot))
                {
                    Console.WriteLine("Error whilst loading bot x1: " + Bot.Id);
                }
            }
        }

        public void ClearItems()
        {
            UpdateItems(true);

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("DELETE items, user_presents, room_items_moodlight, room_items_toner, room_items_tele_links, wired_items, items_groups FROM items LEFT JOIN user_presents ON(items.id = user_presents.item_id) LEFT JOIN room_items_toner ON(items.id = room_items_toner.id) LEFT JOIN room_items_moodlight ON(items.id = room_items_moodlight.item_id) LEFT JOIN room_items_tele_links ON(items.id = room_items_tele_links.tele_one_id) LEFT JOIN items_groups ON(items.id = items_groups.id) LEFT JOIN wired_items ON(items.id = wired_items.id) WHERE room_id = '0' AND user_id=" + _userId);
            }

            _floorItems.Clear();
            _wallItems.Clear();
            _songDisks.Clear();

            if (_client != null)
            {
                _client.SendMessage(new FurniListUpdateComposer());
            }
        }

        public void SetIdleState()
        {
            if (_botItems != null)
            {
                _botItems.Clear();
            }

            if (_petsItems != null)
            {
                _petsItems.Clear();
            }

            if (_floorItems != null)
            {
                _floorItems.Clear();
            }

            if (_wallItems != null)
            {
                _wallItems.Clear();
            }

            if (_songDisks != null)
            {
                _songDisks.Clear();
            }

            _botItems = null;
            _songDisks = null;
            _petsItems = null;
            _floorItems = null;
            _wallItems = null;

            _client = null;
        }


        public void UpdateItems(bool FromDatabase)
        {
            if (FromDatabase)
            {
                Init();
            }

            if (_client != null)
            {
                _client.SendMessage(new FurniListUpdateComposer());
            }
        }

        public Item GetItem(int Id)
        {

            if (_floorItems.ContainsKey(Id))
            {
                return _floorItems[Id];
            }
            else if (_wallItems.ContainsKey(Id))
            {
                return _wallItems[Id];
            }
            else if (_songDisks.ContainsKey(Id))
            {
                return _songDisks[Id];
            }

            return null;
        }

        public IEnumerable<Item> GetItems => _floorItems.Values.Concat(_wallItems.Values);

        public Item AddNewItem(int Id, int BaseItem, string ExtraData, int Group, bool ToInsert, bool FromRoom, int LimitedNumber, int LimitedStack)
        {
            if (ToInsert)
            {
                if (FromRoom)
                {
                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunFastQuery("UPDATE `items` SET `base_item` = " + BaseItem + ", `room_id` = 0, `user_id` = " + _userId + " WHERE `id` = " + Id);
                    }
                }
                else
                {
                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        if (Id > 0)
                        {
                            dbClient.RunFastQuery("INSERT INTO `items` (`id`,`base_item`, `extradata`,  `user_id`, `limited_number`, `limited_stack`) VALUES ('" + Id + "', '" + BaseItem + "', '" + ExtraData + "', '" + _userId + "', '" + LimitedNumber + "', '" + LimitedStack + "')");
                        }
                        else
                        {
                            dbClient.SetQuery("INSERT INTO `items` (`base_item`, `user_id`, `extra_data`, `limited_number`, `limited_stack`) VALUES ('" + BaseItem + "', '" + _userId + "', '" + ExtraData + "', '" + LimitedNumber + "', '" + LimitedStack + "')");
                            Id = Convert.ToInt32(dbClient.InsertQuery());
                        }

                        SendNewItems(Convert.ToInt32(Id));

                        if (Group > 0)
                        {
                            dbClient.RunFastQuery("INSERT INTO `items_groups` VALUES (" + Id + ", " + Group + ")");
                        }

                        if (!string.IsNullOrEmpty(ExtraData))
                        {
                            dbClient.SetQuery("UPDATE `items` SET `extra_data` = @extradata WHERE `id` = '" + Id + "' LIMIT 1");
                            dbClient.AddParameter("extradata", ExtraData);
                            dbClient.RunQuery();
                        }
                    }
                }
            }

            Item ItemToAdd = new Item(Id, 0, BaseItem, ExtraData, 0, 0, 0, 0, _userId, Group, LimitedNumber, LimitedStack, string.Empty);

            if (UserHoldsItem(Id))
            {
                RemoveItem(Id);
            }

            if (ItemToAdd.IsWallItem)
            {
                _wallItems.TryAdd(ItemToAdd.Id, ItemToAdd);
            }
            else
            {
                _floorItems.TryAdd(ItemToAdd.Id, ItemToAdd);
            }

            return ItemToAdd;
        }

        private bool UserHoldsItem(int itemID)
        {

            if (_floorItems.ContainsKey(itemID))
            {
                return true;
            }

            if (_wallItems.ContainsKey(itemID))
            {
                return true;
            }

            return false;
        }

        public Dictionary<int, Item> songDisks
        {
            get
            {
                Dictionary<int, Item> discs = new Dictionary<int, Item>();

                foreach (Item item in _floorItems.Values)
                {
                    if (item.GetBaseItem().InteractionType == InteractionType.MUSIC_DISC)
                    {
                        discs.Add(item.Id, item);
                    }
                }

                return discs;
            }
        }

        public bool TryAddFloorItem(int itemId, Item item)
        {
            return _floorItems.TryAdd(itemId, item);
        }

        public bool TryAddWallItem(int itemId, Item item)
        {
            return _floorItems.TryAdd(itemId, item);
        }

        public void RemoveItem(int Id)
        {
            if (GetClient() == null)
            {
                return;
            }

            if (GetClient().GetHabbo() == null || GetClient().GetHabbo().GetInventoryComponent() == null)
            {
                GetClient().Disconnect();
            }

            if (_floorItems.ContainsKey(Id))
            {
                _floorItems.TryRemove(Id, out Item ToRemove);
            }

            if (_wallItems.ContainsKey(Id))
            {
                _wallItems.TryRemove(Id, out Item ToRemove);
            }

            GetClient().SendMessage(new FurniListRemoveComposer(Id));
        }

        private GameClient GetClient()
        {
            return StarBlueServer.GetGame().GetClientManager().GetClientByUserID(_userId);
        }

        public void SendNewItems(int Id)
        {
            _client.SendMessage(new FurniListNotificationComposer(Id, 1));
        }

        #region Pet Handling
        public ICollection<Pet> GetPets()
        {
            return _petsItems.Values;
        }

        public bool TryAddPet(Pet Pet)
        {
            //TODO: Sort this mess.
            Pet.RoomId = 0;
            Pet.PlacedInRoom = false;

            return _petsItems.TryAdd(Pet.PetId, Pet);
        }

        public bool TryRemovePet(int PetId, out Pet PetItem)
        {
            if (_petsItems.ContainsKey(PetId))
            {
                return _petsItems.TryRemove(PetId, out PetItem);
            }
            else
            {
                PetItem = null;
                return false;
            }
        }

        public bool TryGetPet(int PetId, out Pet Pet)
        {
            if (_petsItems.ContainsKey(PetId))
            {
                return _petsItems.TryGetValue(PetId, out Pet);
            }
            else
            {
                Pet = null;
                return false;
            }
        }
        #endregion

        #region Bot Handling
        public ICollection<Bot> GetBots()
        {
            return _botItems.Values;
        }

        public bool TryAddBot(Bot Bot)
        {
            return _botItems.TryAdd(Bot.Id, Bot);
        }

        public bool TryRemoveBot(int BotId, out Bot Bot)
        {
            if (_botItems.ContainsKey(BotId))
            {
                return _botItems.TryRemove(BotId, out Bot);
            }
            else
            {
                Bot = null;
                return false;
            }
        }

        public bool TryGetBot(int BotId, out Bot Bot)
        {
            if (_botItems.ContainsKey(BotId))
            {
                return _botItems.TryGetValue(BotId, out Bot);
            }
            else
            {
                Bot = null;
                return false;
            }
        }
        #endregion

        public void LoadUserInventory(int InventaryId)
        {
            // dont need to update Inventary?

            InventaryUserId = InventaryId;
            UpdateItems(true);
        }

        private int GetUserInventaryId()
        {
            if (InventaryUserId > 0)
            {
                return InventaryUserId;
            }
            else
            {
                return _userId;
            }
        }

        internal Item GetFirstItemByBaseId(int id)
        {
            return _floorItems.Values.Where(item => item != null && item.GetBaseItem() != null && item.GetBaseItem().Id == id).FirstOrDefault();
        }

        public bool TryAddItem(Item item)
        {
            if (item.GetBaseItem() == null)
                return false;

            if (item.Data.Type.ToString().ToLower() == "s")// ItemType.FLOOR)
            {
                return _floorItems.TryAdd(item.Id, item);
            }

            else if (item.Data.Type.ToString().ToLower() == "i")//ItemType.WALL)
            {
                return _wallItems.TryAdd(item.Id, item);
            }
            else
            {
                throw new InvalidOperationException("Item did not match neither floor or wall item");
            }
        }

        public ICollection<Item> GetSongDisks()
        {
            return _songDisks.Values;
        }

        public IEnumerable<Item> GetWallAndFloor => _floorItems.Values.Concat(_wallItems.Values);
    }
}