using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.Newtonsoft.Json.Utils;
using JeremyTCD.PipelinesCE.Tools;
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
        // TODO should be private or internal
        public static int Main(string[] args)
        {
            ILoggingService<ConfigHostStartup> loggingService = null;
            IContainer container = null;
            int exitCode = 1;

            try
            {
                // Parse args into PipelineOptions
                (PipelineOptions pipelineOptions, string parseArgsWarnings) = ParseArgs(args);

                // Initialize container
                container = new Container(new ConfigHostRegistry());

                // Configure configurable services
                Configure(container, pipelineOptions);

                // Create logger
                loggingService = container.GetInstance<ILoggingService<ConfigHostStartup>>();
                if (!string.IsNullOrEmpty(parseArgsWarnings))
                {
                    loggingService.LogWarning(parseArgsWarnings);
                }

                ConfigHostCore core = container.GetInstance<ConfigHostCore>();
                core.Run(pipelineOptions);

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
        /// <see cref="PipelineOptions"/>
        /// </returns>
        public static (PipelineOptions, string) ParseArgs(string[] args)
        {
            PrivateFieldsJsonConverter converter = new PrivateFieldsJsonConverter();
            PipelineOptions result = JsonConvert.DeserializeObject<PipelineOptions>(args[0], converter);
            string warnings = null; // Can't log until LoggerFactory is setup, setting up LoggerFactory

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

            return (result, warnings);
        }

        // TODO should be private or internal
        public static void Configure(IContainer container, PipelineOptions pipelineOptions)
        {
            ILoggerFactory loggerFactory = container.GetInstance<ILoggerFactory>();

            // If need be, claOptions should be made configurable via json (using microsoft.extensions.configuration)
            LogLevel logLevel = pipelineOptions.Verbose ? PipelineOptions.VerboseMinLogLevel : PipelineOptions.DefaultMinLogLevel;

            loggerFactory.
                AddConsole(logLevel).
                AddFile(PipelineOptions.LogFileFormat, logLevel).
                AddDebug(logLevel);
        }

        // TODO should be private or internal
        public static string NormalizeFieldName(string fieldName)
        {
            return fieldName.Replace("_", "").ToLowerInvariant();
        }
    }
}
