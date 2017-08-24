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

namespace JeremyTCD.PipelinesCE.Config
{
    public class ConfigProgram
    {
        /// <summary>
        /// Parses args, creates container and starts config host.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public int Run(string[] args)
        {
            // Parse args into PipelineOptions
            // TODO parse using commandlineparser
            (PipelinesCEOptions pipelineOptions, SharedStepOptions sharedPluginOptions, string parseArgsWarnings) = ParseArgs(args);

            // Initialize container
            using (IContainer container = CreateContainer())
            {
                // Configure logging
                ILoggerFactory loggerFactory = container.GetInstance<ILoggerFactory>();
                ILoggingConfig loggingConfig = container.GetInstance<ILoggingConfig>();
                loggingConfig.Configure(loggerFactory, pipelineOptions);

                // Create logger
                ILoggingService<ConfigProgram> loggingService = container.GetInstance<ILoggingService<ConfigProgram>>();
                if (!string.IsNullOrEmpty(parseArgsWarnings))
                {
                    loggingService.LogWarning(parseArgsWarnings);
                }

                // Start
                ConfigRunner runner = container.GetInstance<ConfigRunner>();
                runner.Run(pipelineOptions, sharedPluginOptions);
            }

            return 0;
        }

        /// <summary>
        /// Parses args. Logs if there is a version mismatch between PipelinesCE executable and config project
        /// </summary>
        /// <param name="args"></param>
        /// <returns>
        /// <see cref="PipelinesCEOptions"/>
        /// </returns>
        internal static (PipelinesCEOptions, SharedStepOptions, string) ParseArgs(string[] args)
        {
            PrivateFieldsJsonConverter converter = new PrivateFieldsJsonConverter();
            PipelinesCEOptions pipelinesCEOptions = JsonConvert.DeserializeObject<PipelinesCEOptions>(args[0], converter);
            SharedStepOptions sharedPluginOptions = JsonConvert.DeserializeObject<SharedStepOptions>(args[1], converter);

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

        /// <summary>
        /// Use <see cref="Container"/> instead of <see cref="IServiceProvider"/> since plugins require child containers
        /// </summary>
        /// <returns>
        /// <see cref="Container"/>
        /// </returns>
        internal static Container CreateContainer()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddConfigHost();

            return new Container(registry => registry.Populate(services));
        }

        internal static string NormalizeFieldName(string fieldName)
        {
            return fieldName.Replace("_", "").ToLowerInvariant();
        }
    }
}
