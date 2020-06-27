using log4net;
using StarBlue.Communication.Packets.Incoming.LandingView;
using StarBlue.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace StarBlue.Core
{
    public class ServerStatusUpdater : IDisposable
    {
        private static ILog log = LogManager.GetLogger("StarBlue.Core.ServerStatusUpdatear");

        private const int UPDATE_IN_SECS = 15;

        private Timer _timer;
        private Timer _hofTimer;

        private MemoryStream memoryStream;

        public ServerStatusUpdater()
        {
        }

        public void Init()
        {
            memoryStream = new MemoryStream();
            _timer = new Timer(new TimerCallback(OnTick), null, TimeSpan.FromSeconds(UPDATE_IN_SECS), TimeSpan.FromSeconds(UPDATE_IN_SECS));
            _hofTimer = new Timer(new TimerCallback(HofUpdate), null, TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(25));

            Console.Title = "STARBLUE SERVER - [0] USERS - [0] ROOMS - [0] UPTIME";

            log.Info(">> Server Status -> READY!");
        }

        public void HofUpdate(object Obj)
        {
            GetHallOfFame.Load();
        }

        public void OnTick(object Obj)
        {
            UpdateOnlineUsers();
        }

        private void UpdateOnlineUsers()
        {
            memoryStream.Flush();
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (UIntPtr)0xFFFFFFFF, (UIntPtr)0xFFFFFFFF);
            MinimizeFootprint();

            TimeSpan Uptime = DateTime.Now - StarBlueServer.ServerStarted;

            List<GameClient> RunningClients = StarBlueServer.GetGame().GetClientManager().GetClients.ToList().
               Where(Client => Client != null).
               Where(Client => Client.LoggingOut != true).
               Where(Client => Client.GetHabbo() != null).ToList();
            int UsersOnline = Convert.ToInt32(StarBlueServer.GetGame().GetClientManager().Count);
            int RoomCount = StarBlueServer.GetGame().GetRoomManager().Count;
            Console.Title = "STARBLUE SERVER - [" + RunningClients.Count + "] WEBSOCKETS [" + UsersOnline + "] USERS - [" + RoomCount + "] ROOMS - [" + Uptime.Days + "] DAYS [" + Uptime.Hours + "] HOURS";

        }

        public void Dispose()
        {
            _timer.Dispose();
            _hofTimer.Dispose();
            GC.SuppressFinalize(this);
        }

        [DllImport("psapi.dll")]

        public static extern int EmptyWorkingSet(IntPtr hwProc);

        public static void MinimizeFootprint() => EmptyWorkingSet(Process.GetCurrentProcess().Handle);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]

        public static extern bool SetProcessWorkingSetSize(IntPtr process,
            UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);

    }
}
