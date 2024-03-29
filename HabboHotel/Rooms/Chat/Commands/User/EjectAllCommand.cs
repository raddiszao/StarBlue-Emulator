﻿using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class EjectAllCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Limpa todos os mobis de grupo do quarto.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Session.GetHabbo().Id == Room.RoomData.OwnerId)
            {
                //Let us check anyway.
                if (!Room.CheckRights(Session, true))
                {
                    return;
                }

                foreach (Item Item in Room.GetRoomItemHandler().GetWallAndFloor.ToList())
                {
                    if (Item == null || Item.UserID == Session.GetHabbo().Id)
                    {
                        continue;
                    }

                    GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Item.UserID);
                    if (TargetClient != null && TargetClient.GetHabbo() != null)
                    {
                        Room.GetRoomItemHandler().RemoveFurniture(TargetClient, Item.Id);
                        TargetClient.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, Item.BaseItem, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                        TargetClient.GetHabbo().GetInventoryComponent().UpdateItems(false);
                    }
                    else
                    {
                        Room.GetRoomItemHandler().RemoveFurniture(null, Item.Id);
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunFastQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                        }
                    }
                }
            }
            else
            {
                foreach (Item Item in Room.GetRoomItemHandler().GetWallAndFloor.ToList())
                {
                    if (Item == null || Item.UserID != Session.GetHabbo().Id)
                    {
                        continue;
                    }

                    GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Item.UserID);
                    if (TargetClient != null && TargetClient.GetHabbo() != null)
                    {
                        Room.GetRoomItemHandler().RemoveFurniture(TargetClient, Item.Id);
                        TargetClient.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, Item.BaseItem, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                        TargetClient.GetHabbo().GetInventoryComponent().UpdateItems(false);
                    }
                    else
                    {
                        Room.GetRoomItemHandler().RemoveFurniture(null, Item.Id);
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunFastQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                        }
                    }
                }
            }
        }
    }
}
