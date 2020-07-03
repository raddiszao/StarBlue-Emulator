using StarBlue.Communication.Packets.Outgoing.Catalog;
using StarBlue.Communication.Packets.Outgoing.Inventory.Achievements;
using StarBlue.Communication.Packets.Outgoing.Notifications;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Core;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms.TraxMachine;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class UpdateCommand : IChatCommand
    {
        public string PermissionRequired => "user_16";
        public string Parameters => "[VARIABLE]";
        public string Description => "Atualiza uma parte do hotel.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Você deve inserir algo para atualizar, ex. :update catalog");
                return;
            }


            string UpdateVariable = Params[1];
            switch (UpdateVariable.ToLower())
            {
                case "calendar":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_rewards"))
                        {
                            Session.SendWhisper("Oops, surgiu um erro.");
                            break;
                        }

                        StarBlueServer.GetGame().GetCalendarManager().Init();
                        Session.SendWhisper("Calendário atualizado.");
                        break;
                    }
                case "ecotron":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_rewards"))
                        {
                            Session.SendWhisper("Oops, surgiu um erro.");
                            break;
                        }

                        StarBlueServer.GetGame().GetFurniMaticRewardsMnager().Initialize(StarBlueServer.GetDatabaseManager().GetQueryReactor());
                        Session.SendWhisper("Prêmios ecotron atualizados.");
                        break;
                    }
                case "grupos":
                case "groups":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_catalog"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o grupos.");
                            break;
                        }

                        string Message = CommandManager.MergeParams(Params, 2);

                        StarBlueServer.GetGame().GetGroupManager().Init();

                        break;
                    }

                case "cata":
                case "catalog":
                case "catalogue":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_catalog"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o catalogo.");
                            break;
                        }

                        string Message = CommandManager.MergeParams(Params, 2);

                        StarBlueServer.GetGame().GetCatalogFrontPageManager().LoadFrontPage();
                        StarBlueServer.GetGame().GetCatalog().Init(StarBlueServer.GetGame().GetItemManager());
                        StarBlueServer.GetGame().GetClientManager().SendMessage(new CatalogUpdatedComposer());
                        //StarBlueServer.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("catalogue", "O catálogo foi atualizado.", "catalog/open/" + Message + ""));

                        break;
                    }

                case "goals":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_catalog"))
                        {
                            Session.SendWhisper("Oops, você não tempermissão para atualizar o LandingCommunityGoalVS.");
                            break;
                        }

                        string Message = CommandManager.MergeParams(Params, 2);

                        StarBlueServer.GetGame().GetCommunityGoalVS().LoadCommunityGoalVS();

                        Session.SendWhisper("Você atualizou os LandingCommunityGoalVS.", 34);

                        break;
                    }

                case "pinatas":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_catalog"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o premios de las piñatas.");
                            break;
                        }

                        StarBlueServer.GetGame().GetPinataManager().Initialize(StarBlueServer.GetDatabaseManager().GetQueryReactor());
                        StarBlueServer.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("catalogue", "Se han atualizado los premios de las piñatas.", ""));
                        break;
                    }

                case "polls":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_catalog"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o premios de las piñatas.");
                            break;
                        }
                        StarBlueServer.GetGame().GetPollManager().Init();
                        break;
                    }

                case "list":
                    {
                        StringBuilder List = new StringBuilder("");
                        List.AppendLine("Lista de comandos para actualizar");
                        List.AppendLine("---------------------------------");
                        List.AppendLine(":update catalog = Atualizar o cátalogo.");
                        List.AppendLine(":update items = Atualiza os ítems, se mudou algo em 'furniture'");
                        List.AppendLine(":update models =No caso de você adicionar qualquer modelo de sala manualmente");
                        List.AppendLine(":update promotions = Atualize as notícias que estão na vista do hotel 'Server Landinds'");
                        List.AppendLine(":update filter = Atualiza o filtro, 'sempre execute se uma palavra for adicionada'");
                        List.AppendLine(":update navigator = Atualiza o Navegador");
                        List.AppendLine(":update rights = Atualiza os Permisos");
                        List.AppendLine(":update configs = Atualiza a configuração do hotel");
                        List.AppendLine(":update bans = Atualiza os banidos");
                        List.AppendLine(":update tickets = Atualiza os tickets de mod");
                        List.AppendLine(":update badge_definitions = Atualiza os emblemas adicionados");
                        List.AppendLine(":update vouchers = Acualiza os vouchers adicionadoa");
                        List.AppendLine(":update characters = Atualiza os carácteres do filtro.");
                        List.AppendLine(":update offers = Atualiza as ofertas relámpago do hotel.");
                        List.AppendLine(":update nux = Atualiza os premios nux do hotel.");
                        List.AppendLine(":update polls = Atualiza os polls do hotel.");
                        Session.SendMessage(new MOTDNotificationComposer(List.ToString()));
                        break;
                    }


                case "characters":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_filter"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o carácteres do filtro");
                            break;
                        }

                        StarBlueServer.GetGame().GetChatManager().GetFilter().InitCharacters();
                        Session.SendWhisper("Caracteres do filtro atualizados.");
                        break;
                    }

                case "items":
                case "furni":
                case "furniture":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_furni"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o furnis");
                            break;
                        }

                        StarBlueServer.GetGame().GetItemManager().Init();
                        Session.SendWhisper("Items atualizados corretamente.");
                        break;
                    }

                case "crafting":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_furni"))
                    {
                        Session.SendWhisper("Oops, você não tem permissão para atualizar o crafting.");
                        break;
                    }

                    StarBlueServer.GetGame().GetCraftingManager().Init();
                    Session.SendWhisper("Crafting atualizado corretamente.");
                    break;

                case "offers":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_furni"))
                    {
                        Session.SendWhisper("Oops, você não tem permissão para atualizar o furnis");
                        break;
                    }

                    StarBlueServer.GetGame().GetTargetedOffersManager().Initialize(StarBlueServer.GetDatabaseManager().GetQueryReactor());
                    Session.SendWhisper("Ofertas relâmpago atualizadas corretamente.");
                    break;

                case "songs":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_furni"))
                    {
                        Session.SendWhisper("Oops, você não tem permissão para atualizar as músicas.");
                        break;
                    }

                    TraxSoundManager.Init();
                    Session.SendWhisper("Você recarregou todas as músicas.");
                    break;

                case "models":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_models"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o Models");
                            break;
                        }

                        StarBlueServer.GetGame().GetRoomManager().LoadModels();
                        Session.SendWhisper("Modelos de sala atualizados corretamente.");
                        break;
                    }

                case "promotions":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_promotions"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar as promoções.");
                            break;
                        }

                        StarBlueServer.GetGame().GetLandingManager().LoadPromotions();
                        Session.SendWhisper("Noticias de vista do Hotel atualizadas corretamente.");
                        break;
                    }

                case "youtube":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_youtube"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o videos de Youtube TV.");
                            break;
                        }

                        StarBlueServer.GetGame().GetTelevisionManager().Init();
                        Session.SendWhisper("Youtube televisão atualizado corretamente");
                        break;
                    }

                case "filter":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_filter"))
                        {
                            Session.SendWhisper("Oops, Você não tem permissão para atualizar o filtro");
                            break;
                        }

                        StarBlueServer.GetGame().GetChatManager().GetFilter().InitWords();
                        StarBlueServer.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("filters", Session.GetHabbo().Username + " atualizou o filtro do hotel.", ""));
                        break;
                    }

                case "navigator":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_navigator"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o navegador.");
                            break;
                        }

                        StarBlueServer.GetGame().GetNavigator().Init();
                        StarBlueServer.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("newuser", Session.GetHabbo().Username + " você atualizou o navegador do hotel.", ""));
                        break;
                    }

                case "ranks":
                case "rights":
                case "permissions":
                case "commands":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_rights"))
                        {
                            Session.SendWhisper("Oops, você não tem direito para atualizar os direitos e permissões.");
                            break;
                        }

                        StarBlueServer.GetGame().GetPermissionManager().Init();

                        foreach (GameClient Client in StarBlueServer.GetGame().GetClientManager().GetClients.ToList())
                        {
                            if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().GetPermissions() == null)
                            {
                                continue;
                            }

                            Client.GetHabbo().GetPermissions().Init(Client.GetHabbo());
                        }

                        StarBlueServer.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " atualizou todas as permissões, comandos e cargos do hotel.", ""));
                        break;
                    }

                case "config":
                case "settings":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_configuration"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar a configuração do Hotel");
                            break;
                        }

                        StarBlueServer.GetSettingsManager().Init();
                        StarBlueServer.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " recarregou as configurações do hotel.", ""));
                        break;
                    }

                case "bans":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_bans"))
                        {
                            Session.SendWhisper("Oops, você não tem permissões para atualizar a lista de banidos");
                            break;
                        }

                        StarBlueServer.GetGame().GetModerationManager().ReCacheBans();
                        StarBlueServer.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " atualizou a lista de banidos.", ""));
                        break;
                    }

                case "quests":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_quests"))
                        {
                            Session.SendWhisper("Oops, você não tem permissões para atualizar as missões.");
                            break;
                        }

                        StarBlueServer.GetGame().GetQuestManager().Init();
                        StarBlueServer.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " atualizou todas as missões e desafios do hotel.", ""));
                        break;
                    }

                case "achievements":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_achievements"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o logros.");
                            break;
                        }

                        StarBlueServer.GetGame().GetAchievementManager().LoadAchievements();
                        StarBlueServer.GetGame().GetClientManager().SendMessage(new BadgeDefinitionsComposer(StarBlueServer.GetGame().GetAchievementManager()._achievements));
                        StarBlueServer.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " recarregou com sucesso todos os desafios e conquistas do hotel.", ""));
                        break;
                    }



                case "moderation":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_moderation"))
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_moderation' permission.");
                            break;
                        }

                        StarBlueServer.GetGame().GetModerationManager().Init();
                        StarBlueServer.GetGame().GetClientManager().ModAlert("Moderation presets have been updated. Please reload the client to view the new presets.");
                        StarBlueServer.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " configurações de permissão de moderação atualizadas.", ""));
                        break;
                    }


                case "vouchers":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_vouchers"))
                        {
                            Session.SendWhisper("Oops, você não tem permissões suficientes para atualizar os vouchers.");
                            break;
                        }

                        StarBlueServer.GetGame().GetCatalog().GetVoucherManager().Init();
                        StarBlueServer.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " atualizou os códigos de comprovante de hotel.", ""));
                        break;
                    }

                case "gc":
                case "games":
                case "gamecenter":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_game_center"))
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_game_center' permission.");
                            break;
                        }

                        StarBlueServer.GetGame().GetGameDataManager().Init();
                        StarBlueServer.GetGame().GetLeaderBoardDataManager().Init();
                        Session.SendWhisper("Game Center cache successfully updated.");
                        break;
                    }

                case "pet_locale":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_pet_locale"))
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_pet_locale' permission.");
                            break;
                        }

                        StarBlueServer.GetGame().GetChatManager().GetPetLocale().Init();
                        Session.SendWhisper("Pet locale cache successfully updated.");
                        break;
                    }

                case "locale":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_locale"))
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_locale' permission.");
                            break;
                        }

                        StarBlueServer.GetLanguageManager().Init();
                        Session.SendWhisper("Locale cache successfully updated.");
                        break;
                    }

                case "mutant":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_anti_mutant"))
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_anti_mutant' permission.");
                            break;
                        }

                        StarBlueServer.GetFigureManager().Init();
                        Session.SendWhisper("Anti mutant successfully reloaded.");
                        break;
                    }

                case "bots":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_bots"))
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_bots' permission.");
                            break;
                        }

                        StarBlueServer.GetGame().GetBotManager().Init();
                        Session.SendWhisper("Bot recargados correctamente");
                        break;
                    }

                case "rewards":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_rewards"))
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_rewards' permission.");
                            break;
                        }

                        StarBlueServer.GetGame().GetRewardManager().Reload();
                        Session.SendWhisper("Rewards managaer successfully reloaded.");
                        break;
                    }

                case "chat_styles":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_chat_styles"))
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_chat_styles' permission.");
                            break;
                        }

                        StarBlueServer.GetGame().GetChatManager().GetChatStyles().Init();
                        Session.SendWhisper("Chat Styles successfully reloaded.");
                        break;
                    }

                case "badges":
                case "badge_definitions":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_badge_definitions"))
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_badge_definitions' permission.");
                            break;
                        }

                        StarBlueServer.GetGame().GetBadgeManager().Init();
                        StarBlueServer.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("definitions", Session.GetHabbo().Username + " ha atualizado las definiciones de placas.", ""));
                        break;
                    }

                case "configuration":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_configuration"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar a configuração do Hotel");
                            break;
                        }

                        StarBlueServer._configuration = new ConfigurationData(Path.Combine(Application.StartupPath, @"config.ini"));
                        break;
                    }
                default:
                    Session.SendWhisper("'" + UpdateVariable + "' é um comando inválido.");
                    break;
            }
        }
    }
}
