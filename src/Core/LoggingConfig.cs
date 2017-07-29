using Microsoft.Extensions.Logging;
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
                Layout = layout
            };
            config.AddTarget(nameof(ConsoleTarget), consoleTarget);
            LoggingRule consoleRule = new LoggingRule("*", logLevel, consoleTarget);
            config.LoggingRules.Add(consoleRule);

            // File
            if (pipelinesCEOptions.FileLogging)
            {
                string logFile = pipelinesCEOptions.LogFile;
                if (!_pathService.IsPathRooted(logFile))
                {
                    string projectDir = _directoryService.GetParent(pipelinesCEOptions.ProjectFile).FullName;
                    logFile = _pathService.Combine(projectDir, logFile);
                }

                FileTarget fileTarget = new FileTarget
                {
                    FileName = logFile,
                    Layout = layout
                };
                config.AddTarget(nameof(FileTarget), fileTarget);
                LoggingRule fileRule = new LoggingRule("*", logLevel, fileTarget);
                config.LoggingRules.Add(fileRule);
            }

            // Debugger
            if (pipelinesCEOptions.Debug || pipelinesCEOptions.Verbose)
            {
                DebuggerTarget debuggerTarget = new DebuggerTarget
                {
                    Layout = layout
                };
                config.AddTarget(nameof(DebugTarget), debuggerTarget);
                LoggingRule debugRule = new LoggingRule("*", logLevel, debuggerTarget);
                config.LoggingRules.Add(debugRule);
            }

            return config;
        }
    }
}
