using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Logging;
using StructureMap;
using System;
using System.Linq;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public class Program
    {
        public static int Main(string[] args)
        {
            ILoggingService<Program> loggingService = null;
            IContainer container = null;
            int exitCode = 1;

            try
            {
                // Initialize container
                container = new Container(new CommandLineAppRegistry());

                // Configure configurable services
                Configure(container, args);

                // Create logger
                loggingService = container.GetInstance<ILoggingService<Program>>();

                if (loggingService.IsEnabled(LogLevel.Debug))
                {
                    loggingService.LogDebug(Strings.Log_RunningCommandLineApp, string.Join(",", args));
                }

                RootCommand rootCommand = container.GetInstance<RootCommand>();
                exitCode = rootCommand.Execute(args);
            }
            catch (Exception exception) 
            {
                // Catch unhandled exceptions and log them using logger. This ensures that unhandled exceptions are logged by all
                // logging providers (such as file, debug etc - not just console).
                if (loggingService != null)
                {
                    loggingService.LogError(exception.ToString());
                }
            }
            finally
            {
                if (container != null)
                {
                    container.Dispose();
                }
            }

            return exitCode;
        }

        // TODO should be private or internal
        public static void Configure(IContainer container, string[] args)
        {
            ILoggerFactory loggerFactory = container.GetInstance<ILoggerFactory>();

            bool verbose = args.Where(s => s == $"--{Strings.OptionLongName_Verbose}" ||
                s == $"-{Strings.OptionShortName_Verbose}").Count() > 0;

            CommandLineAppOptions claOptions = new CommandLineAppOptions();
            LogLevel logLevel = verbose ? claOptions.VerboseMinLogLevel : claOptions.DefaultMinLogLevel;

            loggerFactory.
                AddConsole(logLevel).
                AddFile(claOptions.LogFileFormat, logLevel).
                AddDebug(logLevel);
        }
    }
}
