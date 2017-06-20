using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Logging;
using StructureMap;
using System;
using System.Linq;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public class CommandLineApp
    {
        public static int Main(string[] args)
        {
            ILoggingService<CommandLineApp> loggingService = null;
            IContainer container = null;
            int exitCode = 1;

            try
            {
                // Make all args lowercase
                args = args.Select(s => s.ToLowerInvariant()).ToArray();

                // Initialize container
                container = new Container(new CommandLineAppRegistry());
                    
                // Configure logging and create logging service
                Startup startup = new Startup();
                ILoggerFactory loggerFactory = container.GetInstance<ILoggerFactory>();
                startup.Configure(loggerFactory, args);
                loggingService = container.GetInstance<ILoggingService<CommandLineApp>>();

                if (loggingService.IsEnabled(LogLevel.Debug))
                {
                    loggingService.LogDebug(Strings.Log_RunningCommandLineApp, string.Join(",", args));
                }

                RootCommand rootCommand = container.GetInstance<RootCommand>();
                rootCommand.Execute(args);
                exitCode = 0;
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
    }
}
