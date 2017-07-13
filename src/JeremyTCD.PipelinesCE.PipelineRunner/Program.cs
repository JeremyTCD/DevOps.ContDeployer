using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StructureMap;
using System;

namespace JeremyTCD.PipelinesCE.PipelineRunner
{
    public class Program
    {
        private static int Main(string[] args)
        {
            ILoggingService<Program> loggingService = null;
            IContainer container = null;
            int exitCode = 1;

            try
            {
                // Parse args into PipelineOptions
                // TODO logic to catch errors which can occur if Tools version is different for program that supplied args
                PipelineOptions pipelineOptions = ParseArgs(args);

                // Initialize container
                container = new Container(new PipelineRunnerRegistry());

                // Configure configurable services
                Startup startup = new Startup();
                startup.Configure(container, pipelineOptions);

                // Create logger
                loggingService = container.GetInstance<ILoggingService<Program>>();

                Root root = container.GetInstance<Root>();
                root.Run(pipelineOptions);
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

        // TODO should be private or internal
        public static PipelineOptions ParseArgs(string[] args)
        {
            // Attempt to deserialize, require exact match member wise
            // If members do not match, advise user to update PipelinesCE (CLA) and PipelineRunner to their latest versions
            // - as far as possible, try to run (when developing, keep PipelineOptions backward compatible)
            //   - if PipelinesCE is newer, ignore the new members (notify user). This way user wont have to install older version just to interact with a project that hasn't updated its PipelineRunner package.
            //   - if pipelinerunner is newer, continue using default for the field (every field must have default)

            return JsonConvert.DeserializeObject<PipelineOptions>(args[0]);
        }
    }
}
