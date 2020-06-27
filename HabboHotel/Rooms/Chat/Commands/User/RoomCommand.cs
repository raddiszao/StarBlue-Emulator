using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using System.Text;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    class RoomCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return "[COMANDO]"; }
        }

        public string Description
        {
            get { return "Ativa ou desativa funções dentro do seu quarto."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oops, deves escolher uma opção para desativar. use o comando ':room list'");
                return;
            }

            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oops, somente o dono da sala pode usar este comando");
                return;
            }

            string Option = Params[1];
            switch (Option)
            {
                case "list":
                    {
                        StringBuilder List = new StringBuilder("");
                        List.AppendLine("Lista de comando em salas");
                        List.AppendLine("-------------------------");
                        List.AppendLine("Pet Morphs: " + (Room.PetMorphsAllowed == true ? "Habilitado" : "Desabilitado"));
                        //List.AppendLine("Pull: " + (Room.PullEnabled == true ? "Habilitado" : "Desabilitado"));
                        //List.AppendLine("Push: " + (Room.PushEnabled == true ? "Habilitado" : "Desabilitado"));
                        //List.AppendLine("Golpes: " + (Room.GolpeEnabled == true ? "Habilitado" : "Desabilitado"));
                        //List.AppendLine("Super Pull: " + (Room.SPullEnabled == true ? "Habilitado" : "Desabilitado"));
                        //List.AppendLine("Super Push: " + (Room.SPushEnabled == true ? "Habilitado" : "Desabilitado"));
                        List.AppendLine("Respect: " + (Room.RespectNotificationsEnabled == true ? "Habilitado" : "Desabilitado"));
                        //List.AppendLine("Enables: " + (Room.EnablesEnabled == true ? "Habilitado" : "Desabilitado"));
                        //List.AppendLine("Besos: " + (Room.BesosEnabled == true ? "Habilitado" : "Desabilitado"));
                        //List.AppendLine("Quemar: " + (Room.QuemarEnabled == true ? "Habilitado" : "Desabilitado"));
                        Session.SendNotification(List.ToString());
                        break;
                    }

                //case "golpe":
                //{
                //    Room.GolpeEnabled = !Room.GolpeEnabled;
                //    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                //    {
                //        dbClient.SetQuery("UPDATE `rooms` SET `golpe_enabled` = @GolpeEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                //        dbClient.AddParameter("GolpeEnabled", StarBlueServer.BoolToEnum(Room.GolpeEnabled));
                //        dbClient.RunQuery();
                //    }

                //    Session.SendWhisper("Los golpes en esta sala estan " + (Room.GolpeEnabled == true ? "Habilitado!" : "Deshabilitado!"));
                //    break;
                //}

                //case "quemar":
                //    {
                //        Room.QuemarEnabled = !Room.QuemarEnabled;
                //        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                //        {
                //            dbClient.SetQuery("UPDATE `rooms` SET `quemar_enabled` = @QuemarEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                //            dbClient.AddParameter("QuemarEnabled", StarBlueServer.BoolToEnum(Room.QuemarEnabled));
                //            dbClient.RunQuery();
                //        }

                //        Session.SendWhisper("¡El poder de quemar en esta sala está " + (Room.QuemarEnabled == true ? "habilitado!" : "deshabilitado!"));
                //        break;
                //    }

                //case "beso":
                //    {
                //        Room.BesosEnabled = !Room.BesosEnabled;
                //        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                //        {
                //            dbClient.SetQuery("UPDATE `rooms` SET `besos_enabled` = @BesosENabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                //            dbClient.AddParameter("BesosEnabled", StarBlueServer.BoolToEnum(Room.QuemarEnabled));
                //            dbClient.RunQuery();
                //        }

                //        Session.SendWhisper("¡El poder de besar en esta sala está " + (Room.QuemarEnabled == true ? "habilitado!" : "deshabilitado!"));
                //        break;
                //    }

                //case "push":
                //    {
                //        Room.PushEnabled = !Room.PushEnabled;
                //        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                //        {
                //            dbClient.SetQuery("UPDATE `rooms` SET `push_enabled` = @PushEnabled WHERE `id` = '" + Room.Id +"' LIMIT 1");
                //            dbClient.AddParameter("PushEnabled", StarBlueServer.BoolToEnum(Room.PushEnabled));
                //            dbClient.RunQuery();
                //        }

                //        Session.SendWhisper("Modo Push ahora esta " + (Room.PushEnabled == true ? "Habilitado!" : "Deshabilitado!"));
                //        break;
                //    }

                //case "spush":
                //    {
                //        Room.SPushEnabled = !Room.SPushEnabled;
                //        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                //        {
                //            dbClient.SetQuery("UPDATE `rooms` SET `spush_enabled` = @PushEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                //            dbClient.AddParameter("PushEnabled", StarBlueServer.BoolToEnum(Room.SPushEnabled));
                //            dbClient.RunQuery();
                //        }

                //        Session.SendWhisper("Modo Super Push ahora esta " + (Room.SPushEnabled == true ? "Habilitado!" : "Deshabilitado!"));
                //        break;
                //    }

                //case "spull":
                //    {
                //        Room.SPullEnabled = !Room.SPullEnabled;
                //        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                //        {
                //            dbClient.SetQuery("UPDATE `rooms` SET `spull_enabled` = @PullEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                //            dbClient.AddParameter("PullEnabled", StarBlueServer.BoolToEnum(Room.SPullEnabled));
                //            dbClient.RunQuery();
                //        }

                //        Session.SendWhisper("Modo Super Pull ahora esta  " + (Room.SPullEnabled == true ? "Habilitado!" : "Deshabilitado!"));
                //        break;
                //    }

                //case "pull":
                //    {
                //        Room.PullEnabled = !Room.PullEnabled;
                //        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                //        {
                //            dbClient.SetQuery("UPDATE `rooms` SET `pull_enabled` = @PullEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                //            dbClient.AddParameter("PullEnabled", StarBlueServer.BoolToEnum(Room.PullEnabled));
                //            dbClient.RunQuery();
                //        }

                //        Session.SendWhisper("Modo Pull ahora esta " + (Room.PullEnabled == true ? "Habilitado!" : "Deshabilitado!"));
                //        break;
                //    }

                //case "enable":
                //case "enables":
                //    {
                //        Room.EnablesEnabled = !Room.EnablesEnabled;
                //        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                //        {
                //            dbClient.SetQuery("UPDATE `rooms` SET `enables_enabled` = @EnablesEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                //            dbClient.AddParameter("EnablesEnabled", StarBlueServer.BoolToEnum(Room.EnablesEnabled));
                //            dbClient.RunQuery();
                //        }

                //        Session.SendWhisper("Los efectos en esta sala estan " + (Room.EnablesEnabled == true ? "Habilitados!" : "Deshabilitados!"));
                //        break;
                //    }

                case "respect":
                    {
                        Room.RespectNotificationsEnabled = !Room.RespectNotificationsEnabled;
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `respect_notifications_enabled` = @RespectNotificationsEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("RespectNotificationsEnabled", StarBlueServer.BoolToEnum(Room.RespectNotificationsEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("As notificações de respeito essão " + (Room.RespectNotificationsEnabled == true ? "ativadas!" : "desativadas!"));
                        break;
                    }

                case "pets":
                case "morphs":
                    {
                        Room.PetMorphsAllowed = !Room.PetMorphsAllowed;
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `pet_morphs_allowed` = @PetMorphsAllowed WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PetMorphsAllowed", StarBlueServer.BoolToEnum(Room.PetMorphsAllowed));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Que se torna animais de estimação isso " + (Room.PetMorphsAllowed == true ? "Habilitado!" : "Desabilitado!"));

                        if (!Room.PetMorphsAllowed)
                        {
                            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers())
                            {
                                if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                                {
                                    continue;
                                }

                                User.GetClient().SendWhisper("El propietario de la sala ha deshabilitado la opcion de convertirse en mascota.");
                                if (User.GetClient().GetHabbo().PetId > 0)
                                {
                                    //Tell the user what is going on.
                                    User.GetClient().SendWhisper("Oops, el dueño de la sala solo permite Usuarios normales, no mascotas..");

                                    //Change the users Pet Id.
                                    User.GetClient().GetHabbo().PetId = 0;

                                    //Quickly remove the old user instance.
                                    Room.SendMessage(new UserRemoveComposer(User.VirtualId));

                                    //Add the new one, they won't even notice a thing!!11 8-)
                                    Room.SendMessage(new UsersComposer(User));
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }
}
