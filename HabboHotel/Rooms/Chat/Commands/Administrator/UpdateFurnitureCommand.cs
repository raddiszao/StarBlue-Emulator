using StarBlue.Communication.Packets.Outgoing.Notifications;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Items;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class UpdateFurnitureCommand : IChatCommand
    {
        public string PermissionRequired => "user_16";

        public string Parameters => "(tipo) (quantidade)";

        public string Description => "Editar uma mobília.";
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Item IItem = Room.GetGameMap().GetCoordinatedItems(new Point(Session.GetRoomUser().X, Session.GetRoomUser().Y)).OrderBy(I => I.GetZ).LastOrDefault();
            if (Params.Length == 1 || Params[1] == "info")
            {
                StringBuilder Lista = new StringBuilder();
                Lista.Append("Lista de usos: (Nota: Edita TODOS os items abaixo): \r\r");
                Lista.Append("1) :item width numero (Editar para largura do item) \r");
                Lista.Append("2) :item length numero (Editar para comprimento do item) \r");
                Lista.Append("3) :item height numero (Editar a altura do Item) \r");
                Lista.Append("4) :item cansit yes/no (Permite / Não Permite sentar sobre o Item) \r");
                Lista.Append("4) :item extrarot yes/no (Permite / Não permite girar um item a força.) \r");
                Lista.Append("5) :item canwalk yes/no (Permite / Não Permite caminhar sobre o Item) \r");
                Lista.Append("6) :item canstack yes/no (Permite / Não Permite empilhar sobre o Item) \r");
                Lista.Append("7) :item interaction nome (Atribuir uma interação ao item) \r");
                Lista.Append("8) :item interactioncount numero (Atribuir o número de interações do item) \r");
                Lista.Append("9) :item interactioncount numero (Atribuir o número de interações do item) \r");
                Lista.Append("10) :item heightadj valor (Atribuir várias alturas ao item) \r\r");
                Lista.Append("11) :item effectid valor (Atribuir efeito automático ao item) \r\r");
                Lista.Append("Nota: Para atualizar o Item, você deve pegá-lo e colocá-lo na sala novamente ou atualizar o quarto.");
                Session.SendMessage(new MOTDNotificationComposer(Lista.ToString()));
                return;
            }

            string Type = Params[1].ToLower();
            string value = "";
            int numeroint = 0, FurnitureID = 0;
            double numerodouble = 0;
            DataRow Item = null;
            string opcion = "";
            switch (Type)
            {
                case "width":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                Item = dbClient.GetRow();
                                if (Item == null)
                                {
                                    return;
                                }

                                FurnitureID = Convert.ToInt32(Item[0]);
                                dbClient.RunFastQuery("UPDATE `furniture` SET `width` = '" + numeroint + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                            }
                            Session.SendWhisper("Lagura do item: " + FurnitureID + " editada com sucesso. (Valor: " + numeroint.ToString() + ")");
                            StarBlueServer.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro.");
                        }
                    }
                    break;
                case "length":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                Item = dbClient.GetRow();
                                if (Item == null)
                                {
                                    return;
                                }

                                FurnitureID = Convert.ToInt32(Item[0]);
                                dbClient.RunFastQuery("UPDATE `furniture` SET `length` = '" + numeroint + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                            }
                            Session.SendWhisper("Comprimento do item: " + FurnitureID + " editado com sucesso. (Valor: " + numeroint.ToString() + ")");
                            StarBlueServer.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro.");
                        }
                    }
                    break;
                case "height":
                    {
                        try
                        {
                            numerodouble = Convert.ToDouble(Params[2]);
                            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                Item = dbClient.GetRow();
                                if (Item == null)
                                {
                                    return;
                                }

                                FurnitureID = Convert.ToInt32(Item[0]);
                                dbClient.RunFastQuery("UPDATE `furniture` SET `stack_height` = '" + numerodouble + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                            }
                            Session.SendWhisper("Altura do item: " + FurnitureID + " editado com sucesso. (Valor: " + numerodouble.ToString() + ")");
                            StarBlueServer.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro.");
                        }
                    }
                    break;
                case "interactioncount":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                Item = dbClient.GetRow();
                                if (Item == null)
                                {
                                    return;
                                }

                                FurnitureID = Convert.ToInt32(Item[0]);
                                dbClient.RunFastQuery("UPDATE `furniture` SET `interaction_modes_count` = '" + numeroint + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                            }
                            Session.SendWhisper("Número de interações do item: " + FurnitureID + " editado com sucesso. (Valor: " + numeroint.ToString() + ")");

                            StarBlueServer.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro.");
                        }
                    }
                    break;
                case "effectid":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                Item = dbClient.GetRow();
                                if (Item == null)
                                {
                                    return;
                                }

                                FurnitureID = Convert.ToInt32(Item[0]);
                                dbClient.RunFastQuery("UPDATE `furniture` SET `effect_id` = '" + numeroint + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                            }
                            Session.SendWhisper("Efeito do item: " + FurnitureID + " editado com sucesso. (Valor: " + numeroint.ToString() + ")");

                            StarBlueServer.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro.");
                        }
                    }
                    break;
                case "vendingids":
                    {
                        try
                        {
                            value = Params[2];
                            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                Item = dbClient.GetRow();
                                if (Item == null)
                                {
                                    return;
                                }

                                FurnitureID = Convert.ToInt32(Item[0]);
                                dbClient.RunFastQuery("UPDATE `furniture` SET `vending_ids` = '" + value + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");

                                Session.SendWhisper("Vending ID do item: " + FurnitureID + " editado com sucesso. (Valor: " + value + ")");
                            }
                            StarBlueServer.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro.");
                        }
                    }
                    break;
                case "heightadj":
                    {
                        try
                        {
                            value = Params[2];
                            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                Item = dbClient.GetRow();
                                if (Item == null)
                                {
                                    return;
                                }

                                FurnitureID = Convert.ToInt32(Item[0]);
                                dbClient.RunFastQuery("UPDATE `furniture` SET `height_adjustable` = '" + value + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                            }
                            Session.SendWhisper("Alturas do item: " + FurnitureID + " editado com sucesso. (Valor: " + value + ")");

                            StarBlueServer.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro.");
                        }
                    }
                    break;
                case "cansit":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            if (!opcion.Equals("yes") && !opcion.Equals("no"))
                            {
                                Session.SendWhisper("Digite uma opção válida. (yes/no)");
                                return;
                            }
                            if (opcion.Equals("yes"))
                            {
                                opcion = "1";
                            }
                            else if (opcion.Equals("no"))
                            {
                                opcion = "0";
                            }

                            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                Item = dbClient.GetRow();
                                if (Item == null)
                                {
                                    return;
                                }

                                FurnitureID = Convert.ToInt32(Item[0]);
                                dbClient.RunFastQuery("UPDATE `furniture` SET `can_sit` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                            }
                            Session.SendWhisper("can_sit do item: " + FurnitureID + " editado com sucesso.");

                            StarBlueServer.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro.");
                        }
                    }
                    break;
                case "extrarot":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            if (!opcion.Equals("yes") && !opcion.Equals("no"))
                            {
                                Session.SendWhisper("Digite uma opção válida. (yes/no)");
                                return;
                            }
                            if (opcion.Equals("yes"))
                            {
                                opcion = "1";
                            }
                            else if (opcion.Equals("no"))
                            {
                                opcion = "0";
                            }

                            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                Item = dbClient.GetRow();
                                if (Item == null)
                                {
                                    return;
                                }

                                FurnitureID = Convert.ToInt32(Item[0]);
                                dbClient.RunFastQuery("UPDATE `furniture` SET `extra_rot` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");

                                Session.SendWhisper("extra_rot do item: " + FurnitureID + " editado com sucesso.");
                            }
                            StarBlueServer.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro.");
                        }
                    }
                    break;
                case "canstack":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            if (!opcion.Equals("yes") && !opcion.Equals("no"))
                            {
                                Session.SendWhisper("Digite uma opção válida (yes/no)");
                                return;
                            }
                            if (opcion.Equals("yes"))
                            {
                                opcion = "1";
                            }
                            else if (opcion.Equals("no"))
                            {
                                opcion = "0";
                            }

                            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                Item = dbClient.GetRow();
                                if (Item == null)
                                {
                                    return;
                                }

                                FurnitureID = Convert.ToInt32(Item[0]);
                                dbClient.RunFastQuery("UPDATE `furniture` SET `can_stack` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                            }
                            Session.SendWhisper("can_stack do item: " + FurnitureID + " editado com sucesso.");

                            StarBlueServer.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro.");
                        }
                    }
                    break;
                case "canwalk":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            if (!opcion.Equals("yes") && !opcion.Equals("no"))
                            {
                                Session.SendWhisper("Digite uma opção válida (yes/no)");
                                return;
                            }
                            if (opcion.Equals("yes"))
                            {
                                opcion = "1";
                            }
                            else if (opcion.Equals("no"))
                            {
                                opcion = "0";
                            }

                            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                Item = dbClient.GetRow();
                                if (Item == null)
                                {
                                    return;
                                }

                                FurnitureID = Convert.ToInt32(Item[0]);
                                dbClient.RunFastQuery("UPDATE `furniture` SET `is_walkable` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                            }
                            Session.SendWhisper("can_walk do item: " + FurnitureID + " editado com sucesso.");

                            StarBlueServer.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro.");
                        }
                    }
                    break;
                case "interaction":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();

                            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                Item = dbClient.GetRow();
                                if (Item == null)
                                {
                                    return;
                                }

                                FurnitureID = Convert.ToInt32(Item[0]);
                                dbClient.RunFastQuery("UPDATE `furniture` SET `interaction_type` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                            }
                            Session.SendWhisper("Interação do item: " + FurnitureID + " editada com sucesso. (Valor: " + opcion + ")");

                            StarBlueServer.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro.");
                        }
                    }
                    break;
                default:
                    {
                        Session.SendNotification("A opção digitada não existe, para saber as opções digite :item info");
                        return;
                    }
            }
        }
    }
}