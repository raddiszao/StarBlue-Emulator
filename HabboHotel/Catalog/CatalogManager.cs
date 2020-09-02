using log4net;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Catalog.Clothing;
using StarBlue.HabboHotel.Catalog.Marketplace;
using StarBlue.HabboHotel.Catalog.Pets;
using StarBlue.HabboHotel.Catalog.PredesignedRooms;
using StarBlue.HabboHotel.Catalog.Vouchers;
using StarBlue.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace StarBlue.HabboHotel.Catalog
{
    public class CatalogManager
    {
        private static readonly ILog log = LogManager.GetLogger("StarBlue.HabboHotel.Catalog.CatalogManager");

        private MarketplaceManager _marketplace;
        private PetRaceManager _petRaceManager;
        private VoucherManager _voucherManager;
        private ClothingManager _clothingManager;
        private PredesignedRoomsManager _predesignedManager;

        private Dictionary<int, int> _itemOffers;
        private Dictionary<int, CatalogPage> _pages;
        private Dictionary<int, BCCatalogPage> _bcpages;
        private Dictionary<int, CatalogBot> _botPresets;
        private Dictionary<int, Dictionary<int, CatalogItem>> _items;
        private Dictionary<int, Dictionary<int, BCCatalogItem>> _bcitems;
        private Dictionary<int, Dictionary<int, CatalogDeal>> _deals;
        private Dictionary<int, PredesignedContent> _predesignedItems;

        public CatalogManager()
        {
            _marketplace = new MarketplaceManager();
            _petRaceManager = new PetRaceManager();
            _voucherManager = new VoucherManager();
            _clothingManager = new ClothingManager();
            _predesignedManager = new PredesignedRoomsManager();
            _predesignedManager.Initialize();

            _itemOffers = new Dictionary<int, int>();
            _pages = new Dictionary<int, CatalogPage>();
            _bcpages = new Dictionary<int, BCCatalogPage>();
            _botPresets = new Dictionary<int, CatalogBot>();
            _items = new Dictionary<int, Dictionary<int, CatalogItem>>();
            _bcitems = new Dictionary<int, Dictionary<int, BCCatalogItem>>();
            _deals = new Dictionary<int, Dictionary<int, CatalogDeal>>();
            _predesignedItems = new Dictionary<int, PredesignedContent>();
        }

        public void Init(ItemDataManager ItemDataManager)
        {
            if (_pages.Count > 0)
            {
                _pages.Clear();
            }

            if (_bcpages.Count > 0)
            {
                _bcpages.Clear();
            }

            if (_botPresets.Count > 0)
            {
                _botPresets.Clear();
            }

            if (_items.Count > 0)
            {
                _items.Clear();
            }

            if (_bcitems.Count > 0)
            {
                _bcitems.Clear();
            }

            if (_deals.Count > 0)
            {
                _deals.Clear();
            }

            if (_predesignedItems.Count > 0)
            {
                _predesignedItems.Clear();
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`item_id`,`catalog_name`,`cost_credits`,`cost_pixels`,`cost_diamonds`,`amount`,`page_id`,`limited_sells`,`limited_stack`,`offer_active`,`extradata`,`badge`,`offer_id`,`cost_gotw`,`predesigned_id` FROM `catalog_items`");
                DataTable CatalogueItems = dbClient.GetTable();

                if (CatalogueItems != null)
                {
                    foreach (DataRow Row in CatalogueItems.Rows)
                    {
                        if (Convert.ToInt32(Row["amount"]) <= 0)
                        {
                            continue;
                        }

                        int ItemId = Convert.ToInt32(Row["id"]);
                        int PageId = Convert.ToInt32(Row["page_id"]);
                        int BaseId = Convert.ToInt32(Row["item_id"]);
                        int OfferId = Convert.ToInt32(Row["offer_id"]);
                        uint PredesignedId = Convert.ToUInt32(Row["predesigned_id"]);
                        if (BaseId == 0 && PredesignedId > 0)
                        {
                            PredesignedRooms.PredesignedRooms roomPack = _predesignedManager.predesignedRoom[PredesignedId];
                            if (roomPack == null)
                            {
                                continue;
                            }

                            if (roomPack.CatalogItems.Contains(";"))
                            {
                                Dictionary<int, int> cataItems = new Dictionary<int, int>();
                                string[] itemArray = roomPack.CatalogItems.Split(new char[] { ';' });
                                foreach (string item in itemArray)
                                {
                                    string[] items = item.Split(',');
                                    if (!ItemDataManager.GetItem(Convert.ToInt32(items[0]), out ItemData PredesignedData))
                                    {
                                        log.Error("Catalog Bundle " + ItemId + " has no furniture data.");
                                        continue;
                                    }

                                    cataItems.Add(Convert.ToInt32(items[0]), Convert.ToInt32(items[1]));
                                }

                                _predesignedItems[PageId] = new PredesignedContent(ItemId, cataItems);
                            }
                        }

                        ItemData Data = null;
                        if (PredesignedId <= 0)
                        {
                            if (!ItemDataManager.GetItem(BaseId, out Data))
                            {
                                log.Error("Couldn't load Catalog Item " + ItemId + ", no furniture record found. {3}");
                                continue;
                            }
                        }

                        if (!_items.ContainsKey(PageId))
                        {
                            _items[PageId] = new Dictionary<int, CatalogItem>();
                        }

                        if (OfferId != -1 && !_itemOffers.ContainsKey(OfferId))
                        {
                            _itemOffers.Add(OfferId, PageId);
                        }

                        _items[PageId].Add(Convert.ToInt32(Row["id"]), new CatalogItem(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["item_id"]),
                            Data, Convert.ToString(Row["catalog_name"]), Convert.ToInt32(Row["page_id"]), Convert.ToInt32(Row["cost_credits"]), Convert.ToInt32(Row["cost_pixels"]), Convert.ToInt32(Row["cost_diamonds"]),
                            Convert.ToInt32(Row["amount"]), Convert.ToInt32(Row["limited_sells"]), Convert.ToInt32(Row["limited_stack"]), StarBlueServer.EnumToBool(Row["offer_active"].ToString()),
                            Convert.ToString(Row["extradata"]), Convert.ToString(Row["badge"]), Convert.ToInt32(Row["offer_id"]), Convert.ToInt32(Row["cost_gotw"]),
                            Convert.ToInt32(Row["predesigned_id"])));
                    }
                }


                dbClient.SetQuery("SELECT `id`,`item_id`,`catalog_name`,`cost_credits`,`cost_pixels`,`cost_diamonds`,`amount`,`page_id`,`limited_sells`,`limited_stack`,`offer_active`,`extradata`,`badge`,`offer_id`,`cost_gotw`,`predesigned_id` FROM `catalog_bc_items`");
                DataTable BCCatalogueItems = dbClient.GetTable();

                if (BCCatalogueItems != null)
                {
                    foreach (DataRow Row in BCCatalogueItems.Rows)
                    {
                        if (Convert.ToInt32(Row["amount"]) <= 0)
                        {
                            continue;
                        }

                        int ItemId = Convert.ToInt32(Row["id"]);
                        int PageId = Convert.ToInt32(Row["page_id"]);
                        int BaseId = Convert.ToInt32(Row["item_id"]);
                        int OfferId = Convert.ToInt32(Row["offer_id"]);
                        uint PredesignedId = Convert.ToUInt32(Row["predesigned_id"]);
                        if (BaseId == 0 && PredesignedId > 0)
                        {
                            PredesignedRooms.PredesignedRooms roomPack = _predesignedManager.predesignedRoom[PredesignedId];
                            if (roomPack == null)
                            {
                                continue;
                            }

                            if (roomPack.CatalogItems.Contains(";"))
                            {
                                Dictionary<int, int> cataItems = new Dictionary<int, int>();
                                string[] itemArray = roomPack.CatalogItems.Split(new char[] { ';' });
                                foreach (string item in itemArray)
                                {
                                    string[] items = item.Split(',');
                                    if (!ItemDataManager.GetItem(Convert.ToInt32(items[0]), out ItemData PredesignedData))
                                    {
                                        log.Error("Couldn't load Catalog BC Item " + ItemId + ", no furniture record found. {2}");
                                        continue;
                                    }

                                    cataItems.Add(Convert.ToInt32(items[0]), Convert.ToInt32(items[1]));
                                }

                                _predesignedItems[PageId] = new PredesignedContent(ItemId, cataItems);
                            }
                        }

                        ItemData Data = null;
                        if (PredesignedId <= 0)
                        {
                            if (!ItemDataManager.GetItem(BaseId, out Data))
                            {
                                log.Error("Couldn't load BC Item " + ItemId + ", no furniture record found. {1}");
                                continue;
                            }
                        }

                        if (!_bcitems.ContainsKey(PageId))
                        {
                            _bcitems[PageId] = new Dictionary<int, BCCatalogItem>();
                        }

                        if (OfferId != -1 && !_itemOffers.ContainsKey(OfferId))
                        {
                            _itemOffers.Add(OfferId, PageId);
                        }

                        _bcitems[PageId].Add(Convert.ToInt32(Row["id"]), new BCCatalogItem(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["item_id"]),
                            Data, Convert.ToString(Row["catalog_name"]), Convert.ToInt32(Row["page_id"]), Convert.ToInt32(Row["cost_credits"]), Convert.ToInt32(Row["cost_pixels"]), Convert.ToInt32(Row["cost_diamonds"]),
                            Convert.ToInt32(Row["amount"]), Convert.ToInt32(Row["limited_sells"]), Convert.ToInt32(Row["limited_stack"]), StarBlueServer.EnumToBool(Row["offer_active"].ToString()),
                            Convert.ToString(Row["extradata"]), Convert.ToString(Row["badge"]), Convert.ToInt32(Row["offer_id"]), Convert.ToInt32(Row["cost_gotw"]),
                            Convert.ToInt32(Row["predesigned_id"])));
                    }
                }


                dbClient.SetQuery("SELECT * FROM `catalog_deals`");
                DataTable GetDeals = dbClient.GetTable();

                if (GetDeals != null)
                {
                    foreach (DataRow Row in GetDeals.Rows)
                    {
                        int Id = Convert.ToInt32(Row["id"]);
                        int PageId = Convert.ToInt32(Row["page_id"]);
                        string Items = Convert.ToString(Row["items"]);
                        string Name = Convert.ToString(Row["name"]);
                        int Credits = Convert.ToInt32(Row["cost_credits"]);
                        int Pixels = Convert.ToInt32(Row["cost_pixels"]);

                        if (!_deals.ContainsKey(PageId))
                        {
                            _deals[PageId] = new Dictionary<int, CatalogDeal>();
                        }

                        CatalogDeal Deal = new CatalogDeal(Id, PageId, Items, Name, Credits, Pixels, ItemDataManager);
                        _deals[PageId].Add(Deal.Id, Deal);
                    }
                }

                dbClient.SetQuery("SELECT `id`,`parent_id`,`caption`,`page_link`,`visible`,`enabled`,`min_rank`,`min_vip`,`icon_image`,`page_layout`,`page_strings_1`,`page_strings_2` FROM `catalog_pages` ORDER BY `order_num`");
                DataTable CatalogPages = dbClient.GetTable();

                if (CatalogPages != null)
                {
                    foreach (DataRow Row in CatalogPages.Rows)
                    {
                        _pages.Add(Convert.ToInt32(Row["id"]), new CatalogPage(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["parent_id"]), Row["enabled"].ToString(), Convert.ToString(Row["caption"]),
                            Convert.ToString(Row["page_link"]), Convert.ToInt32(Row["icon_image"]), Convert.ToInt32(Row["min_rank"]), Convert.ToInt32(Row["min_vip"]), Row["visible"].ToString(), Convert.ToString(Row["page_layout"]),
                            Convert.ToString(Row["page_strings_1"]), Convert.ToString(Row["page_strings_2"]),
                            _items.ContainsKey(Convert.ToInt32(Row["id"])) ? _items[Convert.ToInt32(Row["id"])] : new Dictionary<int, CatalogItem>(),
                            _deals.ContainsKey(Convert.ToInt32(Row["id"])) ? _deals[Convert.ToInt32(Row["id"])] : new Dictionary<int, CatalogDeal>(),
                            _predesignedItems.ContainsKey(Convert.ToInt32(Row["id"])) ? _predesignedItems[Convert.ToInt32(Row["id"])] : null,
                            ref _itemOffers));
                    }
                }

                dbClient.SetQuery("SELECT `id`,`parent_id`,`caption`,`page_link`,`visible`,`enabled`,`min_rank`,`min_vip`,`icon_image`,`page_layout`,`page_strings_1`,`page_strings_2` FROM `catalog_bc_pages` ORDER BY `order_num`");
                DataTable BCCatalogPages = dbClient.GetTable();

                if (BCCatalogPages != null)
                {
                    foreach (DataRow Row in BCCatalogPages.Rows)
                    {
                        _bcpages.Add(Convert.ToInt32(Row["id"]), new BCCatalogPage(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["parent_id"]), Row["enabled"].ToString(), Convert.ToString(Row["caption"]),
                            Convert.ToString(Row["page_link"]), Convert.ToInt32(Row["icon_image"]), Convert.ToInt32(Row["min_rank"]), Convert.ToInt32(Row["min_vip"]), Row["visible"].ToString(), Convert.ToString(Row["page_layout"]),
                            Convert.ToString(Row["page_strings_1"]), Convert.ToString(Row["page_strings_2"]),
                            _bcitems.ContainsKey(Convert.ToInt32(Row["id"])) ? _bcitems[Convert.ToInt32(Row["id"])] : new Dictionary<int, BCCatalogItem>(),
                            _deals.ContainsKey(Convert.ToInt32(Row["id"])) ? _deals[Convert.ToInt32(Row["id"])] : new Dictionary<int, CatalogDeal>(),
                            _predesignedItems.ContainsKey(Convert.ToInt32(Row["id"])) ? _predesignedItems[Convert.ToInt32(Row["id"])] : null,
                            ref _itemOffers));
                    }
                }

                dbClient.SetQuery("SELECT `id`,`name`,`figure`,`motto`,`gender`,`ai_type` FROM `catalog_bot_presets`");
                DataTable bots = dbClient.GetTable();

                if (bots != null)
                {
                    foreach (DataRow Row in bots.Rows)
                    {
                        _botPresets.Add(Convert.ToInt32(Row[0]), new CatalogBot(Convert.ToInt32(Row[0]), Convert.ToString(Row[1]), Convert.ToString(Row[2]), Convert.ToString(Row[3]), Convert.ToString(Row[4]), Convert.ToString(Row[5])));
                    }
                }

                _petRaceManager.Init();
                _clothingManager.Init();
            }

            log.Info(">> Catalog Manager -> READY!");
        }

        public bool TryGetBot(int ItemId, out CatalogBot Bot)
        {
            return _botPresets.TryGetValue(ItemId, out Bot);
        }

        public Dictionary<int, int> ItemOffers => _itemOffers;

        public bool TryGetPage(int pageId, out CatalogPage page)
        {
            return _pages.TryGetValue(pageId, out page);
        }

        public bool TryGetBCPage(int pageId, out BCCatalogPage page)
        {
            return _bcpages.TryGetValue(pageId, out page);
        }

        public ICollection<CatalogPage> GetPages()
        {
            return _pages.Values;
        }

        public Dictionary<int, Dictionary<int, CatalogItem>> GetItems()
        {
            return _items;
        }

        public ICollection<BCCatalogPage> GetBCPages()
        {
            return _bcpages.Values;
        }

        public MarketplaceManager GetMarketplace()
        {
            return _marketplace;
        }

        public PetRaceManager GetPetRaceManager()
        {
            return _petRaceManager;
        }

        public CatalogPage TryGetPageByTemplate(string template)
        {
            return _pages.Values.Where(current => current.Template == template).First();
        }

        public CatalogPage TryGetPageByPageLink(string PageLink)
        {
            return _pages.Values.Where(current => current.PageLink == PageLink).First();
        }

        public VoucherManager GetVoucherManager()
        {
            return _voucherManager;
        }

        public ClothingManager GetClothingManager()
        {
            return _clothingManager;
        }

        internal PredesignedRoomsManager GetPredesignedRooms()
        {
            return _predesignedManager;
        }
    }
}