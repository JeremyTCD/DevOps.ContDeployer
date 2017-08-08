﻿using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Targets;
using NLog.Extensions.Logging;
using LogLevel = NLog.LogLevel;
using JeremyTCD.DotNetCore.Utils;

namespace JeremyTCD.PipelinesCE.Core
{
    /// <summary>
    /// Programatically configures NLog provider for <see cref="ILoggerFactory"/>. This class facilitates consistent logging configuration
    /// across AssemblyLoadContexts.
    /// </summary>
    public class LoggingConfig : ILoggingConfig
    {
        private IPathService _pathService { get; }
        private IDirectoryService _directoryService { get; }

        public LoggingConfig(IPathService pathService, IDirectoryService directoryService)
        {
            _pathService = pathService;
            _directoryService = directoryService;
        }

        public void Configure(ILoggerFactory loggerFactory, PipelinesCEOptions pipelinesCEOptions)
        {
            LoggingConfiguration config = CreateLoggingConfiguration(pipelinesCEOptions);

            loggerFactory.
                AddNLog().
                ConfigureNLog(config);
        }

        // TODO should be internal or private
        public LoggingConfiguration CreateLoggingConfiguration(PipelinesCEOptions pipelinesCEOptions)
        {
            LogLevel logLevel = pipelinesCEOptions.Debug || pipelinesCEOptions.Verbose ? LogLevel.Debug : LogLevel.Info;
            string layout = "[${longdate}][${logger}][${level: uppercase = true}] ${message}";
            LoggingConfiguration config = new LoggingConfiguration();

            // Console
            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget
            {
                Name = nameof(ConsoleTarget),
                Layout = layout
            };
            config.AddTarget(consoleTarget);
            LoggingRule consoleRule = new LoggingRule("*", logLevel, consoleTarget);
            config.LoggingRules.Add(consoleRule);

            // File
            if (pipelinesCEOptions.FileLogging)
            {
                string logFile = pipelinesCEOptions.LogFile;
                string archiveFile = pipelinesCEOptions.ArchiveFile;
                bool logFileRooted = _pathService.IsPathRooted(logFile);
                bool archiveFileRooted = _pathService.IsPathRooted(archiveFile);

                if (!logFileRooted || !archiveFileRooted)
                {
                    string projectDir = _directoryService.GetParent(pipelinesCEOptions.ProjectFile).FullName;
                    logFile = logFileRooted ? logFile : _pathService.Combine(projectDir, logFile);
                    archiveFile = archiveFileRooted ? archiveFile : _pathService.Combine(projectDir, archiveFile);
                }

                FileTarget fileTarget = new FileTarget
                {
                    Name = nameof(FileTarget),
                    // KeepFileOpen = true, // Can improve performance but holds on to file lock. Profile.
                    FileName = logFile,
                    Layout = layout,
                    ArchiveOldFileOnStartup = true,
                    ArchiveFileName = archiveFile,
                    ArchiveNumbering = ArchiveNumberingMode.Sequence
                };
                config.AddTarget(fileTarget);
                LoggingRule fileRule = new LoggingRule("*", logLevel, fileTarget);
                config.LoggingRules.Add(fileRule);
            }

            // Debugger
            if (pipelinesCEOptions.Debug || pipelinesCEOptions.Verbose)
            {
                DebuggerTarget debuggerTarget = new DebuggerTarget
                {
                    Name = nameof(DebugTarget),
                    Layout = layout
                };
                config.AddTarget(debuggerTarget);
                LoggingRule debugRule = new LoggingRule("*", logLevel, debuggerTarget);
                config.LoggingRules.Add(debugRule);
            }

            return config;
        }
    }
}
