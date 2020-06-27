﻿using ConsoleWriter;
using System;

namespace StarBlue.Core
{
    public static class Logging
    {
        public static bool DisabledState
        {
            get { return Writer.DisabledState; }
            set { Writer.DisabledState = value; }
        }

        public static void WriteLine(string Line, ConsoleColor Colour = ConsoleColor.Gray)
        {
            Writer.WriteLine(Line, Colour);
        }

        public static void LogException(string logText)
        {
            Writer.LogException(Environment.NewLine + logText + Environment.NewLine);
        }

        public static void LogMySQLError(string logText)
        {
            Writer.LogMySQLError(Environment.NewLine + logText + Environment.NewLine);
        }

        public static void LogWiredException(string logText)
        {
            Writer.LogWiredException(logText);
        }

        public static void LogCacheException(string logText)
        {
            Writer.LogCacheException(logText);
        }

        public static void LogPathfinderException(string logText)
        {
            Writer.LogPathfinderException(logText);
        }

        public static void LogCriticalException(string logText)
        {
            Writer.LogCriticalException(logText);
        }

        public static void LogThreadException(string Exception, string Threadname)
        {
            Writer.LogThreadException(Exception, Threadname);
        }

        public static void LogPacketException(string Packet, string Exception)
        {
            Writer.LogPacketException(Packet, Exception);
        }

        public static void HandleException(Exception pException, string pLocation)
        {
            Writer.HandleException(pException, pLocation);
        }

        public static void DisablePrimaryWriting(bool ClearConsole)
        {
            Writer.DisablePrimaryWriting(ClearConsole);
        }
    }
}