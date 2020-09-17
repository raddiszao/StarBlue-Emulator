using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Generic;
using System.Linq;


namespace StarBlue.Communication.Packets.Incoming.Rooms.Settings
{
    internal class DeleteRoomEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().UsersRooms == null)
            {
                return;
            }

            int RoomId = Packet.PopInt();
            if (RoomId == 0)
            {
                return;
            }

            if (Session.GetHabbo().Rank > 3 && !Session.GetHabbo().StaffOk)
            {
                return;
            }


            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(RoomId, out Room Room))
            {
                return;
            }

            RoomData data = Room.RoomData;
            if (data == null)
            {
                return;
            }

            if (Room.RoomData.OwnerId != Session.GetHabbo().Id && !Session.GetHabbo().GetPermissions().HasRight("room_delete_any") || StarBlueServer.GoingIsToBeClose)
            {
                Session.SendNotification("Essa função foi desativada até o servidor for reinicializado.");
                return;
            }

            if (data.Group != null)
            {
                Session.SendMessage(new BroadcastMessageAlertComposer("Este quarto possui um grupo, apague-o primeiro para apagar seu quarto."));
                return;
            }

            List<Item> ItemsToRemove = new List<Item>();
            foreach (Item Item in Room.GetRoomItemHandler().GetWallAndFloor.ToList())
            {
                if (Item == null)
                {
                    continue;
                }

                if (Item.GetBaseItem().InteractionType == InteractionType.MOODLIGHT)
                {
                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunFastQuery("DELETE FROM `room_items_moodlight` WHERE `item_id` = '" + Item.Id + "' LIMIT 1");
                    }
                }

                ItemsToRemove.Add(Item);
            }

            foreach (Item Item in ItemsToRemove)
            {
                GameClient targetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Item.UserID);
                if (targetClient != null && targetClient.GetHabbo() != null)//Again, do we have an active client?
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

            StarBlueServer.GetGame().GetRoomManager().UnloadRoom(Room.Id, true);

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("DELETE rooms,user_roomvisits,user_favorites,room_rights FROM rooms LEFT JOIN user_roomvisits ON (rooms.id = user_roomvisits.room_id) LEFT JOIN user_favorites ON (rooms.id = user_favorites.room_id) LEFT JOIN room_rights ON (rooms.id = room_rights.room_id) WHERE rooms.id = " + Room.Id + "");
                dbClient.RunFastQuery("UPDATE `users` SET `home_room` = '0' WHERE `home_room` = '" + RoomId + "'");
                dbClient.RunFastQuery("DELETE FROM room_models WHERE id='" + Room.RoomData.ModelName + "' AND custom='1'");
            }

            RoomData removedRoom = (from p in Session.GetHabbo().UsersRooms where p != null && p.Id == RoomId select p).SingleOrDefault();
            if (removedRoom != null)
            {
                Session.GetHabbo().UsersRooms.Remove(removedRoom);
            }
        }
    }
}