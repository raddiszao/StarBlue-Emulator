using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Concurrency;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using log4net;
using Newtonsoft.Json;
using StarBlue.Network.Codec;
using StarBlue.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace StarBlue.Network
{
    public class NetworkBootstrap
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NetworkBootstrap));

        private string[] Ports { get; }
        private string Host { get; }

        private IEventLoopGroup BossGroup { get; set; }
        private IEventLoopGroup WorkerGroup { get; set; }
        private IEventLoopGroup ChannelGroup { get; set; }
        private ServerBootstrap ServerBootstrap { get; set; }
        private List<IChannel> ServerChannels { get; set; }

        public static bool IdleTimerEnabled { get; set; }

        private static ConcurrentDictionary<string, int> IpConnectionsCount;

        private static List<string> BlockedIps;

        public static bool SuspiciousMode;

        public NetworkBootstrap(string host, string[] ports)
        {
            this.Host = host;
            this.Ports = ports;
            this.ServerChannels = new List<IChannel>();

            IpConnectionsCount = new ConcurrentDictionary<string, int>();
            BlockedIps = new List<string>();
            IdleTimerEnabled = Convert.ToBoolean(StarBlueServer.GetConfig().data["game.tcp.idleTimer.enabled"]);

            this.BossGroup = new MultithreadEventLoopGroup(int.Parse(StarBlueServer.GetConfig().data["game.tcp.acceptGroupThreads"]));
            this.WorkerGroup = new MultithreadEventLoopGroup(int.Parse(StarBlueServer.GetConfig().data["game.tcp.ioGroupThreads"]));
            this.ChannelGroup = new MultithreadEventLoopGroup(int.Parse(StarBlueServer.GetConfig().data["game.tcp.channelGroupThreads"]));
        }

        public async Task InitAsync()
        {
            ServerBootstrap = new ServerBootstrap()
                .Group(BossGroup, WorkerGroup)
                .Channel<TcpServerSocketChannel>()
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    string IpAddress = ((IPEndPoint)channel.RemoteAddress).Address.MapToIPv4().ToString();
                    if (BlockedIps.Contains(IpAddress))
                    {
                        channel.CloseAsync();
                        return;
                    }

                    if (SuspiciousMode)
                    {
                        string IpCountry = GetAddressCountry(IpAddress);
                        if (!IpCountry.Contains("Brazil") || IpCountry == null)
                        {
                            BlockedIps.Add(IpAddress);
                            log.Warn("Suspicious attack [" + IpAddress + "]. Country (" + IpCountry + ").");
                            channel.CloseAsync();
                            return;
                        }
                    }

                    if (this.SuspiciousConnection(IpAddress))
                    {
                        log.Warn("Client denied for suspicious activity at address " + IpAddress + ". Suspicious mode was activated for 10 minutes.");
                        SuspiciousMode = true;
                        channel.CloseAsync();

                        Threading threading = new Threading();
                        threading.SetMinutes(10);
                        threading.SetAction(() => DisableSuspiciousActivity());
                        threading.Start();
                    }
                    else
                    {
                        IChannelPipeline Pipeline = channel.Pipeline;
                        Pipeline.AddLast("logger", new LoggingHandler());

                        Pipeline.AddLast("xmlDecoder", new GamePolicyDecoder());
                        Pipeline.AddLast("gameDecoder", new GameByteDecoder());

                        Pipeline.AddLast("gameEncoder", new GameByteEncoder());

                        if (IdleTimerEnabled)
                            Pipeline.AddLast("idleHandler", new IdleStateHandler(60, 30, 0));

                        Pipeline.AddLast(ChannelGroup, "clientHandler", new GameMessageHandler());
                        IncrementCounterForIp(IpAddress, (IpConnectionCount(IpAddress) + 1));
                    }
                }))
                .ChildOption(ChannelOption.TcpNodelay, true)
                .ChildOption(ChannelOption.SoKeepalive, true)
                .ChildOption(ChannelOption.SoReuseaddr, true)
                .ChildOption(ChannelOption.SoRcvbuf, 4096)
                .ChildOption(ChannelOption.RcvbufAllocator, new FixedRecvByteBufAllocator(4096))
                .ChildOption(ChannelOption.Allocator, new UnpooledByteBufferAllocator(false));

            try
            {
                foreach (string Port in Ports)
                {
                    IChannel ServerChannel = await ServerBootstrap.BindAsync(IPAddress.Parse(Host), Convert.ToInt32(Port));
                    ServerChannels.Add(ServerChannel);
                }

                log.Info($">> Netty Server listening on ({Host}:{string.Join(",", Ports)}).");
            }
            catch
            {
                log.Warn("Failed to initialize sockets on host " + Host);
            }
        }

        public async Task Shutdown()
        {
            foreach (IChannel ServerChannel in ServerChannels)
                await ServerChannel.CloseAsync();
        }

        public void ShutdownWorkers()
        {
            BossGroup.ShutdownGracefullyAsync();
            WorkerGroup.ShutdownGracefullyAsync();
        }

        public static void IncrementCounterForIp(string Ip, int Amount)
        {
            if (Ip == "127.0.0.1")
                return;

            if (IpConnectionsCount.ContainsKey(Ip))
            {
                IpConnectionsCount[Ip] = Amount;
                return;
            }

            IpConnectionsCount.TryAdd(Ip, Amount);
        }

        public static int IpConnectionCount(string Ip)
        {
            if (Ip == "127.0.0.1")
                return 0;

            if (IpConnectionsCount.ContainsKey(Ip))
            {
                IpConnectionsCount.TryGetValue(Ip, out int Count);
                return Count;
            }
            else
            {
                return 0;
            }
        }

        public void DisableSuspiciousActivity()
        {
            SuspiciousMode = false;
            BlockedIps.Clear();
        }

        public bool SuspiciousConnection(string Ip)
        {
            return IpConnectionCount(Ip) > Convert.ToInt32(StarBlueServer.GetConfig().data["game.tcp.conperip"]);
        }

        public static string GetAddressCountry(string ip)
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
    }
}