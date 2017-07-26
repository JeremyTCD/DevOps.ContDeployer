using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.Newtonsoft.Json.Utils;
using JeremyTCD.PipelinesCE.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JeremyTCD.PipelinesCE.ConfigHost
{
    public class ConfigHostStartup
    {
        /// <summary>
        /// Parses args, creates container and starts config host.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        // TODO should be private or internal
        public static int Main(string[] args)
        {
            ILoggingService<ConfigHostStartup> loggingService = null;
            IContainer container = null;
            int exitCode = 1;

            try
            {
                // Parse args into PipelineOptions
                (PipelinesCEOptions pipelineOptions, SharedPluginOptions sharedPluginOptions, string parseArgsWarnings) = ParseArgs(args);

                // Initialize container
                container = CreateContainer();

                // Configure configurable services
                Configure(container, pipelineOptions);

                // Create logger
                loggingService = container.GetInstance<ILoggingService<ConfigHostStartup>>();
                if (!string.IsNullOrEmpty(parseArgsWarnings))
                {
                    loggingService.LogWarning(parseArgsWarnings);
                }

                ConfigHostCore core = container.GetInstance<ConfigHostCore>();
                core.Start(pipelineOptions, sharedPluginOptions);

                container.Dispose();
                exitCode = 0;
            }
            catch (Exception exception)
            {
                // Catch unhandled exceptions and log them using loggingService. This ensures that unhandled exceptions are logged by all
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
        /// <summary>
        /// Parses args. Logs if there is a version mismatch between PipelinesCE executable and config project
        /// </summary>
        /// <param name="args"></param>
        /// <returns>
        /// <see cref="PipelinesCEOptions"/>
        /// </returns>
        public static (PipelinesCEOptions, SharedPluginOptions, string) ParseArgs(string[] args)
        {
            PrivateFieldsJsonConverter converter = new PrivateFieldsJsonConverter();
            PipelinesCEOptions pipelinesCEOptions = JsonConvert.DeserializeObject<PipelinesCEOptions>(args[0], converter);
            SharedPluginOptions sharedPluginOptions = JsonConvert.DeserializeObject<SharedPluginOptions>(args[1], converter);

            string warnings = null; // Can't log until LoggerFactory is setup

            if (converter.MissingFields.Count > 0 || converter.ExtraFields.Count > 0)
            {
                string missingFields = "";
                string extraFields = "";

                foreach (FieldInfo field in converter.MissingFields)
                {
                    missingFields += Environment.NewLine + NormalizeFieldName(field.Name);
                }

                foreach (KeyValuePair<string, JToken> pair in converter.ExtraFields)
                {
                    extraFields += Environment.NewLine + NormalizeFieldName(pair.Key);
                }

                warnings = string.Format(Strings.Log_ExecutableAndProjectVersionsDoNotMatch, extraFields, missingFields);
            }

            return (pipelinesCEOptions, sharedPluginOptions, warnings);
        }

        // TODO should be private or internal
        /// <summary>
        /// Use <see cref="Container"/> instead of <see cref="IServiceProvider"/> since plugins require child containers
        /// </summary>
        /// <returns>
        /// <see cref="Container"/>
        /// </returns>
        public static Container CreateContainer()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddConfigHost();

            return new Container(registry => registry.Populate(services));
        }

        // TODO should be private or internal
        public static void Configure(IContainer container, PipelinesCEOptions pipelineOptions)
        {
            ILoggerFactory loggerFactory = container.GetInstance<ILoggerFactory>();

            // If need be, claOptions should be made configurable via json (using microsoft.extensions.configuration)
            LogLevel logLevel = pipelineOptions.Verbose ? PipelinesCEOptions.VerboseMinLogLevel : PipelinesCEOptions.DefaultMinLogLevel;

            loggerFactory.
                AddConsole(logLevel).
                AddFile(PipelinesCEOptions.LogFileFormat, logLevel).
                AddDebug(logLevel);
        }

        // TODO should be private or internal
        public static string NormalizeFieldName(string fieldName)
        {
            return fieldName.Replace("_", "").ToLowerInvariant();
        }
    }
}
