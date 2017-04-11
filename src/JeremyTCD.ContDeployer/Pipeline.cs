using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace JeremyTCD.ContDeployer
{
    public class Pipeline
    {
        public ILogger<Pipeline> Logger { get; }
        public PipelineOptions Options { get; }
        public PluginFactory PluginFactory { get; }

        public Pipeline(IOptions<PipelineOptions> optionsAccessor,
            ILogger<Pipeline> logger,
            PluginFactory pluginFactory)
        {
            Logger = logger;
            Options = optionsAccessor.Value;
            PluginFactory = pluginFactory;
        }

        /// <summary>
        /// Runs each pipeline step serially
        /// </summary>
        public void Run()
        {
            // Use linked list since steps will be added to and removed from start of list
            LinkedList<PipelineStep> steps = new LinkedList<PipelineStep>(Options.PipelineSteps);

            Logger.LogInformation("=== Starting pipeline ===");

            while (steps.Count > 0)
            {
                PipelineStep step = steps.First();

                IPlugin plugin = PluginFactory.BuildPlugin(step.PluginName, (object) step.Config ?? step.Options);

                steps.RemoveFirst();

                Logger.LogInformation($"Running step with plugin: {step.PluginName}");
                plugin.Run(steps);
                Logger.LogInformation("Step complete");
            }

            Logger.LogInformation("=== Pipeline complete ===");
        }
    }
}
