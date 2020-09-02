
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Catalog;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Items.Wired;
using StarBlue.HabboHotel.Quests;
using StarBlue.HabboHotel.Rooms;


namespace StarBlue.Communication.Packets.Incoming.Rooms.Engine
{
    internal class UseWallItemEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
            {
                return;
            }


            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            int itemID = Packet.PopInt();
            Item Item = Room.GetRoomItemHandler().GetItem(itemID);
            if (Item == null)
            {
                return;
            }

            if (Session.GetHabbo().FindFurniMode)
            {
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("select page_id from catalog_items where `item_id` = '" + Item.GetBaseItem().Id + "'");

                    if (StarBlueServer.GetGame().GetCatalog().TryGetPage(dbClient.GetInteger(), out CatalogPage OutPage))
                    {
                        Session.SendWhisper("O mobi se encontra na página " + OutPage.Caption + ".", 34);
                    }
                    else
                    {
                        Session.SendWhisper("Ocorreu um erro ao encontrar o mobi, tente novamente.", 34);
                    }
                }
                Session.GetHabbo().FindFurniMode = false;
                return;
            }

            bool hasRights = false;
            if (Room.CheckRights(Session, false, true))
            {
                hasRights = true;
            }

            string oldData = Item.ExtraData;
            int request = Packet.PopInt();

            Item.Interactor.OnTrigger(Session, Item, request, hasRights);
            Item.GetRoom().GetWired().TriggerEvent(WiredBoxType.TriggerStateChanges, Session.GetHabbo(), Item);

            StarBlueServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, Item.GetBaseItem().Id);

            //IMPORTANTE
        }
    }
}
