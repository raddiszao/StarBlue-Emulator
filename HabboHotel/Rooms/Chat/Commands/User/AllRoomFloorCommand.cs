using StarBlue.HabboHotel.GameClients;
using System.Collections.Generic;
using StarBlue.HabboHotel.Items;
using System.Linq;
using StarBlue.Communication.Packets.Outgoing.Rooms.Session;
using System.Drawing;
using Database_Manager.Database.Session_Details.Interfaces;
using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class AllRoomFloorCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";
        public string Parameters => "";
        public string Description => "Adicione o piso que você está em cima a todos os quadrados válidos do quarto.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session))
            {
                Session.SendWhisper("Lamentamos, este comando só está disponivel para o dono do quarto.");
                return;
            }

            List<Item> Items = Room.GetGameMap().GetRoomItemForSquare(Session.GetRoomUser().X, Session.GetRoomUser().Y);
            foreach (Item Item in Items.ToList())
            {
                for (int y = 0; y < Room.GetGameMap().StaticModel.MapSizeY; y++)
                {
                    for (int x = 0; x < Room.GetGameMap().StaticModel.MapSizeX; x++)
                    {
                        if (Room.GetGameMap().GetCoordinatedItems(new Point(x, y)).Count == 0)
                        {
                            if (Room.GetGameMap().CanRollItemHere(x, y))
                            {
                                Item _itemAdd = new Item(0, Room.RoomId, Item.Data.Id, Item.ExtraData, x, y, Item.GetZ, Item.Rotation, Session.GetHabbo().Id, Item.GroupId, Item.LimitedNo, Item.LimitedTot, "", Room);

                                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("INSERT INTO `items` (base_item,user_id,room_id,x,y,z,wall_pos,rot,extra_data,`limited_number`,`limited_stack`) VALUES (@did,@uid,@rid,@x,@y,@z,@wall_pos,@rot,@extra_data, @limited_number, @limited_stack)");
                                    dbClient.AddParameter("did", Item.Data.Id);
                                    dbClient.AddParameter("uid", Session.GetHabbo().Id);
                                    dbClient.AddParameter("rid", Room.RoomId);
                                    dbClient.AddParameter("x", x);
                                    dbClient.AddParameter("y", y);
                                    dbClient.AddParameter("z", Item.GetZ);
                                    dbClient.AddParameter("wall_pos", "");
                                    dbClient.AddParameter("rot", Item.Rotation);
                                    dbClient.AddParameter("extra_data", Item.ExtraData);
                                    dbClient.AddParameter("limited_number", Item.LimitedNo);
                                    dbClient.AddParameter("limited_stack", Item.LimitedTot);
                                    _itemAdd.Id = Convert.ToInt32(dbClient.InsertQuery());

                                    if (Item.GroupId > 0)
                                    {
                                        dbClient.SetQuery("INSERT INTO `items_groups` (`id`, `group_id`) VALUES (@id, @gid)");
                                        dbClient.AddParameter("id", _itemAdd.Id);
                                        dbClient.AddParameter("gid", Item.GroupId);
                                        dbClient.RunQuery();
                                    }
                                }

                                if (_itemAdd != null)
                                {
                                    if (Room.GetRoomItemHandler().SetFloorItem(Session, _itemAdd, x, y, _itemAdd.Rotation, true, false, true))
                                    {
                                        Session.GetHabbo().GetInventoryComponent().RemoveItem(_itemAdd.Id);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Room.GetGameMap().GenerateMaps();
        }
    }
}