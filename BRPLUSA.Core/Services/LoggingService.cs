﻿using NLog;
using System;
using System.Text;
using NLog.Config;
using NLog.Targets;

namespace BRPLUSA.Core.Services
{
    public static class LoggingService
    {
        private static Logger Logger { get; set; }
        private static string LogDirectory { get; set; }

        static LoggingService()
        {
            Initialize();
        }

        private static void Initialize()
        {
            SetLogDirectory();
            var config = new LoggingConfiguration();

            var error = CreateErrorLoggingTarget();
            var full = CreateFullLoggingTarget();

            config.AddRule(LogLevel.Trace, LogLevel.Fatal, full);
            config.AddRule(LogLevel.Warn, LogLevel.Fatal, error);

            LogManager.Configuration = config;
            Logger = new LogFactory(config).GetCurrentClassLogger();
        }

        private static void SetLogDirectory()
        {
            var localappdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dir = $"{localappdata}/TEMP/BRPLUSA/LOGS/";

            LogDirectory = dir;
        }

        private static string GetLogName(bool isFullLog)
        {
            var time = DateTime.Now.ToString("s").Replace(":", string.Empty);
            var name = isFullLog 
                ? $"{time}_full.log" 
                : $"{time}_errors.log";

            return name;
        }

        private static FileTarget CreateErrorLoggingTarget()
        {
            var logName = GetLogName(false);

            var logError = new FileTarget("log_error")
            {
                FileName =  LogDirectory + logName,
                CleanupFileName = true,
                Layout = "${longdate}\t${level}\t${message}\t\t${exception}",
                //Header = "*********Enhancement_Log_Errors*************",
                Encoding = Encoding.UTF8,
                //ArchiveAboveSize = 100,
                //MaxArchiveFiles = 100,
                //ArchiveFileName = "archive_${shortdate}",
                //ArchiveOldFileOnStartup = true,
                //DeleteOldFileOnStartup = false,
                //EnableFileDelete = true,
                //CreateDirs = true,
                //ConcurrentWrites = true,
                //NetworkWrites = true,
                //KeepFileOpen = true
            };

            return logError;
        }

        private static FileTarget CreateFullLoggingTarget()
        {
            var logName = GetLogName(true);

            var logFull = new FileTarget("log_full")
            {
                FileName = LogDirectory + logName,
                CleanupFileName = true,
                Layout = "${longdate}\t${level}\t${message}\t\t${exception}",
                //Header = "*********Enhancement_Log_Errors*************",
                Encoding = Encoding.UTF8,
                //ArchiveAboveSize = 100,
                //MaxArchiveFiles = 100,
                //ArchiveFileName = "archive_${shortdate}",
                //ArchiveOldFileOnStartup = true,
                //DeleteOldFileOnStartup = false,
                //EnableFileDelete = true,
                //CreateDirs = true,
                //ConcurrentWrites = true,
                //NetworkWrites = true,
                //KeepFileOpen = true
            };

            return logFull;
        }

        public static void LogError(string msg, Exception e)
        {
            Logger.Error(e, msg);

            var inner = e?.InnerException;

            if(inner != null)
                LogError(inner.Message, inner);
        }

        public static void LogInfo(string msg)
        {
            Logger.Info(msg);
        }

        public static void LogWarning(string msg)
        {
            Logger.Warn(msg);
        }
    }
}
