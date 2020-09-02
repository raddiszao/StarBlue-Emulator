using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Database.Interfaces;
using System.Text;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class RoomCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "[COMANDO]";

        public string Description => "Ativa ou desativa funções dentro do seu quarto.";

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
                        List.AppendLine("Lista de comando de quartos:");
                        List.AppendLine("-------------------------");
                        List.AppendLine("Pet Morphs: " + (Room.RoomData.PetMorphsAllowed == true ? "Habilitado" : "Desabilitado"));
                        List.AppendLine("Pull: " + (Room.RoomData.PullEnabled == true ? "Habilitado" : "Desabilitado"));
                        List.AppendLine("Push: " + (Room.RoomData.PushEnabled == true ? "Habilitado" : "Desabilitado"));
                        List.AppendLine("Golpes: " + (Room.RoomData.GolpeEnabled == true ? "Habilitado" : "Desabilitado"));
                        List.AppendLine("Super Pull: " + (Room.RoomData.SPullEnabled == true ? "Habilitado" : "Desabilitado"));
                        List.AppendLine("Super Push: " + (Room.RoomData.SPushEnabled == true ? "Habilitado" : "Desabilitado"));
                        List.AppendLine("Notificação de Respeito: " + (Room.RoomData.RespectNotificationsEnabled == true ? "Habilitado" : "Desabilitado"));
                        List.AppendLine("Enables: " + (Room.RoomData.EnablesEnabled == true ? "Habilitado" : "Desabilitado"));
                        List.AppendLine("Fastwalk: " + (Room.RoomData.FastWalkEnabled == true ? "Habilitado" : "Desabilitado"));
                        List.AppendLine("Handitem: " + (Room.RoomData.HandItemEnabled == true ? "Habilitado" : "Desabilitado"));
                        List.AppendLine("Sexo: " + (Room.RoomData.SexEnabled == true ? "Habilitado" : "Desabilitado"));
                        Session.SendNotification(List.ToString());
                        break;
                    }

                case "sexo":
                    {
                        Room.RoomData.SexEnabled = !Room.RoomData.SexEnabled;
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `sex_enabled` = @SexoEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("SexoEnabled", StarBlueServer.BoolToEnum(Room.RoomData.SexEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("O sexo neste quarto estão " + (Room.RoomData.SexEnabled == true ? "Habilitado!" : "Desabilitado!"));
                        break;
                    }

                case "golpe":
                    {
                        Room.RoomData.GolpeEnabled = !Room.RoomData.GolpeEnabled;
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `golpe_enabled` = @GolpeEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("GolpeEnabled", StarBlueServer.BoolToEnum(Room.RoomData.GolpeEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("OS golpes/socos neste quarto estão " + (Room.RoomData.GolpeEnabled == true ? "Habilitado!" : "Desabilitado!"));
                        break;
                    }

                case "handitem":
                    {
                        Room.RoomData.HandItemEnabled = !Room.RoomData.HandItemEnabled;
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `handitem_enabled` = @HanditemEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("HanditemEnabled", StarBlueServer.BoolToEnum(Room.RoomData.HandItemEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("O handitems neste quarto estão " + (Room.RoomData.HandItemEnabled == true ? "Habilitados!" : "Desabilitados!"));
                        break;
                    }

                //case "quemar":
                //    {
                //        Room.QuemarEnabled = !Room.QuemarEnabled;
                //        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                //        {
                //            dbClient.SetQuery("UPDATE `rooms` SET `quemar_enabled` = @QuemarEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                //            dbClient.AddParameter("QuemarEnabled", StarBlueServer.BoolToEnum(Room.QuemarEnabled));
                //            dbClient.RunQuery();
                //        }

                //        Session.SendWhisper("¡El poder de quemar en esta sala está " + (Room.QuemarEnabled == true ? "habilitado!" : "Desabilitado!"));
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

                //        Session.SendWhisper("¡El poder de besar en esta sala está " + (Room.QuemarEnabled == true ? "habilitado!" : "Desabilitado!"));
                //        break;
                //    }

                case "push":
                    {
                        Room.RoomData.PushEnabled = !Room.RoomData.PushEnabled;
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `push_enabled` = @PushEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PushEnabled", StarBlueServer.BoolToEnum(Room.RoomData.PushEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Push agora está " + (Room.RoomData.PushEnabled == true ? "Habilitado!" : "Desabilitado!"));
                        break;
                    }

                case "fastwalk":
                    {
                        Room.RoomData.FastWalkEnabled = !Room.RoomData.FastWalkEnabled;
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `fastwalk_enabled` = @FastWalk WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("FastWalk", StarBlueServer.BoolToEnum(Room.RoomData.FastWalkEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Fastwalk agora está " + (Room.RoomData.FastWalkEnabled == true ? "Habilitado!" : "Desabilitado!"));
                        break;
                    }

                case "spush":
                    {
                        Room.RoomData.SPushEnabled = !Room.RoomData.SPushEnabled;
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `spush_enabled` = @PushEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PushEnabled", StarBlueServer.BoolToEnum(Room.RoomData.SPushEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Super Push está " + (Room.RoomData.SPushEnabled == true ? "Habilitado!" : "Desabilitado!"));
                        break;
                    }

                case "spull":
                    {
                        Room.RoomData.SPullEnabled = !Room.RoomData.SPullEnabled;
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `spull_enabled` = @PullEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PullEnabled", StarBlueServer.BoolToEnum(Room.RoomData.SPullEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Super Pull está  " + (Room.RoomData.SPullEnabled == true ? "Habilitado!" : "Desabilitado!"));
                        break;
                    }

                case "pull":
                    {
                        Room.RoomData.PullEnabled = !Room.RoomData.PullEnabled;
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `pull_enabled` = @PullEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PullEnabled", StarBlueServer.BoolToEnum(Room.RoomData.PullEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Pull está " + (Room.RoomData.PullEnabled == true ? "Habilitado!" : "Desabilitado!"));
                        break;
                    }

                case "enable":
                case "enables":
                    {
                        Room.RoomData.EnablesEnabled = !Room.RoomData.EnablesEnabled;
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `enables_enabled` = @EnablesEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("EnablesEnabled", StarBlueServer.BoolToEnum(Room.RoomData.EnablesEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Os enables do quarto foram " + (Room.RoomData.EnablesEnabled == true ? "Habilitados!" : "Desabilitados!"));
                        break;
                    }

                case "respect":
                    {
                        Room.RoomData.RespectNotificationsEnabled = !Room.RoomData.RespectNotificationsEnabled;
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `respect_notifications_enabled` = @RespectNotificationsEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("RespectNotificationsEnabled", StarBlueServer.BoolToEnum(Room.RoomData.RespectNotificationsEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("As notificações de respeito estão " + (Room.RoomData.RespectNotificationsEnabled == true ? "ativadas!" : "desativadas!"));
                        break;
                    }

                case "pets":
                case "morphs":
                    {
                        Room.RoomData.PetMorphsAllowed = !Room.RoomData.PetMorphsAllowed;
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `pet_morphs_allowed` = @PetMorphsAllowed WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PetMorphsAllowed", StarBlueServer.BoolToEnum(Room.RoomData.PetMorphsAllowed));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Se tornar pet está " + (Room.RoomData.PetMorphsAllowed == true ? "Habilitado!" : "Desabilitado!"));

                        if (!Room.RoomData.PetMorphsAllowed)
                        {
                            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers())
                            {
                                if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                                {
                                    continue;
                                }

                                User.GetClient().SendWhisper("O proprietário do quarto desabilitou esta função.");
                                if (User.GetClient().GetHabbo().PetId > 0)
                                {
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
