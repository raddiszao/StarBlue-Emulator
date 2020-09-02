using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class AllRoomFloorCommand : IChatCommand
    {
        public string PermissionRequired => "user_16";
        public string Parameters => "";
        public string Description => "Adicione o piso que você está em cima a todos os quadrados válidos do quarto.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session))
            {
                Session.SendWhisper("Lamentamos, este comando só está disponivel para o dono do quarto.", 34);
                return;
            }

            List<Item> Items = Room.GetGameMap().GetCoordinatedItems(new Point(Session.GetRoomUser().X, Session.GetRoomUser().Y));
            foreach (Item Item in Items.ToList())
            {
                if (Item == null)
                    continue;

                if (Item.Data.InteractionType == InteractionType.EXCHANGE || (!Item.Data.IsSeat && !Item.Data.Walkable))
                {
                    Session.SendWhisper("Você não pode duplicar esse tipo de mobi.", 34);
                    return;
                }

                for (int y = 0; y < Room.RoomData.Model.MapSizeY; y++)
                {
                    for (int x = 0; x < Room.RoomData.Model.MapSizeX; x++)
                    {
                        if ((Room.RoomData.Model.DoorX != x && Room.RoomData.Model.DoorY != y) && Room.GetGameMap().GetCoordinatedItems(new Point(x, y)).Count == 0 && Room.GetGameMap().CanRollItemHere(x, y) && !Room.GetGameMap().SquareHasUsers(x, y))
                        {
                            Item _itemAdd = new Item(0, Room.Id, Item.Data.Id, Item.ExtraData, x, y, 0, Item.Rotation, Session.GetHabbo().Id, Item.GroupId, Item.LimitedNo, Item.LimitedTot, "", Room);

                            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("INSERT INTO `items` (base_item,user_id,room_id,x,y,z,wall_pos,rot,extra_data,`limited_number`,`limited_stack`) VALUES (@did,@uid,@rid,@x,@y,@z,@wall_pos,@rot,@extra_data, @limited_number, @limited_stack)");
                                dbClient.AddParameter("did", _itemAdd.Data.Id);
                                dbClient.AddParameter("uid", Session.GetHabbo().Id);
                                dbClient.AddParameter("rid", Room.Id);
                                dbClient.AddParameter("x", x);
                                dbClient.AddParameter("y", y);
                                dbClient.AddParameter("z", Room.GetGameMap().SqAbsoluteHeight(x, y));
                                dbClient.AddParameter("wall_pos", "");
                                dbClient.AddParameter("rot", _itemAdd.Rotation);
                                dbClient.AddParameter("extra_data", _itemAdd.ExtraData);
                                dbClient.AddParameter("limited_number", _itemAdd.LimitedNo);
                                dbClient.AddParameter("limited_stack", _itemAdd.LimitedTot);
                                _itemAdd.Id = Convert.ToInt32(dbClient.InsertQuery());

                                if (_itemAdd.GroupId > 0)
                                {
                                    dbClient.SetQuery("INSERT INTO `items_groups` (`id`, `group_id`) VALUES (@id, @gid)");
                                    dbClient.AddParameter("id", _itemAdd.Id);
                                    dbClient.AddParameter("gid", _itemAdd.GroupId);
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

            Room.GetGameMap().GenerateMaps();
        }
    }
}