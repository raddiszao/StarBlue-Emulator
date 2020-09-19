using DotNetty.Transport.Channels;
using log4net;
using StarBlue.Communication.Packets.Outgoing;
using StarBlue.Communication.Packets.Outgoing.Notifications;
using StarBlue.Core;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Groups;
using StarBlue.HabboHotel.Users.Messenger;
using StarBlue.Network;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace StarBlue.HabboHotel.GameClients
{
    public class GameClientManager
    {
        private static readonly ILog log = LogManager.GetLogger("StarBlue.HabboHotel.GameClients.GameClientManager");

        private ConcurrentDictionary<IChannelId, GameClient> _clients;

        private Dictionary<int, GameClient> guides;
        private Dictionary<int, GameClient> alphas;

        private ConcurrentDictionary<int, GameClient> _userIDRegister;
        private ConcurrentDictionary<string, GameClient> _usernameRegister;

        private readonly Queue timedOutConnections;

        private readonly Stopwatch clientPingStopwatch;

        public GameClientManager()
        {
            _clients = new ConcurrentDictionary<IChannelId, GameClient>();
            _userIDRegister = new ConcurrentDictionary<int, GameClient>();
            _usernameRegister = new ConcurrentDictionary<string, GameClient>();

            guides = new Dictionary<int, GameClient>();
            alphas = new Dictionary<int, GameClient>();
            timedOutConnections = new Queue();

            clientPingStopwatch = new Stopwatch();
            clientPingStopwatch.Start();
        }

        public GameClient GetClientByUserID(int userID)
        {
            if (_userIDRegister.ContainsKey(userID))
            {
                return _userIDRegister[userID];
            }

            return null;
        }


        public GameClient GetClientByUsername(string username)
        {
            if (_usernameRegister.ContainsKey(username.ToLower()))
            {
                return _usernameRegister[username.ToLower()];
            }

            return null;
        }

        public bool TryGetClient(IChannelId ClientId, out GameClient Client)
        {
            return this._clients.TryGetValue(ClientId, out Client);
        }

        public bool UpdateClientUsername(GameClient Client, string OldUsername, string NewUsername)
        {
            if (Client == null || !_usernameRegister.ContainsKey(OldUsername.ToLower()))
            {
                return false;
            }

            _usernameRegister.TryRemove(OldUsername.ToLower(), out Client);
            _usernameRegister.TryAdd(NewUsername.ToLower(), Client);
            return true;
        }

        public string GetNameById(int Id)
        {
            GameClient client = GetClientByUserID(Id);

            if (client != null)
            {
                return client.GetHabbo().Username;
            }

            string username;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT username FROM users WHERE id = @id LIMIT 1");
                dbClient.AddParameter("id", Id);
                username = dbClient.GetString();
            }

            return username;
        }

        public int GetUserIdByUsername(string Username)
        {
            GameClient client = GetClientByUsername(Username);

            if (client != null)
            {
                return client.GetHabbo().Id;
            }

            int userid;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id FROM users WHERE username = @username LIMIT 1");
                dbClient.AddParameter("username", Username);
                userid = dbClient.GetInteger();
            }

            return userid;
        }

        public IEnumerable<GameClient> GetClientsById(Dictionary<int, MessengerBuddy>.KeyCollection users)
        {
            foreach (int id in users)
            {
                GameClient client = GetClientByUserID(id);
                if (client != null)
                {
                    yield return client;
                }
            }
        }

        public void StaffAlert1(MessageComposer Message, int Exclude = 0)
        {
            foreach (GameClient client in GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                {
                    continue;
                }

                if (client.GetHabbo().Rank < 4 || client.GetHabbo().Id == Exclude || client.GetHabbo()._alerttype == "1")
                {
                    continue;
                }

                client.SendMessage(Message);
            }
        }

        public void StaffAlert2(MessageComposer Message, int Exclude = 0)
        {
            foreach (GameClient client in GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                {
                    continue;
                }

                if (client.GetHabbo().Rank < 4 || client.GetHabbo().Id == Exclude || client.GetHabbo()._alerttype == "2")
                {
                    continue;
                }

                client.SendMessage(Message);
            }
        }

        public void StaffAlert(MessageComposer Message, int Exclude = 0)
        {
            foreach (GameClient client in GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                {
                    continue;
                }

                if (client.GetHabbo().Rank < 4 || client.GetHabbo().Id == Exclude)
                {
                    continue;
                }

                client.SendMessage(Message);
            }
        }

        public void ManagerAlert(MessageComposer Message, int Exclude = 0)
        {
            foreach (GameClient client in GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                {
                    continue;
                }

                if (client.GetHabbo().Rank < 16 || client.GetHabbo().Id == Exclude)
                {
                    continue;
                }

                client.SendMessage(Message);
            }
        }

        public void GroupChatAlert(MessageComposer Message, Group Group, int Exclude = 0)
        {
            foreach (GameClient client in GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                {
                    continue;
                }

                if (!Group.IsMember(client.GetHabbo().Id) || client.GetHabbo().Id == Exclude)
                {
                    continue;
                }

                client.SendMessage(Message);
            }
        }

        public void GuideAlert(MessageComposer Message, int Exclude = 0)
        {
            foreach (GameClient client in GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                {
                    continue;
                }

                if (client.GetHabbo()._guidelevel < 1 || client.GetHabbo().Id == Exclude)
                {
                    continue;
                }

                client.SendMessage(Message);
            }
        }

        public void MsgAlert2(MessageComposer Message, int Exclude = 0)
        {
            foreach (GameClient client in GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                {
                    continue;
                }

                if (client.GetHabbo().Rank < 1 || client.GetHabbo().Id == Exclude)
                {
                    continue;
                }

                client.SendMessage(Message);
            }
        }

        public void ModAlert(string Message)
        {
            foreach (GameClient client in GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                {
                    continue;
                }

                if (client.GetHabbo().GetPermissions().HasRight("mod_tool") && !client.GetHabbo().GetPermissions().HasRight("staff_ignore_mod_alert"))
                {
                    try { client.SendWhisper(Message, 5); }
                    catch { }
                }
            }
        }

        public void DoAdvertisingReport(GameClient Reporter, GameClient Target)
        {
            if (Reporter == null || Target == null || Reporter.GetHabbo() == null || Target.GetHabbo() == null)
            {
                return;
            }

            StringBuilder Builder = new StringBuilder();
            Builder.Append("Novo Report!\r\r");
            Builder.Append("Usuário: " + Reporter.GetHabbo().Username + "\r");
            Builder.Append("Usuário Reportado: " + Target.GetHabbo().Username + "\r\r");
            Builder.Append(Target.GetHabbo().Username + "Últimas 10 mensagens:\r\r");

            DataTable GetLogs = null;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `message` FROM `chatlogs` WHERE `user_id` = '" + Target.GetHabbo().Id + "' ORDER BY `id` DESC LIMIT 10");
                GetLogs = dbClient.GetTable();

                if (GetLogs != null)
                {
                    int Number = 11;
                    foreach (DataRow Log in GetLogs.Rows)
                    {
                        Number -= 1;
                        Builder.Append(Number + ": " + Convert.ToString(Log["message"]) + "\r");
                    }
                }
            }

            foreach (GameClient Client in GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                {
                    continue;
                }

                if (Client.GetHabbo().GetPermissions().HasRight("mod_tool") && !Client.GetHabbo().GetPermissions().HasRight("staff_ignore_advertisement_reports"))
                {
                    Client.SendMessage(new MOTDNotificationComposer(Builder.ToString()));
                }
            }
        }

        public void SendMessage(MessageComposer Packet, string fuse = "")
        {
            foreach (GameClient Client in _clients.Values.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(fuse))
                {
                    if (!Client.GetHabbo().GetPermissions().HasRight(fuse))
                    {
                        continue;
                    }
                }

                Client.SendMessage(Packet);
            }
        }

        public void CreateAndStartClient(IChannelHandlerContext connection)
        {
            GameClient Client = new GameClient(connection);
            if (this._clients.TryAdd(connection.Channel.Id, Client))
            {
                //Hmmmmm?
            }
            else
            {
                Client.Dispose();
            }
        }

        public void DisposeConnection(IChannelId clientID)
        {
            if (!TryGetClient(clientID, out GameClient Client))
                return;

            if (Client != null)
                Client.Dispose();

            NetworkBootstrap.IncrementCounterForIp(Client.GetIpAddress(), (NetworkBootstrap.IpConnectionCount(Client.GetIpAddress()) - 1));
            this._clients.TryRemove(clientID, out Client);
        }

        public void LogClonesOut(int UserID)
        {
            GameClient client = GetClientByUserID(UserID);
            if (client != null)
            {
                client.Disconnect();
            }
        }

        public void RegisterClient(GameClient client, int userID, string username)
        {
            if (_usernameRegister.ContainsKey(username.ToLower()))
            {
                _usernameRegister[username.ToLower()] = client;
            }
            else
            {
                _usernameRegister.TryAdd(username.ToLower(), client);
            }

            if (_userIDRegister.ContainsKey(userID))
            {
                _userIDRegister[userID] = client;
            }
            else
            {
                _userIDRegister.TryAdd(userID, client);
            }
        }

        public void UnregisterClient(int userid, string username)
        {
            _userIDRegister.TryRemove(userid, out GameClient Client);
            _usernameRegister.TryRemove(username.ToLower(), out Client);
        }

        public void CloseAll()
        {
            foreach (GameClient client in this.GetClients.ToList())
            {
                if (client == null)
                    continue;

                if (client.GetHabbo() != null)
                {
                    try
                    {
                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunFastQuery(client.GetHabbo().GetQueryString);
                        }

                        Console.Clear();
                        log.Info("<<- SERVER SHUTDOWN ->> INVENTORY IS SAVING");
                    }
                    catch
                    {
                    }
                }
            }

            log.Info("Done saving users inventory!");
            log.Info("Closing server connections...");
            try
            {
                foreach (GameClient client in this.GetClients.ToList())
                {
                    if (client == null)
                        continue;

                    try
                    {
                        client.Dispose();
                    }
                    catch
                    {
                    }
                }

                Console.Clear();
                log.Info("<<- SERVER SHUTDOWN ->> CLOSING CONNECTIONS");
            }
            catch (Exception e)
            {
                Logging.HandleException(e, "");
            }

            if (this._clients.Count > 0)
                this._clients.Clear();

            log.Info("Connections closed!");
        }

        public int Count => _clients.Count;

        public ICollection<GameClient> GetClients => _clients.Values;
    }
}