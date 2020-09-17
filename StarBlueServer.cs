using log4net;
using StarBlue.Communication.Encryption;
using StarBlue.Communication.Encryption.Keys;
using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Communication.WebSocket;
using StarBlue.Core;
using StarBlue.Core.FigureData;
using StarBlue.Core.Language;
using StarBlue.Core.Rank;
using StarBlue.Core.Settings;
using StarBlue.Database;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel;
using StarBlue.HabboHotel.Cache;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Users;
using StarBlue.HabboHotel.Users.UserDataManagement;
using StarBlue.Network;
using StarBlue.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace StarBlue
{
    public static class StarBlueServer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StarBlueServer));

        public const string PrettyVersion = "StarBlue Server";
        public const string PrettyBuild = "2.0";
        public static string HotelName;

        public static ConfigurationData _configuration;
        private static Encoding _defaultEncoding;
        private static Game _game;
        private static NetworkBootstrap _bootstrap;
        private static FigureDataManager _figureManager;
        private static LanguageManager _languageManager;
        private static DatabaseManager _datebasemanager;
        private static SettingsManager _settingsManager;
        private static WebSocketManager _webSocketManager;
        private static RankManager _rankManager;
        public static CultureInfo CultureInfo;

        public static bool GoingIsToBeClose = false;
        public static bool Event = false;
        public static DateTime LastEvent;
        public static DateTime ServerStarted;
        public static string LastUpdate;

        private static readonly List<char> Allowedchars = new List<char>(new[]
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
                'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
                'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '.'
            });

        private static ConcurrentDictionary<int, Habbo> _usersCached = new ConcurrentDictionary<int, Habbo>();


        public static string SWFRevision = "PRODUCTION-201609061203-935497134";

        public static void Initialize()
        {
            LastUpdate = Convert.ToString(File.GetLastWriteTime(Path.Combine(Directory.GetCurrentDirectory(), "StarBlue.exe")));

            Console.SetWindowSize(97, 43);
            _configuration = new ConfigurationData(Path.Combine(Application.StartupPath, @"./config/config.ini"));
            HotelName = Convert.ToString(GetConfig().data["hotel.name"]);

            ServerStarted = DateTime.Now;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine();
            Console.WriteLine("  " + PrettyVersion + " ~ " + PrettyBuild + ".");
            Console.WriteLine(@"  " + "© 2020 - Server of " + HotelName + " Hotel.");
            Console.WriteLine();
            Console.WriteLine(@"_________________________________________________________________________________________________");
            Console.WriteLine();
            Console.WriteLine();
            _defaultEncoding = Encoding.Default;

            CultureInfo = CultureInfo.CreateSpecificCulture("en-GB");

            try
            {
                _datebasemanager = new DatabaseManager(uint.Parse(GetConfig().data["db.pool.maxsize"]), uint.Parse(GetConfig().data["db.pool.minsize"]), GetConfig().data["db.hostname"], uint.Parse(GetConfig().data["db.port"]), GetConfig().data["db.username"], GetConfig().data["db.password"], GetConfig().data["db.name"]);
                bool DatabaseConnected = _datebasemanager.IsConnected();

                int TryCount = 0;
                while (!DatabaseConnected)
                {
                    TryCount++;
                    Logging.WriteLine("Failed to connect to the specified MySQL server.");
                    Thread.Sleep(5000);

                    if (TryCount > 10)
                    {
                        Console.ReadKey(true);
                        Environment.Exit(1);
                        return;
                    }
                }

                if (DatabaseConnected)
                {
                    log.Info(">> Connection to MySQL server was successfully.");
                }

                //Reset our statistics first.
                using (IQueryAdapter dbClient = GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("TRUNCATE `catalog_marketplace_data`");
                    dbClient.RunFastQuery("UPDATE `rooms` SET `users_now` = '0' WHERE `users_now` > '0';");
                    dbClient.RunFastQuery("UPDATE `users` SET `online` = '0' WHERE `online` = '1'");
                }

                //Have our encryption ready.
                HabboEncryptionV2.Initialize(new RSAKeys());

                _settingsManager = new SettingsManager();
                _settingsManager.Init();

                _figureManager = new FigureDataManager();
                _figureManager.Init();

                _languageManager = new LanguageManager();
                _languageManager.Init();

                _rankManager = new RankManager();
                _rankManager.Init();

                _game = new Game();
                _game.StartGameLoop();

                _webSocketManager = new WebSocketManager(527, int.Parse(GetConfig().data["game.tcp.conperip"]));

                _bootstrap = new NetworkBootstrap(GetConfig().data["game.tcp.bindip"], GetConfig().data["game.tcp.port"].Split(','));
                _bootstrap.InitAsync().Wait();

                TimeSpan TimeUsed = DateTime.Now - ServerStarted;
                Logging.WriteLine(">> STARBLUE SERVER -> OK! (" + TimeUsed.Seconds + " s, " + TimeUsed.Milliseconds + " ms)", ConsoleColor.DarkGray);
            }
            catch (KeyNotFoundException e)
            {
                Logging.WriteLine("Please check your configuration file - some values appear to be missing.", ConsoleColor.Red);
                Logging.WriteLine("Press any key to shut down ...");
                Logging.WriteLine(e.ToString());
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
            catch (InvalidOperationException e)
            {
                Logging.WriteLine("Failed to initialize StarBlue Emulator: " + e.Message, ConsoleColor.Red);
                Logging.WriteLine("Press any key to shut down ...");
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
            catch (Exception e)
            {
                Logging.WriteLine("Fatal error during startup: " + e, ConsoleColor.Red);
                Logging.WriteLine("Press a key to exit");

                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        public static bool EnumToBool(string Enum)
        {
            return (Enum == "1");
        }

        public static string BoolToEnum(bool Bool)
        {
            return (Bool == true ? "1" : "0");
        }

        public static int GetRandomNumber(int Min, int Max)
        {
            return RandomNumber.GenerateNewRandom(Min, Max);
        }

        public static double GetUnixTimestamp()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return ts.TotalSeconds;
        }

        internal static int GetIUnixTimestamp()
        {
            var ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            var unixTime = ts.TotalSeconds;
            return Convert.ToInt32(unixTime);
        }

        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public static long Now()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            double unixTime = ts.TotalMilliseconds;
            return (long)unixTime;
        }

        public static string FilterFigure(string figure)
        {
            foreach (char character in figure)
            {
                if (!isValid(character))
                {
                    return "sh-3338-93.ea-1406-62.hr-831-49.ha-3331-92.hd-180-7.ch-3334-93-1408.lg-3337-92.ca-1813-62";
                }
            }

            return figure;
        }

        internal static bool IsNum(string Int)
        {
            bool isNum = double.TryParse(Int, out double Num);
            return isNum;
        }

        private static bool isValid(char character)
        {
            return Allowedchars.Contains(character);
        }

        public static bool IsValidAlphaNumeric(string inputStr)
        {
            inputStr = inputStr.ToLower();
            if (string.IsNullOrEmpty(inputStr))
            {
                return false;
            }

            for (int i = 0; i < inputStr.Length; i++)
            {
                if (!isValid(inputStr[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static string GetUsernameById(int UserId)
        {
            string Name = "Unknown User";

            GameClient Client = GetGame().GetClientManager().GetClientByUserID(UserId);
            if (Client != null && Client.GetHabbo() != null)
            {
                return Client.GetHabbo().Username;
            }

            UserCache User = StarBlueServer.GetGame().GetCacheManager().GenerateUser(UserId);
            if (User != null)
            {
                return User.Username;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `username` FROM `users` WHERE id = @id LIMIT 1");
                dbClient.AddParameter("id", UserId);
                Name = dbClient.GetString();
            }

            if (string.IsNullOrEmpty(Name))
            {
                Name = "Unknown User";
            }

            return Name;
        }

        public static string RainbowT()
        {
            int numColorst = 1000;
            var colorst = new List<string>();
            var randomt = new Random();
            for (int i = 0; i < numColorst; i++)
            {
                colorst.Add(String.Format("#{0:X6}", randomt.Next(0x1000000)));
            }

            int indext = 0;
            string rainbowt = colorst[indext];

            if (indext > numColorst)
            {
                indext = 0;
            }
            else
            {
                indext++;
            }

            return rainbowt;
        }

        public static Habbo GetHabboById(int UserId)
        {
            try
            {
                GameClient Client = GetGame().GetClientManager().GetClientByUserID(UserId);
                if (Client != null)
                {
                    Habbo User = Client.GetHabbo();
                    if (User != null && User.Id > 0)
                    {
                        if (_usersCached.ContainsKey(UserId))
                        {
                            _usersCached.TryRemove(UserId, out User);
                        }

                        return User;
                    }
                }
                else
                {
                    try
                    {
                        if (_usersCached.ContainsKey(UserId))
                        {
                            return _usersCached[UserId];
                        }
                        else
                        {
                            UserData data = UserDataFactory.GetUserData(UserId);
                            if (data != null)
                            {
                                Habbo Generated = data.user;
                                if (Generated != null)
                                {
                                    Generated.InitInformation(data);
                                    _usersCached.TryAdd(UserId, Generated);
                                    return Generated;
                                }
                            }
                        }
                    }
                    catch { return null; }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static Habbo GetHabboByUsername(string UserName)
        {
            try
            {
                using (IQueryAdapter dbClient = GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT `id` FROM `users` WHERE `username` = @user LIMIT 1");
                    dbClient.AddParameter("user", UserName);
                    int id = dbClient.GetInteger();
                    if (id > 0)
                        return GetHabboById(Convert.ToInt32(id));
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        internal static void PerformRestart()
        {
            PerformShutDown(true);
        }

        public static void PerformShutDown(bool restart = false)
        {
            Console.Clear();
            log.Info("StarBlue EMULATOR --> CLOSING");
            Console.Title = "StarBlue EMULATOR: SHUTTING DOWN!";
            GetGame().GetClientManager().SendMessage(new BroadcastMessageAlertComposer("<b><font color=\"#ba3733\" size=\"14\">HOTEL SERÁ REINICIADO!</font></b><br><br>O hotel será reiniciado nesse instante para aplicarmos atualizações, voltaremos em minutos!"));
            GetGame().StopGameLoop();
            Thread.Sleep(2500);

            GetGame().GetPacketManager().UnregisterAll();//Unregister the packets.
            GetGame().GetPacketManager().WaitForAllToComplete();
            GetGame().GetClientManager().CloseAll();//Close all connections
            GetGame().GetRoomManager().Dispose();//Stop the game loop.
            GetGame().GetWebClientManager().CloseAll();
            _webSocketManager.destroy();
            _bootstrap.Shutdown().Wait();
            _bootstrap.ShutdownWorkers();
            GetGame().GetCacheManager().Dispose();

            using (IQueryAdapter dbClient = _datebasemanager.GetQueryReactor())
            {
                dbClient.RunFastQuery("TRUNCATE `catalog_marketplace_data`");
                dbClient.RunFastQuery("UPDATE `users` SET online = '0'");
                dbClient.RunFastQuery("TRUNCATE `user_auth_ticket`");
                dbClient.RunFastQuery("UPDATE `rooms` SET `users_now` = '0' WHERE `users_now` > '0'");
            }

            log.Info("StarBlue session shutted down.");

            if (restart)
            {
                Process.Start(Assembly.GetEntryAssembly().Location);
            }

            Thread.Sleep(1000);
            Environment.Exit(0);
        }

        public static ConfigurationData GetConfig()
        {
            return _configuration;
        }

        public static RankManager GetRankManager()
        {
            return _rankManager;
        }

        public static SettingsManager GetSettingsManager()
        {
            return _settingsManager;
        }

        public static Encoding GetDefaultEncoding()
        {
            return _defaultEncoding;
        }

        public static Game GetGame()
        {
            return _game;
        }

        public static DatabaseManager GetDatabaseManager()
        {
            return _datebasemanager;
        }

        public static FigureDataManager GetFigureManager()
        {
            return _figureManager;
        }

        public static LanguageManager GetLanguageManager()
        {
            return _languageManager;
        }

        public static ICollection<Habbo> GetUsersCached()
        {
            return _usersCached.Values;
        }

        public static bool RemoveFromCache(int Id, out Habbo Data)
        {
            return _usersCached.TryRemove(Id, out Data);
        }
    }
}