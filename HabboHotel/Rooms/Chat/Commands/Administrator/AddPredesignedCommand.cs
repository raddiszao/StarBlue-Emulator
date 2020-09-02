using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Catalog.PredesignedRooms;
using System.Linq;
using System.Text;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator
{
    internal class AddPredesignedCommand : IChatCommand
    {
        public string PermissionRequired => "user_17";
        public string Parameters => "";
        public string Description => "Adiciona um quarto a um quarto designado";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Room == null)
            {
                return;
            }
            //if (StarBlueServer.GetGame().GetCatalog().GetPredesignedRooms().Exists((uint)Room.Id))
            //{
            //    Session.SendWhisper("O quarto não está selecionado na lista.");
            //    return;
            //}

            StringBuilder itemAmounts = new StringBuilder(), floorItemsData = new StringBuilder(), wallItemsData = new StringBuilder(),
                decoration = new StringBuilder();
            System.Collections.Generic.ICollection<Items.Item> floorItems = Room.GetRoomItemHandler().GetFloor;
            System.Collections.Generic.ICollection<Items.Item> wallItems = Room.GetRoomItemHandler().GetWall;
            foreach (Items.Item roomItem in floorItems)
            {
                int itemCount = floorItems.Count(item => item.BaseItem == roomItem.BaseItem);
                if (!itemAmounts.ToString().Contains(roomItem.BaseItem + "," + itemCount + ";"))
                {
                    itemAmounts.Append(roomItem.BaseItem + "," + itemCount + ";");
                }

                floorItemsData.Append(roomItem.BaseItem + "$$$$" + roomItem.GetX + "$$$$" + roomItem.GetY + "$$$$" + roomItem.GetZ +
                    "$$$$" + roomItem.Rotation + "$$$$" + roomItem.ExtraData + ";");
            }
            foreach (Items.Item roomItem in wallItems)
            {
                int itemCount = wallItems.Count(item => item.BaseItem == roomItem.BaseItem);
                if (!itemAmounts.ToString().Contains(roomItem.BaseItem + "," + itemCount + ";"))
                {
                    itemAmounts.Append(roomItem.BaseItem + "," + itemCount + ";");
                }

                wallItemsData.Append(roomItem.BaseItem + "$$$$" + roomItem.wallCoord + "$$$$" + roomItem.ExtraData + ";");
            }

            decoration.Append(Room.RoomData.FloorThickness + ";" + Room.RoomData.WallThickness + ";" +
                Room.RoomData.Model.WallHeight + ";" + Room.RoomData.Hidewall + ";" + Room.RoomData.Wallpaper + ";" +
                Room.RoomData.Landscape + ";" + Room.RoomData.Floor);

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO catalog_predesigned_rooms(room_model,flooritems,wallitems,catalogitems,room_id,room_decoration) VALUES('" + Room.RoomData.ModelName +
                    "', '" + floorItemsData + "', '" + wallItemsData + "', '" + itemAmounts + "', " + Room.Id + ", '" + decoration + "');");
                uint predesignedId = (uint)dbClient.InsertQuery();

                StarBlueServer.GetGame().GetCatalog().GetPredesignedRooms().predesignedRoom.Add(predesignedId,
                    new PredesignedRooms(predesignedId, (uint)Room.Id, Room.RoomData.ModelName,
                        floorItemsData.ToString().TrimEnd(';'), wallItemsData.ToString().TrimEnd(';'),
                        itemAmounts.ToString().TrimEnd(';'), decoration.ToString()));
            }

            Session.SendWhisper("A sala foi adicionada corretamente a lista.");



        }
    }
}