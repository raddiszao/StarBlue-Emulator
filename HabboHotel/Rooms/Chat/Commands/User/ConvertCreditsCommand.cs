using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Inventory.Purse;
using StarBlue.HabboHotel.Items;
using System;
using System.Data;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class ConvertCreditsCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Envia suas moedas para o banco";

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
                    Session.SendWhisper("No momento você não tem moedas no inventário!", 34);
                    return;
                }

                foreach (DataRow Row in Table.Rows)
                {
                    Item Item = Session.GetHabbo().GetInventoryComponent().GetItem(Convert.ToInt32(Row[0]));
                    if (Item == null)
                    {
                        continue;
                    }

                    if (!Item.GetBaseItem().ItemName.StartsWith("CF_"))
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
                        Session.GetHabbo().Credits += Value;
                        Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                    }
                }

                if (TotalValue > 0)
                {
                    Session.SendNotification("Todos seus créditos do inventário irão para o banco com um !\r\r(Total de: " + TotalValue + " creditos!");
                }
                else
                {
                    Session.SendNotification("Ao que parece você não pode mais fazer isso!");
                }
            }
            catch
            {
                Session.SendNotification("Oops, ocorreu um erro enquanto tentavamos converter seus creditos, contate um administrador!");
            }
        }
    }
}
