using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Communication.Packets.Outgoing.Rooms.Furni;
using StarBlue.Communication.Packets.Outgoing.WebSocket;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Catalog;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Items.Wired;
using StarBlue.HabboHotel.Quests;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Engine
{
    internal class UseFurnitureEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
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
            if (Item == null || Item.Data.InteractionType == InteractionType.DICE)
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
                        Session.SendWhisper("O mobi se encontra na página " + OutPage.Caption, 34);
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

            if (Item.GetBaseItem().InteractionType == InteractionType.banzaitele)
            {
                return;
            }

            if (Item.GetBaseItem().InteractionType == InteractionType.idol_chair)
            {
                return;
            }

            if (Item.GetBaseItem().InteractionType == InteractionType.idol_counter)
            {
                return;
            }

            if (Item.GetBaseItem().InteractionType == InteractionType.TONER)
            {
                if (!Room.CheckRights(Session, true))
                {
                    return;
                }

                if (Room.TonerData.Enabled == 0)
                {
                    Room.TonerData.Enabled = 1;
                }
                else
                {
                    Room.TonerData.Enabled = 0;
                }

                Room.SendMessage(new ObjectUpdateComposer(Item, Room.RoomData.OwnerId));

                Item.UpdateState();

                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("UPDATE `room_items_toner` SET `enabled` = '" + Room.TonerData.Enabled + "' LIMIT 1");
                }
                return;
            }

            if (Item.Data.InteractionType == InteractionType.GNOME_BOX && Item.UserID == Session.GetHabbo().Id)
            {
                Session.SendMessage(new GnomeBoxComposer(Item.Id));
            }

            bool Toggle = true;
            if (Item.GetBaseItem().InteractionType == InteractionType.WF_FLOOR_SWITCH_1 || Item.GetBaseItem().InteractionType == InteractionType.WF_FLOOR_SWITCH_2)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (User == null)
                {
                    return;
                }

                if (!Gamemap.TilesTouching(Item.GetX, Item.GetY, User.X, User.Y))
                {
                    Toggle = false;
                }
            }

            int request = Packet.RemainingLength() >= 4 ? Packet.PopInt() : 0;

            Item.Interactor.OnTrigger(Session, Item, request, hasRights);

            if (Session.GetHabbo().BuilderTool && hasRights)
            {
                Session.GetHabbo().SendWebPacket(new UpdateMobiComposer(Item.GetBaseItem().ItemName, Item.Rotation, Item.ExtraData));
            }

            if (Toggle)
            {
                Item.GetRoom().GetWired().TriggerEvent(WiredBoxType.TriggerStateChanges, Session.GetHabbo(), Item);
            }

            if (Item.Data.AdjustableHeights.Count > 0 && hasRights)
            {
                Room.SendMessage(new UpdateStackMapMessageComposer(Room, Item.GetAffectedTiles));
            }

            StarBlueServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, Item.GetBaseItem().Id);
        }
    }
}
