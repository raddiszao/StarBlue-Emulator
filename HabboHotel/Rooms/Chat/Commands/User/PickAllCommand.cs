using StarBlue.Communication.Packets.Outgoing.Inventory.Furni;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class PickAllCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Recolhe todos os mobis do quarto.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
            {
                return;
            }

            Room.GetRoomItemHandler().RemoveItems(Session);
            Room.GetGameMap().GenerateMaps();

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `items` SET `room_id` = '0' WHERE `room_id` = @RoomId AND `user_id` = @UserId");
                dbClient.AddParameter("RoomId", Room.Id);
                dbClient.AddParameter("UserId", Session.GetHabbo().Id);
                dbClient.RunQuery();
            }

            List<Item> Items = Room.GetRoomItemHandler().GetWallAndFloor.ToList();
            if (Items.Count > 0)
            {
                Session.SendWhisper("Há mais mobis nesta sala?, expulse-os manualmente digitando :ejectall !", 34);
            }

            Session.SendMessage(new FurniListUpdateComposer());
        }
    }
}