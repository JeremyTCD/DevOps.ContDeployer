using JeremyTCD.DotNetCore.Utils;
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
                // TODO logic to catch errors which can occur if Tools version is different for program that supplied args
                PipelineOptions pipelineOptions = JsonConvert.DeserializeObject<PipelineOptions>(args[0], new PrivateFieldsJsonConverter());

                // Initialize container
                container = new Container(new PipelineRunnerRegistry());

                // Configure configurable services
                Startup startup = new Startup();
                startup.Configure(container, pipelineOptions);

                // Create logger
                _loggingService = container.GetInstance<ILoggingService<Program>>();

                Root root = container.GetInstance<Root>();
                root.Run(pipelineOptions);
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
        public static PipelineOptions ParseArgs(string[] args)
        {
            PrivateFieldsJsonConverter converter = new PrivateFieldsJsonConverter();

            PipelineOptions result = JsonConvert.DeserializeObject<PipelineOptions>(args[0], converter);


            foreach(FieldInfo field in converter.MissingFields)
            {
                // log
            }

            foreach(KeyValuePair<string, JToken> pair in converter.ExtraFields)
            {
                // log
            }

            return result;
        }
    }
}
