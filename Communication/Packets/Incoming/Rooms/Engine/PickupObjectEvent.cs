using StarBlue.Communication.Packets.Outgoing.Rooms.Furni;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Quests;
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Engine
{
    internal class PickupObjectEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {

            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            if (Session.GetHabbo().Rank > 3 && !Session.GetHabbo().StaffOk || StarBlueServer.GoingIsToBeClose)
            {
                return;
            }

            int Unknown = Packet.PopInt();
            int ItemId = Packet.PopInt();

            Item Item = Room.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
            {
                return;
            }

            if (Room.ForSale)
            {
                Session.SendWhisper("Não pode editar a sala se ela está a venda.");
                Session.SendWhisper("Cancele a venda da sala digitando ':unload' (sem as '')");
                return;
            }

            if (Item.GetBaseItem().InteractionType == InteractionType.POSTIT)
            {
                return;
            }

            bool ItemRights = false;
            if (Item.UserID == Session.GetHabbo().Id || Room.CheckRights(Session, false))
            {
                ItemRights = true;
            }
            else if (Room.RoomData.Group != null && Room.CheckRights(Session, false, true))//Room has a group, this user has group rights.
            {
                ItemRights = true;
            }
            else if (Session.GetHabbo().GetPermissions().HasRight("room_item_take"))
            {
                ItemRights = true;
            }

            if (ItemRights == true)
            {
                if (Item.GetBaseItem().InteractionType == InteractionType.TENT || Item.GetBaseItem().InteractionType == InteractionType.TENT_SMALL)
                {
                    Room.RemoveTent(Item.Id, Item);
                }

                if (Item.GetBaseItem().InteractionType == InteractionType.MOODLIGHT)
                {
                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunFastQuery("DELETE FROM `room_items_moodlight` WHERE `item_id` = '" + Item.Id + "' LIMIT 1");
                    }
                }
                else if (Item.GetBaseItem().InteractionType == InteractionType.TONER)
                {
                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunFastQuery("DELETE FROM `room_items_toner` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }
                }


                if (Item.isBCitem)
                {
                    Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
                    Session.GetHabbo().GetInventoryComponent().UpdateItems(false);
                }
                else if (Item.UserID == Session.GetHabbo().Id)
                {
                    Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
                    Session.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, Item.BaseItem, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                    Session.GetHabbo().GetInventoryComponent().UpdateItems(false);
                }
                else if (Session.GetHabbo().GetPermissions().HasRight("room_item_take"))//Staff are taking this item
                {
                    Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
                    Session.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, Item.BaseItem, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                    Session.GetHabbo().GetInventoryComponent().UpdateItems(false);

                }
                else//Item is being ejected.
                {
                    GameClient targetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Item.UserID);
                    if (targetClient != null && targetClient.GetHabbo() != null && Item != null)//Again, do we have an active client?
                    {
                        Room.GetRoomItemHandler().RemoveFurniture(targetClient, Item.Id);
                        targetClient.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, Item.BaseItem, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                        targetClient.GetHabbo().GetInventoryComponent().UpdateItems(false);
                    }
                    else//No, query time.
                    {
                        Room.GetRoomItemHandler().RemoveFurniture(null, Item.Id);
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunFastQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                        }
                    }
                }

                Room.SendMessage(new UpdateStackMapMessageComposer(Room, Item.GetAffectedTiles));
                StarBlueServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_PICK);
            }
        }
    }
}