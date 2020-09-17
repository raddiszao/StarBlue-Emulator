using log4net;
using Newtonsoft.Json;
using StarBlue.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace StarBlue.Communication.ConnectionManager
{
    public class GameSocketManager
    {
        private static readonly ILog log = LogManager.GetLogger("StarBlue.Communication.ConnectionManager");

        private Socket connectionListener;
        private bool acceptConnections;
        private int maxIpConnectionCount;
        private int acceptedConnections;
        private IDataParser parser;
        private ConcurrentDictionary<string, int> _ipConnectionsCount;
        public List<string> blockedIps;
        public bool suspiciousAttack;
        public event ConnectionEvent connectionEvent;

        public void init(int portID, int connectionsPerIP, IDataParser parser)
        {
            this._ipConnectionsCount = new ConcurrentDictionary<string, int>();

            this.parser = parser;
            this.maxIpConnectionCount = connectionsPerIP;
            this.acceptedConnections = 0;

            this.connectionListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.connectionListener.NoDelay = true;
            try
            {
                this.connectionListener.Bind((EndPoint)new IPEndPoint(IPAddress.Any, portID));
                this.connectionListener.Listen(100);
                this.connectionListener.BeginAccept(new AsyncCallback(this.newConnectionRequest), (object)this.connectionListener);
                this.connectionListener.SendBufferSize = GameSocketManagerStatics.BUFFER_SIZE;
                this.connectionListener.ReceiveBufferSize = GameSocketManagerStatics.BUFFER_SIZE;
                log.Info(">> WebSocket Server listening on " + this.connectionListener.LocalEndPoint.ToString().Split(new char[1] { ':' })[0] + ":" + portID + ".");
            }
            catch (Exception ex)
            {
                this.Destroy();
                Console.WriteLine(ex);
                return;
            }

            this.acceptConnections = true;
        }

        public void Destroy()
        {
            this.acceptConnections = false;
            try
            {
                this.connectionListener.Close();
            }
            catch
            {
            }
        }

        private void newConnectionRequest(IAsyncResult iAr)
        {
            if (!this.acceptConnections)
            {
                Console.WriteLine("Connection denied, server is not currently accepting connections!");
                return;
            }
            try
            {
                Socket dataStream = this.connectionListener.EndAccept(iAr);
                dataStream.NoDelay = true;

                string Ip = dataStream.RemoteEndPoint.ToString().Split(new char[1] { ':' })[0];

                if (suspiciousAttack)
                {
                    string IpCountry = GetUserCountryByIp(Ip);
                    if (!IpCountry.Contains("Brazil") || IpCountry == null)
                    {
                        this.blockedIps.Add(Ip);
                        log.Info("Suspicious attack [" + Ip + "]. Country (" + IpCountry + ").");
                        return;
                    }

                    if (!CanPing(Ip))
                    {
                        this.blockedIps.Add(Ip);
                        log.Info("Suspicious IP [" + Ip + "]. Country (" + IpCountry + ").");
                        return;
                    }
                }

                int ConnectionCount = getAmountOfConnectionFromIp(Ip);
                if (ConnectionCount < maxIpConnectionCount)
                {
                    Interlocked.Increment(ref this.acceptedConnections);

                    ConnectionInformation connection = new ConnectionInformation(dataStream, this.acceptedConnections, this.parser.Clone() as IDataParser, Ip);

                    connection.connectionClose += new ConnectionInformation.ConnectionChange(this.c_connectionChanged);

                    reportUserLogin(Ip);

                    if (this.connectionEvent != null)
                        this.connectionEvent(connection);
                }
                else
                {
                    this.suspiciousAttack = true;
                    Threading threading = new Threading();
                    threading.SetMinutes(10);
                    threading.SetAction(() => disableSuspiciousAttack());
                    threading.Start();
                }
            }
            catch
            {
            }
            finally
            {
                this.connectionListener.BeginAccept(new AsyncCallback(this.newConnectionRequest), (object)this.connectionListener);
            }
        }

        public void disableSuspiciousAttack()
        {
            this.suspiciousAttack = false;
            this.blockedIps.Clear();
        }

        private void c_connectionChanged(ConnectionInformation information)
        {
            this.reportDisconnect(information);
        }

        public void reportDisconnect(ConnectionInformation gameConnection)
        {
            gameConnection.connectionClose -= new ConnectionInformation.ConnectionChange(this.c_connectionChanged);
            reportUserLogout(gameConnection.getIp());
        }

        private void reportUserLogin(string ip)
        {
            alterIpConnectionCount(ip, (getAmountOfConnectionFromIp(ip) + 1));
        }

        private void reportUserLogout(string ip)
        {
            alterIpConnectionCount(ip, (getAmountOfConnectionFromIp(ip) - 1));
        }

        private void alterIpConnectionCount(string ip, int amount)
        {
            if (ip == "127.0.0.1")
                return;

            if (_ipConnectionsCount.ContainsKey(ip))
            {
                _ipConnectionsCount.TryRemove(ip, out int am);
            }
            _ipConnectionsCount.TryAdd(ip, amount);
        }

        private int getAmountOfConnectionFromIp(string ip)
        {
            try
            {
                if (ip == "127.0.0.1")
                    return 0;

                if (_ipConnectionsCount.ContainsKey(ip))
                {
                    int Count = 0;
                    _ipConnectionsCount.TryGetValue(ip, out Count);
                    return Count;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        public delegate void ConnectionEvent(ConnectionInformation connection);
        public static string GetUserCountryByIp(string ip)
        {
            IpInfo ipInfo = new IpInfo();
            try
            {
                string info = new WebClient().DownloadString("https://ipinfo.io/" + ip);
                ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
                RegionInfo myRI1 = new RegionInfo(ipInfo.Country);
                ipInfo.Country = myRI1.EnglishName;
            }
            catch (Exception)
            {
                ipInfo.Country = null;
            }

            return ipInfo.Country;
        }

        private static bool CanPing(string address)
        {
            Ping ping = new Ping();

            try
            {
                PingReply reply = ping.Send(address, 2000);
                if (reply == null) return false;

                return (reply.Status == IPStatus.Success);
            }
            catch
            {
                return false;
            }
        }

    }
}

public class IpInfo
{

    [JsonProperty("ip")]
    public string Ip { get; set; }

    [JsonProperty("hostname")]
    public string Hostname { get; set; }

    [JsonProperty("city")]
    public string City { get; set; }

    [JsonProperty("region")]
    public string Region { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; }

    [JsonProperty("loc")]
    public string Loc { get; set; }

    [JsonProperty("org")]
    public string Org { get; set; }

    [JsonProperty("postal")]
    public string Postal { get; set; }
}
