using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.Newtonsoft.Json.Utils;
using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JeremyTCD.PipelinesCE.PipelineRunner
{
    public class Program
    {
        private static ILoggingService<Program> _loggingService = null;

        private static int Main(string[] args)
        {
            IContainer container = null;
            int exitCode = 1;

            try
            {
                // Parse args into PipelineOptions
                PipelineOptions pipelineOptions = JsonConvert.DeserializeObject<PipelineOptions>(args[0], new PrivateFieldsJsonConverter());

                // Initialize container
                container = new Container(new PipelineRunnerRegistry());

                // Configure configurable services
                Startup startup = new Startup();
                startup.Configure(container, pipelineOptions);

                // Create logger
                _loggingService = container.GetInstance<ILoggingService<Program>>();

                container.GetInstance<Root>().Run(pipelineOptions);
                exitCode = 0;
            }
            catch (Exception exception)
            {
                // Catch unhandled exceptions and log them using logger. This ensures that unhandled exceptions are logged by all
                // logging providers (such as file, debug etc - not just console).
                if (_loggingService != null)
                {
                    _loggingService.LogError(exception.ToString());
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
        public static PipelineOptions ParseArgs(string[] args)
        {
            PrivateFieldsJsonConverter converter = new PrivateFieldsJsonConverter();
            PipelineOptions result = JsonConvert.DeserializeObject<PipelineOptions>(args[0], converter);

            if (converter.MissingFields.Count > 0 || converter.ExtraFields.Count > 0)
            {
                string missingFields = "";
                string extraFields = "";

                foreach (FieldInfo field in converter.MissingFields)
                {
                    missingFields += Environment.NewLine + field.Name.Replace("_", "").ToLowerInvariant();
                }

                foreach (KeyValuePair<string, JToken> pair in converter.ExtraFields)
                {
                    extraFields += Environment.NewLine + pair.Key.Replace("_", "").ToLowerInvariant();
                }

                _loggingService.LogWarning(Strings.Log_ExecutableAndProjectVersionsDoNotMatch, extraFields, missingFields);
            }

            return result;
        }
    }
}
