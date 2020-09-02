using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Items;
using System;
using System.Data;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class ConvertDiamondsCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Transfere seus diamantes para o banco.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            int TotalValue = 0;

            try
            {
                DataTable Table = null;
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT `id` FROM `items` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND (`room_id`=  '0' OR `room_id` = '')");
                    Table = dbClient.GetTable();
                }

                if (Table == null)
                {
                    Session.SendWhisper("Você não tem diamantes!", 34);
                    return;
                }

                foreach (DataRow Row in Table.Rows)
                {
                    Item Item = Session.GetHabbo().GetInventoryComponent().GetItem(Convert.ToInt32(Row[0]));
                    if (Item == null)
                    {
                        continue;
                    }

                    if (!Item.GetBaseItem().ItemName.StartsWith("DIAMND_") && !Item.GetBaseItem().ItemName.StartsWith("DF_") && !Item.GetBaseItem().ItemName.StartsWith("DIA_") && !Item.GetBaseItem().ItemName.StartsWith("DI_"))
                    {
                        continue;
                    }

                    if (Item.RoomId > 0)
                    {
                        continue;
                    }

                    string[] Split = Item.GetBaseItem().ItemName.Split('_');
                    int Value = int.Parse(Split[1]);

                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunFastQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }

                    Session.GetHabbo().GetInventoryComponent().RemoveItem(Item.Id);

                    TotalValue += Value;

                    if (Value > 0)
                    {
                        Session.GetHabbo().Diamonds += Value;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, Value, 5));
                    }
                }

                if (TotalValue > 0)
                {
                    Session.SendNotification("Todos os diamantes foram convertidos com exito!\r\r(Valor total: " + TotalValue + " diamantes!");
                }
                else
                {
                    Session.SendNotification("Parece que você não tem nenhuma moeda convertivel!");
                }
            }
            catch
            {
                Session.SendNotification("Opps, ocorreu um erro ao converter seus diamantes.");
            }
        }
    }
}