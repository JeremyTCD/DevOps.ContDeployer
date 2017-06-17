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
            IContainer mainContainer = null;
            IContainer claContainer = null;
            int exitCode = 1;

            try
            {
                // Make all args lowercase
                args = args.Select(s => s.ToLowerInvariant()).ToArray();

                // Create service provider for CLA
                Startup startup = new Startup();
                mainContainer = startup.ConfigureServices();
                claContainer = mainContainer.GetProfile(nameof(CommandLineApp)); 
                // TODO does child container overwrite IContainer service? if it does then register
                // IContainer service manually for child container

                // Configure logging and create logging service
                ILoggerFactory loggerFactory = mainContainer.GetInstance<ILoggerFactory>();
                startup.Configure(loggerFactory, args);
                loggingService = mainContainer.GetInstance<ILoggingService<CommandLineApp>>();

                if (loggingService.IsEnabled(LogLevel.Debug))
                {
                    loggingService.LogDebug(Strings.Log_RunningCommandLineApp, string.Join(",", args));
                }

                RootCommand rootCommand = claContainer.GetInstance<RootCommand>();
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
                if (mainContainer != null)
                {
                    mainContainer.Dispose();
                    // TODO Ensure that everything is disposed
                }
                if(claContainer != null)
                {
                    claContainer.Dispose();
                }
            }

            return exitCode;
        }
    }
}
