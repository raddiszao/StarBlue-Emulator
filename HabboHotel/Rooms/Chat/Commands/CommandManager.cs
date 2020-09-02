using StarBlue.Communication.Packets.Outgoing.Notifications;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Items.Wired;
using StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator;
using StarBlue.HabboHotel.Rooms.Chat.Commands.Events;
using StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator;
using StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator.Fun;
using StarBlue.HabboHotel.Rooms.Chat.Commands.User;
using StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static StarBlue.Core.Rank.RankManager;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands
{
    public class CommandManager
    {
        /// <summary>
        /// Command Prefix only applies to custom commands.
        /// </summary>
        private string _prefix = ":";

        /// <summary>
        /// Commands registered for use.
        /// </summary>
        private readonly Dictionary<string, IChatCommand> _commands;
        public Dictionary<string, string> _commands2;

        /// <summary>
        /// The default initializer for the CommandManager
        /// </summary>
        public CommandManager(string Prefix)
        {
            _prefix = Prefix;
            _commands = new Dictionary<string, IChatCommand>();


            RegisterVIP();
            RegisterUser();
            RegisterEvents();
            RegisterModerator();
            RegisterAdministrator();
            //this.UpDateCommands2();

        }

        /// <summary>
        /// Request the text to parse and check for commands that need to be executed.
        /// </summary>
        /// <param name="Session">Session calling this method.</param>
        /// <param name="Message">The message to parse.</param>
        /// <returns>True if parsed or false if not.</returns>
        public bool Parse(GameClient Session, string Message)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().CurrentRoom == null)
            {
                return false;
            }

            if (StarBlueServer.GoingIsToBeClose && Session.GetHabbo().Rank < 5)
            {
                Session.SendNotification("Essa função foi desativada até o servidor for reinicializado.");
                return false;
            }

            if (!Message.StartsWith(_prefix))
            {
                return false;
            }

            Room room = Session.GetHabbo().CurrentRoom;

            if (room.GetFilter().CheckCommandFilter(Message))
            {
                return false;
            }

            if (Message == _prefix + "comandos" || Message == _prefix + "commands")
            {
                StringBuilder List = new StringBuilder();
                List<string> Commands = new List<string>();
                List.Append("[StarBlue Server] - Comandos disponíveis para você:\n\n");

                Commands = StarBlueServer.GetGame().GetPermissionManager().GetCommandsForID(1);
                foreach (string Command in Commands)
                {
                    foreach (KeyValuePair<string, IChatCommand> CmdList in _commands.ToList())
                    {
                        if (CmdList.Value.PermissionRequired == Command)
                        {
                            List.Append("\n:" + CmdList.Key + " " + CmdList.Value.Parameters + " - " + CmdList.Value.Description);
                        }
                    }
                }

                if (Session.GetHabbo().Rank >= 2)
                {
                    for (int i = 2; i < 19; i++)
                    {
                        if (!StarBlueServer.GetRankManager().TryGetValue(i, out RankData Rank))
                        {
                            continue;
                        }

                        Commands = StarBlueServer.GetGame().GetPermissionManager().GetCommandsForID(i);
                        List.Append("\n\n------------------------------------------------------------------------------\n- Comandos disponíveis para [" + Rank.Name + "] ID [" + i + "]\n\n");

                        foreach (string Command in Commands)
                        {
                            foreach (KeyValuePair<string, IChatCommand> CmdList in _commands.ToList())
                            {

                                if (CmdList.Value.PermissionRequired == Command)
                                {
                                    List.Append("\n:" + CmdList.Key + " " + CmdList.Value.Parameters + " - " + CmdList.Value.Description);
                                }
                            }
                        }
                    }

                }

                List.Append("\nTodos os comandos são registrados no banco de dados para evitar o abuso deles!");

                Session.SendMessage(new MOTDNotificationComposer(List.ToString()));

                return true;
            }

            Message = Message.Substring(1);
            string[] Split = Message.Split(' ');

            if (Split.Length == 0)
            {
                return false;
            }

            if (_commands.TryGetValue(Split[0].ToLower(), out IChatCommand Cmd))
            {
                if (Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                {
                    LogCommand(Session.GetHabbo().Id, Message, Session.GetHabbo().MachineId, Session.GetHabbo().Username, Session.GetHabbo().Look);
                }

                if (!string.IsNullOrEmpty(Cmd.PermissionRequired))
                {
                    if (!Session.GetHabbo().GetPermissions().HasCommand(Cmd.PermissionRequired))
                    {
                        return false;
                    }
                }

                Session.GetHabbo().IChatCommand = Cmd;
                Session.GetHabbo().CurrentRoom.GetWired().TriggerEvent(WiredBoxType.TriggerUserSaysCommand, Session.GetHabbo(), this);

                Cmd.Execute(Session, Session.GetHabbo().CurrentRoom, Split);
                return true;
            }
            else
            {
                string CommandSay = Split[0].ToLower().Trim();
                if (CommandSay.Length > 1)
                {
                    foreach (KeyValuePair<string, IChatCommand> _command in _commands)
                    {
                        string Command = _command.Key;
                        IChatCommand Element = _command.Value;
                        if (!string.IsNullOrEmpty(Element.PermissionRequired))
                        {
                            if (!Session.GetHabbo().GetPermissions().HasCommand(Element.PermissionRequired))
                            {
                                continue;
                            }
                        }

                        if ((Command.StartsWith(CommandSay) || CommandSay.StartsWith(Command) || Command.Contains(CommandSay) || CommandSay.Contains(Command)) && !String.IsNullOrEmpty(CommandSay))
                        {
                            Session.SendWhisper("Comando não encontrado, talvez você queira ter dito :" + Command + "?", 34);
                            break;
                        }
                    }
                }
            }

            return false;
        }

        private void RegisterVIP()
        {
            Register("superpuxar", new SuperPullCommand());
            Register("spull", new SuperPullCommand());

            Register("superempurrar", new SuperPushCommand());
            Register("spush", new SuperPushCommand());

            Register("moonwalk", new MoonwalkCommand());

            Register("balaobot", new BubbleBotCommand());

            Register("randomnumber", new RandomizeCommand());

            Register("nometamanho", new ChatHTMLSizeCommand());

            //Register("emoji", new EmojiCommand());

            Register("chatalerta", new ChatAlertCommand());

            Register("atirar", new CutCommand());
            Register("peido", new FartFaceCommand());
            Register("cortarcabeca", new CutHeadCommand());
            Register("queimar", new BurnCommand());

            Register("andarrapido", new FastwalkCommand());
            Register("fastwalk", new FastwalkCommand());

            Register("ondevende", new FindFurniCommand());

            Register("goto", new GOTOCommand());
            Register("ir", new GOTOCommand());

            Register("apostar", new SetBetCommand());

            Register("selfie", new SelfieCommand());

            Register("balao", new BubbleCommand());
        }

        private void RegisterEvents()
        {
            Register("eha", new EventAlertCommand());
            Register("evento", new EventAlertCommand());

            Register("poll", new PollCommand());
            Register("pesquisa", new PollCommand());
            Register("endpoll", new EndPollCommand());

            Register("dj", new DJAlertCommand());

            Register("masspoll", new MassPollCommand());

            Register("megaoferta", new MegaOfertCommand());
        }

        private void RegisterUser()
        {
            Register("room", new RoomCommand());

            Register("pay", new PayCommand());
            Register("pagar", new PayCommand());

            Register("tag", new PrefixNameCommand());

            Register("allroomfloor", new AllRoomFloorCommand());

            Register("chatdegrupo", new GroupChatCommand());

            Register("convertercreditos", new ConvertCreditsCommand());

            Register("converterdiamantes", new ConvertDiamondsCommand());

            Register("convertduckets", new ConvertDucketsCommand());

            Register("esconderwired", new HideWiredCommand());
            Register("hidewired", new HideWiredCommand());

            Register("cor", new ColorCommand());

            Register("builder", new BuilderToolCommand());

            Register("itemnamao", new CarryCommand());
            Register("handitem", new CarryCommand());

            Register("about", new InfoCommand());

            Register("youtube", new YoutubeCommand());

            Register("casino", new CasinoCommand());
            //Register("link", new LinkCommand());

            Register("fq", new CloseRoomCommand());

            Register("desativarsusurros", new DisableWhispersCommand());

            Register("disableevents", new DisableNotificationEventCommand());
            Register("eventosoff", new DisableNotificationEventCommand());

            Register("disablementions", new DisableMentionCommand());
            Register("mencoes", new DisableMentionCommand());

            Register("copiar", new MimicCommand());
            Register("copy", new MimicCommand());
            Register("mimic", new MimicCommand());

            Register("desativarcopiar", new DisableMimicCommand());
            Register("disablemimic", new DisableMimicCommand());

            Register("pet", new PetCommand());

            Register("lajota", new StackCommand());
            Register("lj", new StackCommand());

            Register("mutarpets", new MutePetsCommand());
            Register("mutepets", new MutePetsCommand());

            Register("mutarbots", new MuteBotsCommand());
            Register("mutebots", new MuteBotsCommand());

            Register("dancar", new DanceCommand());
            Register("dance", new DanceCommand());

            Register("empurrar", new PushCommand());
            Register("push", new PushCommand());

            Register("puxar", new PullCommand());
            Register("pull", new PullCommand());

            Register("efeito", new EnableCommand());
            Register("enable", new EnableCommand());

            Register("seguir", new FollowCommand());
            Register("follow", new FollowCommand());

            Register("limparinventario", new EmptyItemsCommand());
            Register("emptyitems", new EmptyItemsCommand());
            Register("empty", new EmptyItemsCommand());

            Register("desativarpedidos", new DisableFriendsCommand());
            Register("disablefriends", new DisableFriendsCommand());

            Register("ativarpedidos", new EnableFriendsCommand());
            Register("enablefriends", new EnableFriendsCommand());

            Register("desativarpresentes", new DisableGiftsCommand());
            Register("disablegifts", new DisableGiftsCommand());

            Register("deitar", new LayCommand());
            Register("lay", new LayCommand());

            Register("sentar", new SitCommand());
            Register("sit", new SitCommand());

            Register("hug", new HugCommand());
            Register("abracar", new HugCommand());

            Register("kikar", new KikarCommand());

            Register("sarrar", new SarrarCommand());

            Register("levantar", new StandCommand());

            Register("dnd", new DNDCommand());

            Register("pickall", new PickAllCommand());

            Register("ejectall", new EjectAllCommand());

            Register("construtor", new BuilderCommand());
            Register("build", new BuilderCommand());

            Register("recarregar", new ReloadCommand());
            Register("unload", new ReloadCommand());
            Register("reload", new ReloadCommand());

            Register("fixroom", new RegenMapsCommand());

            Register("setmax", new SetMaxCommand());

            Register("setspeed", new SetSpeedCommand());

            Register("diagonal", new DiagonalCommand());

            Register("ajuda", new HelpCommand());

            Register("beijar", new KissCommand());

            Register("golpear", new GolpeCommand());
            Register("soco", new GolpeCommand());

            Register("curar", new CurarCommand());

            Register("comprarquarto", new BuyRoomCommand());
            Register("buyroom", new BuyRoomCommand());

            Register("venderquarto", new SellRoomCommand());
            Register("sellroom", new SellRoomCommand());

            Register("matar", new KillCommand());

            Register("aus", new AfkCommand());
            Register("afk", new AfkCommand());

            Register("sexo", new SexCommand());
            Register("fumar", new FumarCommand());

            Register("closedice", new CloseDiceCommand());

        }

        private void RegisterModerator()
        {
            Register("sa", new StaffAlertCommand());

            Register("coordenadas", new CoordsCommand());

            Register("ignorarsusurro", new IgnoreWhispersCommand());

            Register("desativarefeitos", new DisableForcedFXCommand());
            Register("disableeffects", new DisableForcedFXCommand());

            Register("tele", new TeleportCommand());

            Register("override", new OverrideCommand());

            Register("andarsuperrapido", new SuperFastwalkCommand());
            Register("superfastwalk", new SuperFastwalkCommand());

            Register("forcarsentar", new ForceSitCommand());

            Register("forcardeitar", new ForceLayCommand());

            Register("forcarstand", new ForceStandCommand());

            Register("userson", new ViewOnlineCommand());

            Register("boom", new GoBoomCommand());
            Register("tptome", new TeleportToMeCommand());

            Register("summon", new SummonCommand());
            Register("trazer", new SummonCommand());

            Register("usermessage", new UserMessageCommand());

            Register("ui", new UserInfoCommand());

            Register("userinfo", new UserInfoCommand());

            Register("desmutarsala", new RoomUnmuteCommand());
            Register("roomunmute", new RoomUnmuteCommand());

            Register("mutarsala", new RoomMuteCommand());
            Register("roommute", new RoomMuteCommand());

            Register("roomalert", new RoomAlertCommand());
            Register("quartoalerta", new RoomAlertCommand());

            Register("roomkick", new RoomKickCommand());
            Register("kickartodos", new RoomKickCommand());

            Register("kickpets", new KickPetsCommand());
            Register("expulsarpets", new KickPetsCommand());

            Register("kickbots", new KickBotsCommand());
            Register("expulsarbots", new KickBotsCommand());

            Register("mutar", new MuteCommand());
            Register("mute", new MuteCommand());

            Register("desmutar", new UnmuteCommand());
            Register("unmute", new UnmuteCommand());

            Register("kickar", new KickCommand());
            Register("kick", new KickCommand());

            Register("dc", new DisconnectCommand());
            Register("desconectar", new DisconnectCommand());

            Register("alertar", new AlertCommand());

            Register("tradeban", new TradeBanCommand());

            Register("congelar", new FreezeCommand());
            Register("freeze", new FreezeCommand());

            Register("descongelar", new UnFreezeCommand());
            Register("unfreeze", new UnFreezeCommand());

            Register("ban", new BanCommand());

            Register("mip", new MIPCommand());
            Register("ipban", new IPBanCommand());

            Register("ha", new HotelAlertCommand());
            Register("hotelalert", new HotelAlertCommand());
            Register("hab", new PromotionAlertCommand());

            Register("lastmsg", new LastMessagesCommand());
            Register("verhistorico", new LastMessagesCommand());

            Register("lastconsolemsg", new LastConsoleMessagesCommand());

            Register("enviarusuario", new SendUserCommand());

            Register("makesay", new MakeSayCommand());

            Register("mudardenome", new FlagUserCommand());
            Register("flaguser", new FlagUserCommand());

            Register("flagme", new FlagMeCommand());

            Register("quartopublico", new MakePublicCommand());
            Register("makepublic", new MakePublicCommand());

            Register("quartoprivado", new MakePrivateCommand());
            Register("makeprivate", new MakePrivateCommand());

            Register("roombadge", new RoomBadgeCommand());
            Register("quartoemblema", new RoomBadgeCommand());

            Register("givebadge", new GiveBadgeCommand());
            Register("daremblema", new GiveBadgeCommand());

            Register("give", new GiveCommand());
            Register("dar", new GiveCommand());

            Register("massenable", new MassEnableCommand());
            Register("efeitostodos", new MassEnableCommand());

            Register("massdance", new MassDanceCommand());
            Register("dancartodos", new MassDanceCommand());

            Register("premiar", new RewardEventCommand());

            Register("roomgive", new RoomGiveCommand());

            Register("massbadge", new MassBadgeCommand());
            Register("hotelemblema", new MassBadgeCommand());

            Register("massgive", new MassGiveCommand());
            Register("hoteleconomia", new MassGiveCommand());
            Register("daratodos", new MassGiveCommand());

            Register("hal", new HALCommand());
            Register("alertaurl", new HALCommand());

            Register("unban", new UnBanCommand());
            Register("removerban", new UnBanCommand());

            Register("addblackword", new FilterCommand());
            Register("filtro", new FilterCommand());

            Register("rank", new GiveRanksCommand());
        }

        private void RegisterAdministrator()
        {
            Register("verclones", new ViewClonesCommand());

            Register("verinventario", new ViewInventoryCommand());

            Register("deletegroup", new DeleteGroupCommand());
            Register("deletargrupo", new DeleteGroupCommand());

            //Register("addtag", new AddTagsToUserCommand());

            Register("ca", new CustomizedHotelAlert());

            Register("summonall", new SummonAllCommand());
            Register("trazertodos", new SummonAllCommand());

            Register("givespecial", new GiveSpecialReward());

            Register("massevent", new MassiveEventCommand());

            Register("removeremblema", new RemoveBadgeCommand());
            Register("removebadge", new RemoveBadgeCommand());

            Register("ia", new SendGraphicAlertCommand());

            Register("iau", new SendImageToUserCommand());

            Register("staffson", new StaffInfoCommand());

            Register("emptyuser", new EmptyUserCommand());
            Register("limparinventariode", new EmptyUserCommand());

            Register("darvip", new SetVipCommand());
            Register("removevip", new RemoveVipCommand());
            Register("removervip", new RemoveVipCommand());
            Register("setvip", new SetVipCommand());

            Register("alerttype", new AlertSwapCommand());

            Register("addpredesigned", new AddPredesignedCommand());
            Register("addpackdesala", new AddPredesignedCommand());

            Register("removepredesigned", new RemovePredesignedCommand());
            Register("removerpackdoquarto", new RemovePredesignedCommand());

            Register("update", new UpdateCommand());
            Register("atualizar", new UpdateCommand());

            Register("item", new UpdateFurnitureCommand());

            Register("dcall", new DisconnectAllCommand());

            Register("shutdown", new ShutdownCommand());
            Register("reiniciar", new RestartCommand());
            Register("restart", new RestartCommand());

            //this.Register("voucher", new VoucherCommand());

            Register("forcebox", new ForceFurniMaticBoxCommand());

            Register("mw", new MultiwhisperModeCommand());

            Register("progresso", new ProgressAchievementCommand());

            Register("controlar", new ControlCommand());
            Register("control", new ControlCommand());

            Register("dice", new ForceDiceCommand());

            //Register("link", new LinkStaffCommand());
        }

        private void Register(string v)
        {
            throw new NotImplementedException();
        }

        public void Register(string CommandText, IChatCommand Command)
        {
            _commands.Add(CommandText, Command);
        }

        public static string MergeParams(string[] Params, int Start)
        {
            StringBuilder Merged = new StringBuilder();
            for (int i = Start; i < Params.Length; i++)
            {
                if (i > Start)
                {
                    Merged.Append(" ");
                }

                Merged.Append(Params[i]);
            }

            return Merged.ToString();
        }

        public void LogCommand(int UserId, string Data, string MachineId, string Username, string Look)
        {
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `logs_client_staff` (`user_id`,`data_string`,`machine_id`, `timestamp`) VALUES (@UserId,@Data,@MachineId,@Timestamp)");
                dbClient.AddParameter("UserId", UserId);
                dbClient.AddParameter("Data", Data);
                dbClient.AddParameter("MachineId", MachineId);
                dbClient.AddParameter("Timestamp", StarBlueServer.GetUnixTimestamp());
                dbClient.RunQuery();
            }
        }

        public bool TryGetCommand(string Command, out IChatCommand IChatCommand)
        {
            return _commands.TryGetValue(Command, out IChatCommand);
        }
    }
}
