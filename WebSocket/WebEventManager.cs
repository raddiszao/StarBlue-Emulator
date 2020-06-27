using Fleck;
using log4net;
using NetFwTypeLib;
using Newtonsoft.Json;
using StarBlue.Core;
using StarBlue.HabboHotel.GameClients;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace StarBlue.WebSocket
{
    public class WebSocketUser
    {
        public string Username { get; set; }

        public int ID { get; set; }

        public bool Closing { get; set; }

        public IWebSocketConnection Connection { get; set; }

        public WebSocketUser(int ID, string Username, IWebSocketConnection Connection)
        {
            this.ID = ID;
            this.Username = Username;
            Closing = false;
            this.Connection = Connection;
        }

        public void Dispose()
        {
            ID = 0;
            Username = null;
            Closing = true;
        }
    }

    public sealed class WebEventManager
    {
        private static readonly ILog log = LogManager.GetLogger("StarBlue.Web.WebEventManager");

        public WebSocketServer _webSocketServer;

        public ConcurrentDictionary<IWebSocketConnection, WebSocketUser> _webSockets;

        private ConcurrentDictionary<string, IWebEvent> _webEvents;

        //Créditos firewall: Byxhp
        private ConcurrentDictionary<string, int> _ipConnectionsCount;
        public List<string> NoReceiveData = new List<string>();
        public static List<string> BlockedFirewallIps = new List<string>();

        public WebEventManager()
        {
            _ipConnectionsCount = new ConcurrentDictionary<string, int>();
            NoReceiveData = new List<string>();
            BlockedFirewallIps = new List<string>();

            string IP = StarBlueServer.GetConfig().data["ws.tcp.bindip"];
            int Port = Convert.ToInt32(StarBlueServer.GetConfig().data["ws.tcp.port"]);

            _webSocketServer = new WebSocketServer(IP + ":" + Port);
            if (IP.StartsWith("wss"))
            {
                _webSocketServer.EnabledSslProtocols = SslProtocols.Tls12;
                _webSocketServer.Certificate = new X509Certificate2("heibbocertificate.pfx", "22751");

            }

            _webSockets = new ConcurrentDictionary<IWebSocketConnection, WebSocketUser>();
            _webEvents = new ConcurrentDictionary<string, IWebEvent>();
            RegisterIncoming();
        }

        public void Init()
        {
            _webSocketServer.ListenerSocket.NoDelay = true;
            _webSocketServer.Start(ConnectingSocket =>
            {
                if (!CheckIP(ConnectingSocket.ConnectionInfo.ClientIpAddress))
                {
                    var CountConnections = GetConnectionsByIP(ConnectingSocket.ConnectionInfo.ClientIpAddress) + 1;
                    if (CountConnections > 10)
                    {
                        Logging.WriteLine("[FIREWALL] [WEBSOCKET] O endereço de IP [" + ConnectingSocket.ConnectionInfo.ClientIpAddress + "] atingiu o limite e foi bloqueado.", ConsoleColor.Red);
                        BlockIpFirewall(ConnectingSocket.ConnectionInfo.ClientIpAddress, true);
                        BlockedFirewallIps.Add(ConnectingSocket.ConnectionInfo.ClientIpAddress);
                        return;
                    }
                    else
                    {
                        string ip = ConnectingSocket.ConnectionInfo.ClientIpAddress;
                        if (_ipConnectionsCount.ContainsKey(ip))
                        {
                            _ipConnectionsCount.TryRemove(ip, out int am);
                        }

                        _ipConnectionsCount.TryAdd(ip, GetConnectionsByIP(ConnectingSocket.ConnectionInfo.ClientIpAddress) + 1);
                    }

                    ConnectingSocket.OnOpen = () => OnSocketAdd(ConnectingSocket);
                    ConnectingSocket.OnClose = () => OnSocketRemove(ConnectingSocket);
                    ConnectingSocket.OnMessage = SocketData => OnSocketMessage(ConnectingSocket, SocketData);
                    ConnectingSocket.OnError = SocketError => OnSocketError(SocketError);
                }
            });
        }


        public static void BlockIpFirewall(string ip, bool ws)
        {
            if (!FirewallExists(ip))
            {
                INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
                firewallRule.Description = "[BLOCK][WEBSOCKET] Heibbo: [" + ip + "] [" + DateTime.Now.ToString("d/M/y - H:i:s") + "]";
                //firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
                firewallRule.EdgeTraversal = true;
                firewallRule.Enabled = true;
                firewallRule.Name = ip;
                firewallRule.Profiles = 2147483647;
                firewallRule.Protocol = 6;
                firewallRule.RemoteAddresses = ip;
                firewallPolicy.Rules.Add(firewallRule);
                Logging.WriteLine("Firewall IP " + ip + " Blocked to many connections.", ConsoleColor.Red);
            }
        }

        public static bool FirewallExists(string Ip)
        {
            try
            {
                Type tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
                INetFwPolicy2 fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy2);
                var currentProfiles = fwPolicy2.CurrentProfileTypes;

                List<INetFwRule> RuleList = new List<INetFwRule>();

                foreach (INetFwRule rule in fwPolicy2.Rules)
                {
                    if (rule.Name.IndexOf(Ip) != -1)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Logging.LogCriticalException(e.ToString());
                return false;
            }
        }

        public static bool CheckIP(string endereco)
        {
            try
            {
                foreach (var ip in BlockedFirewallIps)
                {
                    if (ip.ToString().IndexOf(endereco) != -1)
                    {
                        BlockedFirewallIps.Add(endereco);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Logging.LogCriticalException(e.ToString());
                return false;
            }
        }

        private int GetConnectionsByIP(string ip)
        {
            try
            {
                if (_ipConnectionsCount.ContainsKey(ip))
                {
                    return _ipConnectionsCount[ip];
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        public void RegisterIncoming()
        {
            _webEvents.TryAdd("1", new EnterRoom());
            _webEvents.TryAdd("2", new BuilderTool());
        }

        private void OnSocketMessage(IWebSocketConnection InteractingSocket, string SentData)
        {
            try
            {
                var ReceivedData = JsonConvert.DeserializeObject<WebEvent>(SentData);

                if (string.IsNullOrEmpty(ReceivedData.EventName))
                {
                    return;
                }

                GameClient InteractingClient = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(ReceivedData.UserId);

                if (InteractingClient == null)
                {
                    return;
                }

                if (InteractingClient.LoggingOut)
                {
                    return;
                }

                if (!_webSockets.ContainsKey(InteractingSocket))
                {
                    return;
                }

                if (!ReceivedData.SSO.Equals(InteractingClient.ssoTicket))
                {
                    return;
                }


                if (_webEvents.TryGetValue(ReceivedData.EventName, out IWebEvent webEvent))
                {
                    webEvent.Execute(InteractingClient, ReceivedData.ExtraData, InteractingSocket);
                    return;
                }

                log.Debug("Unrecognized Web Event: '" + ReceivedData.EventName + "'");

            }
            catch (Exception ex)
            {
                OnSocketError(ex);
            }
        }

        public void OnSocketAdd(IWebSocketConnection ConnectingSocket)
        {
            if (ConnectingSocket == null)
            {
                return;
            }

            if (!ConnectingSocket.IsAvailable)
            {
                return;
            }

            if (ConnectingSocket.ConnectionInfo == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(ConnectingSocket.ConnectionInfo.Path))
            {
                return;
            }

            if (!ConnectingSocket.ConnectionInfo.Path.Trim().Contains("/"))
            {
                return;
            }

            if ((string.IsNullOrEmpty(ConnectingSocket.ConnectionInfo.Path.Trim().Split('/')[1])))
            {
                return;
            }

            try
            {
                int ConnectingUsersID = GetSocketsUserID(ConnectingSocket);
                List<GameClient> PotentialConnectedClients = GetSocketsClient(ConnectingSocket);

                if (PotentialConnectedClients.Count > 0)
                {
                    DeactivateSocket(ConnectingSocket);
                    return;
                }

                WebSocketUser User = null;
                if (_webSockets.ContainsKey(ConnectingSocket))
                {
                    _webSockets.TryRemove(ConnectingSocket, out User);
                }

                _webSockets.TryAdd(ConnectingSocket, new WebSocketUser(ConnectingUsersID, "", ConnectingSocket));

            }
            catch (Exception ex)
            {
                OnSocketError(ex);
            }
        }

        public void OnSocketRemove(IWebSocketConnection User)
        {
            if (!_webSockets.ContainsKey(User))
            {
                return;
            }

            try
            {
                CloseSimilarSockets(GetSocketsUserID(User));
            }
            catch (Exception ex)
            {
                OnSocketError(ex);
            }
        }

        public void CloseSimilarSockets(int Id)
        {
            List<IWebSocketConnection> SocketsToClose = GetSimilarSockets(Id);

            foreach (IWebSocketConnection Socket in SocketsToClose)
            {
                DeactivateSocket(Socket);
            }
        }

        public List<IWebSocketConnection> GetSimilarSockets(int Id)
        {
            List<IWebSocketConnection> SimilarSockets = new List<IWebSocketConnection>();

            foreach (KeyValuePair<IWebSocketConnection, WebSocketUser> AvailableSockets in _webSockets)
            {
                if (AvailableSockets.Value == null)
                {
                    continue;
                }

                if (AvailableSockets.Key == null)
                {
                    continue;
                }

                if (GetSocketsUserID(AvailableSockets.Key) == Id)
                {
                    SimilarSockets.Add(AvailableSockets.Key);
                }
            }

            return SimilarSockets;
        }

        public void OnSocketError(Exception Exception)
        {
            if (Exception.Message.Contains("An existing connection was forcibly closed by the remote host.") || Exception.Message.Contains("A connection attempt failed because the connected party did not properly respond after a period of time"))
            {
                return;
            }

            Logging.HandleException(Exception, "");
        }

        public bool ExecuteWebEvent(GameClient Client, string EventName, string ReceivedData)
        {
            try
            {

                if (ReceivedData.StartsWith("{"))
                {
                    // JSON
                }
                else
                {
                    if (!ReceivedData.Contains("bypass"))
                    {
                        if (Client == null)
                        {
                            return false;
                        }

                        if (Client.LoggingOut)
                        {
                            return false;
                        }

                        if (!SocketReady(Client))
                        {
                            return false;
                        }
                    }
                }

                if (string.IsNullOrEmpty(EventName))
                {
                    return false;
                }

                IWebSocketConnection InteractingSocket = Client.GetHabbo().WebSocketConnection;

                if (!SocketReady(InteractingSocket))
                {
                    return false;
                }

                if (_webEvents.TryGetValue(EventName, out IWebEvent webEvent))
                {
                    if (!_webSockets[InteractingSocket].Closing || !InteractingSocket.IsAvailable)
                    {
                        webEvent.Execute(Client, ReceivedData, InteractingSocket);
                    }

                    return true;
                }

            }
            catch (Exception ex)
            {
                OnSocketError(ex);
            }
            return false;
        }

        public void BroadCastWebEvent(string EventName, string Data)
        {
            foreach (GameClient User in StarBlueServer.GetGame().GetClientManager().GetClients)
            {
                if (User == null)
                {
                    continue;
                }

                if (User.LoggingOut)
                {
                    continue;
                }

                ExecuteWebEvent(User, EventName, Data);
            }
        }

        public void SendDataDirect(GameClient User, string Data)
        {
            if (!SocketReady(User, true))
            {
                return;
            }

            if (User.GetHabbo().WebSocketConnection == null)
            {
                return;
            }

            if (!SocketReady(User.GetHabbo().WebSocketConnection))
            {
                return;
            }

            try
            {
                User.GetHabbo().WebSocketConnection.Send(Data);
            }
            catch (Exception ex)
            {
                OnSocketError(ex);
            }
        }

        public void BroadCastWebData(string Data)
        {
            foreach (GameClient User in StarBlueServer.GetGame().GetClientManager().GetClients)
            {
                if (!SocketReady(User, true))
                {
                    continue;
                }

                if (!SocketReady(User.GetHabbo().WebSocketConnection))
                {
                    continue;
                }

                try
                {
                    User.GetHabbo().WebSocketConnection.Send(Data);
                }
                catch (Exception ex)
                {
                    OnSocketError(ex);
                }
            }
        }

        public bool SocketReady(IWebSocketConnection Socket)
        {
            if (Socket == null)
            {
                return false;
            }

            if (!_webSockets.ContainsKey(Socket))
            {
                return false;
            }

            if (_webSockets[Socket].Closing)
            {
                return false;
            }

            if (!Socket.IsAvailable)
            {
                return false;
            }

            return true;
        }

        public bool SocketReady(GameClient User, bool Logout = false)
        {
            if (User == null)
            {
                return false;
            }

            if (User.GetHabbo() == null)
            {
                return false;
            }

            if (!SocketReady(User.GetHabbo().WebSocketConnection))
            {
                return false;
            }

            if (Logout)
            {
                if (User.LoggingOut)
                {
                    return false;
                }
            }

            return true;
        }

        public IWebSocketConnection GetUsersConnection(GameClient User)
        {

            IWebSocketConnection UsersSocket = null;
            if (User == null)
            {
                return null;
            }

            if (User.GetHabbo() == null)
            {
                return null;
            }

            if (User.LoggingOut)
            {
                return null;
            }

            UsersSocket = _webSockets
                            .Where(MySockets => GetSocketsUserID(MySockets.Key) == User.GetHabbo().Id)
                            .Where(MySockets => SocketReady(MySockets.Key)).FirstOrDefault().Key;

            if (!SocketReady(UsersSocket))
            {
                return null;
            }

            return UsersSocket;
        }

        public int GetSocketsUserID(IWebSocketConnection Socket)
        {
            if (Socket == null)
            {
                return 0;
            }

            if (Socket.ConnectionInfo == null)
            {
                return 0;
            }

            if (Socket.ConnectionInfo.Path == null)
            {
                return 0;
            }

            if (String.IsNullOrEmpty(Socket.ConnectionInfo.Path))
            {
                return 0;
            }

            return Convert.ToInt32(Socket.ConnectionInfo.Path.Trim().Split('/')[1]);
        }

        public List<GameClient> GetSocketsClient(IWebSocketConnection Socket)
        {
            List<GameClient> RunningClients = StarBlueServer.GetGame().GetClientManager().GetClients.ToList().
               Where(Client => Client != null).
               Where(Client => Client.LoggingOut != true).
               Where(Client => Client.GetHabbo() != null).
               Where(Client => SocketReady(Socket)).ToList();

            return RunningClients;
        }

        public ConcurrentDictionary<IWebSocketConnection, WebSocketUser> GetConnectedUsers()
        {
            return _webSockets;
        }

        public void DeactivateSocket(IWebSocketConnection Socket)
        {

            if (_webSockets.ContainsKey(Socket))
            {
                _webSockets[Socket].Closing = true;
                _webSockets[Socket].Dispose();
                _webSockets.TryRemove(Socket, out WebSocketUser user);
            }

            Socket.Close();
        }

        public void Dispose()
        {
            _webSockets.Clear();
            _webSocketServer.Dispose();
        }

        public void CloseSocketByGameClient(int SocketUserID)
        {
            if (SocketUserID == 0)
            {
                return;
            }

            try
            {
                CloseSimilarSockets(SocketUserID);
            }
            catch (Exception)
            {

            }
        }
    }
}
