﻿using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public class CommandLineApp
    {
        public static int Main(string[] args)
        {
            // Make all args lowercase
            args = args.Select(s => s.ToLowerInvariant()).ToArray();

            // Create service provider for CLA
            Startup startup = new Startup();
            IServiceCollection services = new ServiceCollection();
            startup.ConfigureServices(services);
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            // Configure logging and create logging service
            ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            bool verbose = args.Where(s => s == $"--{Strings.VerboseOptionLongName}" ||
                s == $"--{Strings.VerboseOptionLongName}=on" ||
                s == $"-{Strings.VerboseOptionShortName}" ||
                s == $"-{Strings.VerboseOptionShortName}=on").Count() > 0;
            startup.Configure(loggerFactory, verbose);
            ILoggingService<CommandLineApp> loggingService = serviceProvider.GetService<ILoggingService<CommandLineApp>>();

            try
            {
                if (loggingService.IsEnabled(LogLevel.Debug))
                {
                    loggingService.LogDebug(Strings.Log_RunningCommandLineApp, string.Join(",", args));
                }

                RootCommand rootCommand = serviceProvider.GetService<RootCommand>();
                rootCommand.Execute(args);
            }
            catch (Exception exception)
            {
                // Catch unhandled exceptions and log them using logger. This ensures that unhandled exceptions are logged by all
                // logging providers (such as file, debug etc - not just console).
                loggingService.LogError(exception.ToString());
                return 1;
            }

            return 0;
        }
    }
}
