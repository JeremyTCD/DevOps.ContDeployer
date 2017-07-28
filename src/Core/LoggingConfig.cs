using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Targets;
using NLog.Extensions.Logging;
using LogLevel = NLog.LogLevel;

namespace JeremyTCD.PipelinesCE.Core
{
    /// <summary>
    /// Programatically configures NLog provider for <see cref="ILoggerFactory"/>. This class facilitates consistent logging configuration
    /// across AssemblyLoadContexts.
    /// </summary>
    public class LoggingConfig
    {
        public static void Configure(ILoggerFactory loggerFactory, PipelinesCEOptions pipelinesCEOptions)
        {
            LogLevel logLevel = pipelinesCEOptions.Debug || pipelinesCEOptions.Verbose ? LogLevel.Debug :
                LogLevel.Info;
            string layout = "[${longdate}][${logger}][${level: uppercase = true}] ${message}";

            loggerFactory.
                AddNLog();

            LoggingConfiguration config = new LoggingConfiguration();

            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget
            {
                Layout = layout
            };
            config.AddTarget(nameof(ConsoleTarget), consoleTarget);
            LoggingRule consoleRule = new LoggingRule("*", logLevel, consoleTarget);
            config.LoggingRules.Add(consoleRule);

            FileTarget fileTarget = new FileTarget
            {
                FileName = pipelinesCEOptions.LogFile,
                Layout = layout
            };
            config.AddTarget(nameof(FileTarget), fileTarget);
            LoggingRule fileRule = new LoggingRule("*", logLevel, fileTarget);
            config.LoggingRules.Add(fileRule);

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

            loggerFactory.
                ConfigureNLog(config);
        }
    }
}
